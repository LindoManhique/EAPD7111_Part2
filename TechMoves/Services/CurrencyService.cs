using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TechMoves.Interfaces;

namespace TechMoves.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyService> _logger;
        private readonly IConfiguration _configuration;

        private static decimal? _cachedRate;
        private static DateTime _lastFetchTime;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public CurrencyService(
            HttpClient httpClient,
            ILogger<CurrencyService> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                if (_cachedRate.HasValue &&
                    DateTime.UtcNow - _lastFetchTime < CacheDuration)
                {
                    _logger.LogInformation("Using cached USD/ZAR rate: {rate}", _cachedRate.Value);
                    return _cachedRate.Value;
                }

                var apiUrl = _configuration["CurrencyApi:Url"]
                             ?? "https://api.exchangerate.host/latest?base=USD&symbols=ZAR";

                _logger.LogInformation("Fetching USD/ZAR rate from API");

                _httpClient.Timeout = TimeSpan.FromSeconds(5);

                var response = await _httpClient.GetStringAsync(apiUrl);

                using var doc = JsonDocument.Parse(response);

                if (!doc.RootElement.TryGetProperty("rates", out var rates))
                {
                    throw new Exception("Invalid API response: missing rates");
                }

                if (!rates.TryGetProperty("ZAR", out var zarRate))
                {
                    throw new Exception("Invalid API response: missing ZAR");
                }

                decimal rate = zarRate.GetDecimal();

                _cachedRate = rate;
                _lastFetchTime = DateTime.UtcNow;

                _logger.LogInformation("Cached new USD/ZAR rate: {rate}", rate);

                return rate;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Currency API failed, using fallback rate");

                return 18.50m;
            }
        }

        public decimal ConvertUsdToZar(decimal usd, decimal rate)
        {
            if (usd < 0)
            {
                throw new ArgumentException("USD amount cannot be negative");
            }

            return Math.Round(usd * rate, 2);
        }
    }
}