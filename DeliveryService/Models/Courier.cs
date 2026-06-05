namespace DeliveryService.Models
{
    /// <summary>
    /// Модель таблицы Clients
    /// </summary>
    public class Courier
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Номер телефона
        /// </summary>
        public required string CourierPhone { get; set; }
        /// <summary>
        /// Онлайн ли
        /// </summary>
        public bool IsActive { get; set; } = false;
        /// <summary>
        /// Текущая широта
        /// </summary>
        public double Current_Lat { get; set; }
        /// <summary>
        /// Текущая долгота
        /// </summary>
        public double Current_Lon { get; set; }
        /// <summary>
        /// Тип средства передвижения
        /// </summary>
        public string Vehicle_Type { get; set; } = string.Empty;
        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Список заказов курьера
        /// </summary>
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
