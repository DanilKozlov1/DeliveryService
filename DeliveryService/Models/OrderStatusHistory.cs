namespace DeliveryService.Models
{
    /// <summary>
    /// Модель таблицы OrderStatusHistories
    /// </summary>
    public class OrderStatusHistory
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
        /// Статус заказа
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Дата изменения статуса
        /// </summary>
        public DateTime Changed_At { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Описание
        /// </summary>
        public string? FeedBack { get; set; }

        /// <summary>
        /// Объект заказа
        /// </summary>
        public Order Order { get; set; } = null!;
    }
}
