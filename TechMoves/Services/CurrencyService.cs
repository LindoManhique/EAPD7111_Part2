using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TechMoves.Models;
using TechMoves.Interfaces;

namespace TechMoves.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        // CACHE 
        private static decimal? _cachedRate;
        private static DateTime _lastFetchTime;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ================================
        // MAIN API METHOD 
        // ================================
        public async Task<decimal> GetUsdToZarRateAsync()
        {
            // Use cache (10 min)
            if (_cachedRate.HasValue &&
                DateTime.UtcNow < _lastFetchTime.AddMinutes(10))
            {
                return _cachedRate.Value;
            }

            string url = "https://api.exchangerate.host/latest?base=USD&symbols=ZAR";

            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(5);

                var response = await _httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);

                if (!doc.RootElement.TryGetProperty("rates", out var rates))
                {
                    throw new Exception("API response missing 'rates'");
                }

                if (!rates.TryGetProperty("ZAR", out var zarRate))
                {
                    throw new Exception("API response missing 'ZAR'");
                }

                decimal rate = zarRate.GetDecimal();

                // cache update
                _cachedRate = rate;
                _lastFetchTime = DateTime.UtcNow;

                return rate;
            }
            catch
            {
                // fallback (system never crashes)
                return 18.50m;
            }
        }

        // ================================
        // TESTABLE METHOD 
        // ================================
        public decimal ConvertUsdToZar(decimal usd, decimal rate)
        {
            if (usd < 0)
                throw new ArgumentException("USD cannot be negative");

            return usd * rate;
        }
    }
}