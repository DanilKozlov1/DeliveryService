namespace DeliveryService.Models
{
    /// <summary>
    /// Модель таблицы Clients
    /// </summary>
    public class Client
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
        public string Phone { get; set; } = string.Empty;
        /// <summary>
        /// Почта
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Роль
        /// </summary>
        public string Role { get; set; } = string.Empty;
        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime Created_At { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Список заказов пользователя
        /// </summary>
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
    