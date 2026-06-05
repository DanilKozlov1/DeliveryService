namespace DeliveryService.Models
{
    /// <summary>
    /// Модель таблицы Categories
    /// </summary>
    public class Categories
    {
        /// <summary>
        /// Id категории
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Список подписанных к категории объектов еды
        /// </summary>
        public ICollection<Food?> Foods { get; set; }  = new List<Food?>();
    }
}
