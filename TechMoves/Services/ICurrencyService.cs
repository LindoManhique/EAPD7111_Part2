using System.Threading.Tasks;

namespace TechMoves.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> GetUsdToZarRateAsync();

        decimal ConvertUsdToZar(decimal usd, decimal rate);
    }
}