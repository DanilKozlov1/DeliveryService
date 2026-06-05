namespace DeliveryService.Models
{
    /// <summary>
    /// Модель таблицы RoutePoints
    /// </summary>
    public class RoutePoint
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id заказа
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// Широта
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// Долгота
        /// </summary>
        public double Lon { get; set; }
        /// <summary>
        /// Дата записи
        /// </summary>
        public DateTime Recorded_At { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Объект заказа
        /// </summary>
        public Order Order { get; set; } = null!;
    }
}
