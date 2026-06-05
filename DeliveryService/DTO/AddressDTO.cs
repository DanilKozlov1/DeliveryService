namespace DeliveryService.DTO
{
    /// <summary>
    /// Класс-DTO адреса для MapInitialization
    /// </summary>
    public class AddressDTO
    {
        /// <summary>
        /// Тип
        /// </summary>
        public string type { get; set; } = string.Empty;
        /// <summary>
        /// Широта
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// Долгота
        /// </summary>
        public double lon { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        public string address { get; set; } = string.Empty;

    }
}
