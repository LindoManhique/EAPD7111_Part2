using Xunit;
using TechMoves.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
namespace TechMoves.Tests
{
    public class CurrencyServiceTests
    {
        [Fact]
        public void ConvertUsdToZar_ShouldReturnCorrectValue()
        {
            var service = new CurrencyService(new HttpClient());

            decimal usd = 10;
            decimal rate = 18.5m;

            var result = service.ConvertUsdToZar(usd, rate);

            Assert.Equal(185m, result);
        }

        [Fact]
        public void ConvertUsdToZar_ShouldReturnZero_WhenUsdIsZero()
        {
            var service = new CurrencyService(new HttpClient());

            var result = service.ConvertUsdToZar(0, 18.5m);

            Assert.Equal(0, result);
        }

        [Fact]
        public void ConvertUsdToZar_ShouldThrow_WhenNegative()
        {
            var service = new CurrencyService(new HttpClient());

            Assert.Throws<ArgumentException>(() =>
            {
                service.ConvertUsdToZar(-5, 18.5m);
            });
        }
    }
}