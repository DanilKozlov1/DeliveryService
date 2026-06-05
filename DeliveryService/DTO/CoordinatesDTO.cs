namespace DeliveryService.DTO
{
    /// <summary>
    /// Класс-DTO точек пути курьера для MapInitialization
    /// </summary>
    public class CoordinatesDTO
    {
        /// <summary>
        /// Тип
        /// </summary>
        public string? type { get; set; }
        /// <summary>
        /// Список координат пути
        /// </summary>
        public List<List<double>>? coordinates { get; set; }
    }
}
