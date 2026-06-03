namespace DeliveryService.Services.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса конфигурации
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// Получение API-ключа для карты
        /// </summary>
        /// <returns>API-ключ</returns>
        string GetMapApiKey();
    }
}