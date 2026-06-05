namespace DeliveryService.DTO
{
    /// <summary>
    /// Класс-DTO для отображения Заказа в OrderListView
    /// </summary>
    public class OrderDTO
    {
        /// <summary>
        /// Id заказа
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя клиента
        /// </summary>
        public string ClientName { get; set; } = string.Empty;
        /// <summary>
        /// Путь откуда и куда
        /// </summary>
        public string Route { get; set; } = string.Empty;
        /// <summary>
        /// Статус заказа
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Цена заказа
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Дата доставки заказа
        /// </summary>
        public string OrderTime { get; set; } = string.Empty;
        /// <summary>
        /// Id курьера
        /// </summary>
        public int? CourierId { get; set; }
    }
}