using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TechMoves.API.Models;
using Xunit;

namespace TechMoves.Tests.IntergrationTests
{
    public class ContractsApiTests : IClassFixture<CustomWebFactory>
    {
        private readonly HttpClient _client;

        public ContractsApiTests(CustomWebFactory factory)
        {
            _client = factory.CreateClient();
        }

        // =========================
        // CREATE → READ (CORE TEST)
        // =========================
        [Fact]
        public async Task Create_Then_Get_ShouldReturnSameContract()
        {
            var contract = new Contract
            {
                ClientId = 1,
                Status = "Active",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                ServiceLevel = "Gold"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/contracts", contract);

            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createResponse.Content.ReadFromJsonAsync<Contract>();

            var getResponse = await _client.GetAsync($"/api/contracts/{created!.Id}");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var fetched = await getResponse.Content.ReadFromJsonAsync<Contract>();

            fetched!.ClientId.Should().Be(contract.ClientId);
            fetched.Status.Should().Be(contract.Status);
        }

        // =========================
        // DELETE → VERIFY GONE
        // =========================
        [Fact]
        public async Task Delete_Contract_ShouldRemoveIt()
        {
            var create = await _client.PostAsJsonAsync("/api/contracts", new Contract
            {
                ClientId = 1,
                Status = "Active",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                ServiceLevel = "Gold"
            });

            var contract = await create.Content.ReadFromJsonAsync<Contract>();

            var delete = await _client.DeleteAsync($"/api/contracts/{contract!.Id}");

            delete.StatusCode.Should().BeOneOf(HttpStatusCode.NoContent, HttpStatusCode.OK);

            var getAfterDelete = await _client.GetAsync($"/api/contracts/{contract.Id}");

            getAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // =========================
        // PATCH STATUS TEST
        // =========================
        [Fact]
        public async Task Patch_Status_ShouldUpdateContract()
        {
            var create = await _client.PostAsJsonAsync("/api/contracts", new Contract
            {
                ClientId = 1,
                Status = "Draft",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                ServiceLevel = "Silver"
            });

            var contract = await create.Content.ReadFromJsonAsync<Contract>();

            var patch = await _client.PatchAsJsonAsync(
                $"/api/contracts/{contract!.Id}/status",
                "Active"
            );

            patch.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updated = await _client.GetAsync($"/api/contracts/{contract.Id}");
            var updatedContract = await updated.Content.ReadFromJsonAsync<Contract>();

            updatedContract!.Status.Should().Be("Active");
        }

        // =========================
        // GET ALL CONTRACTS
        // =========================
        [Fact]
        public async Task Get_All_ShouldReturnSuccess()
        {
            var response = await _client.GetAsync("/api/contracts");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<List<Contract>>();

            data.Should().NotBeNull();
        }
    }
}