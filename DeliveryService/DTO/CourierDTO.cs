namespace DeliveryService.DTO
{
    /// <summary>
    /// Класс-DTO для отображения Курьера в OrderListView
    /// </summary>
    public class CourierDTO
    {
        /// <summary>
        /// Id курьера
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя курьера
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}