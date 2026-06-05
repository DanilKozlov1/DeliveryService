namespace DeliveryService.Models
{
    /// <summary>
    /// Модель таблицы Baskets
    /// </summary>
    public class Basket
    {
        /// <summary>
        /// Id объекта корзины
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id клиента
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Id еды
        /// </summary>
        public int FoodId { get; set; }
        /// <summary>
        /// Количество еды
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Объект еды
        /// </summary>
        public Food? Food { get; set; }
    }
}
