using Xunit;
using TechMoves.Services;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;

namespace TechMoves.Tests
{
    public class CurrencyServiceTests
    {
        private CurrencyService CreateService()
        {
            var httpClient = new HttpClient();

            var logger = new Mock<ILogger<CurrencyService>>();
            var config = new Mock<IConfiguration>();

            return new CurrencyService(
                httpClient,
                logger.Object,
                config.Object
            );
        }

        [Fact]
        public void ConvertUsdToZar_ShouldReturnCorrectValue()
        {
            var service = CreateService();

            decimal usd = 10;
            decimal rate = 18.5m;

            var result = service.ConvertUsdToZar(usd, rate);

            Assert.Equal(185m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ShouldReturnZero_WhenUsdIsZero()
        {
            var service = CreateService();

            var result = service.ConvertUsdToZar(0, 18.5m);

            Assert.Equal(0, result);
        }

        [Fact]
        public void ConvertUsdToZar_ShouldThrow_WhenNegative()
        {
            var service = CreateService();

            Assert.Throws<ArgumentException>(() =>
            {
                service.ConvertUsdToZar(-5, 18.5m);
            });
        }
    }
}