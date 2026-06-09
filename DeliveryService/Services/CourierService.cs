using DeliveryService.Models;
using DeliveryService.Repositories;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, работающий с Курьерами
    /// </summary>
    public class CourierService
    {
        private readonly ILogger<CourierService> _logger;

        private readonly OrderRepository _orderRepository;
        private readonly CourierRepository _courierRepository;


        public CourierService(ILogger<CourierService> logger, OrderRepository orderRepo, CourierRepository courirerRepo)
        {
            _logger = logger;
            _orderRepository = orderRepo;
            _courierRepository = courirerRepo;
        }


        /// <summary>
        /// Получение всех курьеров
        /// </summary>
        /// <returns>Список курьеров</returns>
        public async Task<List<Courier>> GetAllAsync() => await _courierRepository.GetAllAsync();

        /// <summary>
        /// Получение всех активных курьеров
        /// </summary>
        /// <returns>Список активных курьеров</returns>
        public async Task<List<Courier>> GetActiveCouriersAsync() => await _courierRepository.GetActive();

        /// <summary>
        /// Получение всех свободных от заказов курьеров
        /// </summary>
        /// <returns>Список свободных от заказов курьеров</returns>
        public async Task<List<Courier>> GetFreeCouriersAsync() => await _courierRepository.GetFreeCouriers();

        /// <summary>
        /// Добавление курьера в базу данных
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> AddCourierAsync(Courier courier)
        {
            if (courier == null)
            {
                _logger.LogWarning("Попытка добавить курьера с null-объектом.");
                return false;
            }

            _logger.LogInformation("Добавление нового курьера: {CourierName}", courier.Name);
            try
            {
                courier.IsActive = true;
                courier.Created_At = DateTime.UtcNow;
                courier.Current_Lat = 0.0;
                courier.Current_Lon = 0.0;

                await _courierRepository.AddAsync(courier);
                _logger.LogInformation("Курьер {CourierName} успешно добавлен.", courier.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении курьера {CourierName}", courier.Name);
                throw;
            }
        }

        /// <summary>
        /// Назначение курьера на заказ
        /// </summary>
        /// <param name="courierId">ID курьера</param>
        /// <param name="orderId">ID заказа</param>
        /// <returns>Прошла ли операция назначения</returns>
        public async Task<bool> AssignCourierToOrderAsync(int courierId, int orderId)
        {
            _logger.LogInformation("Попытка назначения курьера {CourierId} на заказ {OrderId}", courierId, orderId);
            try
            {
                Courier? courier = await _courierRepository.GetById(courierId);
                if (courier == null)
                {
                    _logger.LogWarning("Курьер {CourierId} не найден.", courierId);
                    return false;
                }

                Order? order = await _orderRepository.GetById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Заказ {OrderId} не найден.", orderId);
                    return false;
                }

                if (order.CourierId != null)
                {
                    _logger.LogWarning("Заказ {OrderId} уже назначен курьеру {ExistingCourierId}", orderId, order.CourierId);
                    return false;
                }

                order.CourierId = courierId;
                order.Status = "В пути";
                courier.Current_Lat = order.Lat_From;
                courier.Current_Lon = order.Lon_From;
                await _orderRepository.UpdateAsync(order);

                _logger.LogInformation("Курьер {CourierId} успешно назначен на заказ {OrderId}", courierId, orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Исключение при назначении курьера {CourierId} на заказ {OrderId}", courierId, orderId);
                throw;
            }
        }

        /// <summary>
        /// Назначение курьера на заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Прошла ли операция назначения</returns>
        public async Task<bool> AssignFreeCourierToOrderAsync(Order order)
        {
            if (order == null)
            {
                _logger.LogWarning("AssignFreeCourierToOrderAsync вызван с null-order.");
                return false;
            }

            _logger.LogInformation("Автоматическое назначение свободного курьера на заказ {OrderId}", order.Id);
            try
            {
                var freeList = await _courierRepository.GetFreeCouriers();
                if (freeList.Count == 0)
                {
                    _logger.LogWarning("Нет свободных курьеров для заказа {OrderId}. Заказ остаётся в статусе 'Новый'.", order.Id);
                    order.CourierId = null;
                    order.Status = "Новый";
                    await _orderRepository.UpdateAsync(order);
                    return false;
                }

                var random = new Random();
                var courier = freeList[random.Next(freeList.Count)];
                if (order.CourierId != null)
                {
                    _logger.LogWarning("Заказ {OrderId} уже имеет назначенного курьера {CourierId}", order.Id, order.CourierId);
                    return false;
                }

                order.CourierId = courier.Id;
                order.Status = "В пути";
                courier.Current_Lat = order.Lat_From;
                courier.Current_Lon = order.Lon_From;
                await _orderRepository.UpdateAsync(order);

                _logger.LogInformation("Заказу {OrderId} автоматически назначен курьер {CourierId}", order.Id, courier.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при автоматическом назначении курьера на заказ {OrderId}", order?.Id);
                throw;
            }
        }

        /// <summary>
        /// Изменение статуса онлайн/офлайн курьера
        /// </summary>
        /// <param name="courierId">ID курьера</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> ToggleCourierOnlineAsync(int courierId)
        {
            _logger.LogInformation("Переключение онлайн-статуса курьера {CourierId}", courierId);
            try
            {
                var courier = await _courierRepository.GetById(courierId);
                if (courier == null)
                {
                    _logger.LogWarning("Курьер {CourierId} не найден для переключения статуса.", courierId);
                    return false;
                }

                await _courierRepository.ToggleOnline(courierId);
                _logger.LogInformation("Статус курьера {CourierId} успешно изменён.", courierId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при переключении онлайн-статуса курьера {CourierId}", courierId);
                throw;
            }
        }

        /// <summary>
        /// Удаление курьера
        /// </summary>
        /// <param name="courierId">ID курьера</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> RemoveCourierAsync(int courierId)
        {
            _logger.LogInformation("Удаление курьера {CourierId}", courierId);
            try
            {
                var courier = await _courierRepository.GetById(courierId);
                if (courier == null)
                {
                    _logger.LogWarning("Курьер {CourierId} не найден для удаления.", courierId);
                    return false;
                }

                await _courierRepository.DeleteAsync(courier);
                _logger.LogInformation("Курьер {CourierId} успешно удалён.", courierId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении курьера {CourierId}", courierId);
                throw;
            }
        }

        /// <summary>
        /// Получение курьера по айди
        /// </summary>
        /// <param name="id">ID курьера</param>
        /// <returns>Объект курьера</returns>
        public async Task<Courier?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Получение курьера по ID {CourierId}", id);
            try
            {
                return await _courierRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении курьера {CourierId}", id);
                throw;
            }
        }

        /// <summary>
        /// Обновление курьера
        /// </summary>
        /// <param name="courier">Объект курьер</param>
        public async Task UpdateAsync(Courier courier)
        {
            if (courier == null)
            {
                _logger.LogWarning("Попытка изменить курьера с null-объектом.");
                return;
            }

            _logger.LogDebug("Обновление данных курьера {CourierId}", courier?.Id);
            try
            {
                await _courierRepository.UpdateAsync(courier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении курьера {CourierId}", courier?.Id);
                throw;
            }
        }

        /// <summary>
        /// Функция, сохраняющая координаты курьеры
        /// </summary>
        /// <param name="Lat">Широта</param>
        /// <param name="Lat">Долгота</param>
        /// <param name="courier">Объект курьера</param>
        public async Task SaveCourierCoordsAsync(double Lat, double Lon, Courier courier)
        {
            if (courier == null)
            {
                _logger.LogWarning("Попытка изменить курьера с null-объектом.");
                return;
            }

            _logger.LogDebug("Сохранение координат курьера {CourierId}: lat={Lat}, lon={Lon}", courier?.Id, Lat, Lon);
            try
            {
                courier.Current_Lat = Lat;
                courier.Current_Lon = Lon;
                await UpdateAsync(courier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении координат курьера {CourierId}", courier?.Id);
                throw;
            }
        }
    }
}