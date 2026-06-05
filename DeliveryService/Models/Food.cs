namespace DeliveryService.Models
{
    /// <summary>
    /// Модель таблицы Foods
    /// </summary>
    public class Food
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public required string Title { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Ссылка на изображение
        /// </summary>
        public string? ImageUrl { get; set; }
        /// <summary>
        /// Вес
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// Id категории
        /// </summary>
        public int CategoriesId { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Объект категории
        /// </summary>
        public Categories? Categories { get; set; }
        /// <summary>
        /// Список объектов корзины
        /// </summary>
        public ICollection<Basket> Baskets { get; set; } = new List<Basket>();
    }
}
