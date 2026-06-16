using System.Net.Http.Json;
using TechMoves.API.Models;

namespace TechMoves.API.Services
{
    public class ApiClientService
    {
        private readonly HttpClient _http;

        public ApiClientService(HttpClient http)
        {
            _http = http;
        }

        // GET ALL CONTRACTS
        public async Task<List<Contract>> GetContracts()
        {
            return await _http.GetFromJsonAsync<List<Contract>>("api/contracts");
        }

        // GET BY ID
        public async Task<Contract> GetContract(int id)
        {
            return await _http.GetFromJsonAsync<Contract>($"api/contracts/{id}");
        }

        // CREATE
        public async Task<bool> CreateContract(Contract contract)
        {
            var response = await _http.PostAsJsonAsync("api/contracts", contract);
            return response.IsSuccessStatusCode;
        }

        // UPDATE STATUS
        public async Task<bool> UpdateStatus(int id, string status)
        {
            var response = await _http.PatchAsJsonAsync(
                $"api/contracts/{id}/status",
                status
            );

            return response.IsSuccessStatusCode;
        }
    }
}