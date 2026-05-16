using TechMoves.Models;
using TechMoves.Services;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;

namespace TechMoves.Tests
{
    public class ContractServiceTests
    {
        [Fact]
        public void ActiveContract_ShouldAllow_ServiceRequest()
        {
            var service = new ContractService();

            var contract = new Contract { Status = "Active" };

            var result = service.CanCreateServiceRequest(contract);

            Assert.True(result);
        }

        [Fact]
        public void ExpiredContract_ShouldBlock_ServiceRequest()
        {
            var service = new ContractService();

            var contract = new Contract { Status = "Expired" };

            var result = service.CanCreateServiceRequest(contract);

            Assert.False(result);
        }

        [Fact]
        public void NullContract_ShouldReturnFalse()
        {
            var service = new ContractService();

            var result = service.CanCreateServiceRequest(null);

            Assert.False(result);
        }

        [Fact]
        public void EmptyStatus_ShouldReturnFalse()
        {
            var service = new ContractService();

            var contract = new Contract { Status = "" };

            var result = service.CanCreateServiceRequest(contract);

            Assert.False(result);
        }
    }
}