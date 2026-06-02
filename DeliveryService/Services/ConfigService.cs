using Microsoft.Extensions.Configuration;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис конфигурации
    /// </summary>
    public class ConfigService
    {
        /// <summary>
        /// Конфигурация проекта
        /// </summary>
        private readonly IConfiguration _config;


        public ConfigService(IConfiguration config) 
        { 
            _config = config;
        }


        /// <summary>
        /// Получение API-ключа для карты
        /// </summary>
        /// <returns>API-ключ</returns>
        public string GetMapApiKey() => _config.GetSection("ApiMap:Key").Value;
    }
}