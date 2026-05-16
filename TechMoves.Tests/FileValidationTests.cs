using TechMoves.Services;
using Xunit;

namespace TechMoves.Tests
{
    public class FileValidationTests
    {
        [Fact]
        public void Should_Allow_Pdf()
        {
            var service = new ContractService();

            var result = service.IsValidFile("contract.pdf");

            Assert.True(result);
        }

        [Fact]
        public void Should_Reject_Exe()
        {
            var service = new ContractService();

            var result = service.IsValidFile("virus.exe");

            Assert.False(result);
        }

        [Fact]
        public void Should_Reject_Null()
        {
            var service = new ContractService();

            var result = service.IsValidFile(null);

            Assert.False(result);
        }

        [Fact]
        public void Should_Reject_EmptyString()
        {
            var service = new ContractService();

            var result = service.IsValidFile("");

            Assert.False(result);
        }
    }
}