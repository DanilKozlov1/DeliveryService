using DeliveryService.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис конфигурации
    /// </summary>
    public class ConfigService : IConfigService
    {
        private readonly ILogger<ConfigService> _logger;

        /// <summary>
        /// Конфигурация проекта
        /// </summary>
        private readonly IConfiguration _config;


        public ConfigService(ILogger<ConfigService> logger, IConfiguration config) 
        {
            _logger = logger;
            _config = config;
        }


        /// <summary>
        /// Получение api-ключа для карты
        /// </summary>
        /// <returns>API-ключ</returns>
        public string GetMapApiKey()
        {
            string? apiKey = _config.GetSection("ApiMap:Key").Value;
            
            if (apiKey == null)
                _logger.LogError("API-ключ для карты не существует в конфигурации.");
            return apiKey ?? throw new InvalidOperationException("API-ключ для карты не существует в конфигурации.");
        }
    }
}