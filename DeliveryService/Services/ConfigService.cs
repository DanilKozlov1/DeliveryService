using DeliveryService.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис конфигурации
    /// </summary>
    public class ConfigService : IConfigService
    {
        /// <summary>
        /// Конфигурация проекта
        /// </summary>
        private readonly IConfiguration _config;


        public ConfigService(IConfiguration config) 
        { 
            _config = config;
        }


        public string GetMapApiKey()
        {
            string? apiKey = _config.GetSection("ApiMap:Key").Value;
            return apiKey ?? throw new InvalidOperationException("API-ключ для карты не существует в конфигурации.");
        }
    }
}