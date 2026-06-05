namespace DeliveryService.Models
{
    /// <summary>
    /// Модель таблицы Orders
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id клиента
        /// </summary>
        public int ClientId { get; set; }
        /// <summary>
        /// Id курьера
        /// </summary>
        public int? CourierId { get; set; }
        /// <summary>
        /// Id объекта корзины
        /// </summary>
        public int? BasketId { get; set; }
        /// <summary>
        /// Адрес откуда заказ
        /// </summary>
        public string Address_From { get; set; } = string.Empty;
        /// <summary>
        /// Широта адреса откуда
        /// </summary>
        public double Lat_From { get; set; }
        /// <summary>
        /// Долгота адреса откуда
        /// </summary>
        public double Lon_From { get; set; }
        /// <summary>
        /// Адрес куда доставляется
        /// </summary>
        public string Address_To { get; set; } = string.Empty;
        /// <summary>
        /// Широта адреса куда
        /// </summary>
        public double Lat_To { get; set; }
        /// <summary>
        /// Долгота адреса куда
        /// </summary>
        public double Lon_To { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public string? Status { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Дата когда доставлен
        /// </summary>
        public DateTime? Delivered_At { get; set; }

        /// <summary>
        /// Объект клиента
        /// </summary>
        public Client Client { get; set; } = null!;
        /// <summary>
        /// Объект курьера
        /// </summary>
        public Courier? Courier { get; set; }
        /// <summary>
        /// Объект корзины
        /// </summary>
        public Basket? Basket { get; set; }
        /// <summary>
        /// Список точек пути откуда -> куда
        /// </summary>
        public ICollection<RoutePoint> RoutePoints { get; set; } = new List<RoutePoint>();
        /// <summary>
        /// Список истории статусов
        /// </summary>
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}
