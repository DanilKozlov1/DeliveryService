using DeliveryService.Models;
using DeliveryService.Repositories;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, работающий с заказами
    /// </summary>
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;

        private readonly OrderRepository _orderRepository;
        private readonly ClientRepository _clientRepository;


        public OrderService(ILogger<OrderService> logger, OrderRepository orderRepo, ClientRepository clientRepo)
        {
            _logger = logger;
            _orderRepository = orderRepo;
            _clientRepository = clientRepo;
        }
        

        /// <summary>
        /// Получение всех заказов
        /// </summary>
        /// <returns>Возвращает список всех заказов</returns>
        public async Task<List<Order>> GetAllAsync()
        {
            _logger.LogDebug("Запрос всех заказов");
            try
            {
                return await _orderRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка всех заказов");
                throw;
            }
        }

        /// <summary>
        /// Получение всех активных заказов
        /// </summary>
        /// <returns>Список активных заказов</returns>
        public async Task<List<Order>> GetActiveOrdersAsync()
        {
            _logger.LogDebug("Запрос активных заказов");
            try
            {
                return await _orderRepository.GetActive();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активных заказов");
                throw;
            }
        }

        /// <summary>
        /// Получение заказа по id
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Заказ</returns>
        public async Task<Order?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Запрос заказа по ID {OrderId}", id);
            try
            {
                return await _orderRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении заказа {OrderId}", id);
                throw;
            }
        }

        /// <summary>
        /// Создание заказа
        /// </summary>
        /// <param name="client">Объект клиента</param>
        /// <param name="order">Объект заказа</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> CreateOrderAsync(Client client, Order order)
        {
            if (client == null)
            {
                _logger.LogWarning("Попытка передачи клиента с null-объектом.");
                return false;
            }
            if (string.IsNullOrEmpty(order.Address_From) || string.IsNullOrEmpty(order.Address_To))
            {
                _logger.LogWarning("Адрес отправления или назначения не заполнен. ClientId {ClientId}", client.Id);
                return false;
            }

            _logger.LogInformation("Создание нового заказа для клиента {ClientId}", client.Id);
            try
            {
                if (client.Id == 0)
                {
                    await _clientRepository.AddAsync(client);
                    _logger.LogDebug("Новый клиент {ClientId} добавлен", client.Id);
                }

                order.ClientId = client.Id;
                order.Status = "Новый";
                order.Created_At = DateTime.UtcNow;

                await _orderRepository.AddAsync(order);
                _logger.LogInformation("Заказ {OrderId} успешно создан для клиента {ClientId}", order.Id, client.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании заказа для клиента {ClientId}", client?.Id);
                throw;
            }
        }

        /// <summary>
        /// Меняет статус заказа
        /// </summary>
        /// <param name="orderId">Айди заказа</param>
        /// <param name="newStatus">Новый статус</param>
        /// <param name="feedback">Отзыв, при закрытии заказа</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> ChangeStatusAsync(int orderId, string newStatus, string? feedback = null)
        {
            _logger.LogInformation("Изменение статуса заказа {OrderId} на '{NewStatus}'", orderId, newStatus);
            try
            {
                var order = await _orderRepository.GetById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Заказ {OrderId} не найден при попытке изменить статус", orderId);
                    return false;
                }

                order.Status = newStatus;
                await _orderRepository.UpdateAsync(order);

                await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
                {
                    OrderId = orderId,
                    Status = newStatus,
                    Changed_At = DateTime.UtcNow,
                    FeedBack = feedback
                });

                _logger.LogInformation("Статус заказа {OrderId} изменён на '{NewStatus}'", orderId, newStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при изменении статуса заказа {OrderId}", orderId);
                throw;
            }
        }

        /// <summary>
        /// Отменяет заказ
        /// </summary>
        /// <param name="orderId">Айди заказа</param>
        /// <param name="feedback">Отзыв, при желании</param>
        /// <returns>true-если заказ отменен, false-если заказ не найден</returns>
        public async Task<bool?> CancelOrderAsync(int orderId, string? feedback = null)
        {
            _logger.LogInformation("Отмена заказа {OrderId}", orderId);
            try
            {
                var order = await _orderRepository.GetById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Заказ {OrderId} не найден при попытке отмены", orderId);
                    return false;
                }

                order.Status = "Отменён";
                await _orderRepository.UpdateAsync(order);
                await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
                {
                    OrderId = orderId,
                    Status = "Отменён",
                    Changed_At = DateTime.UtcNow,
                    FeedBack = feedback
                });

                _logger.LogInformation("Заказ {OrderId} успешно отменён", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отмене заказа {OrderId}", orderId);
                throw;
            }
        }

        /// <summary>
        /// Получеие заказа по айди курьера
        /// </summary>
        /// <param name="courierId">айди курьера</param>
        /// <returns></returns>
        public async Task<Order?> FindOrderByCourierIdAsync(int courierId)
        {
            _logger.LogDebug("Поиск заказа по курьеру {CourierId}", courierId);
            try
            {
                return await _orderRepository.GetByCourierId(courierId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске заказа по курьеру {CourierId}", courierId);
                throw;
            }
        }

        /// <summary>
        /// Удаление заказа
        /// </summary>
        /// <param name="orderId">Айди заказа</param>
        /// <returns></returns>
        public async Task<bool> RemoveOrderAsync(int orderId)
        {
            _logger.LogInformation("Удаление заказа {OrderId}", orderId);
            try
            {
                var order = await _orderRepository.GetById(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Заказ {OrderId} не найден для удаления", orderId);
                    return false;
                }

                await _orderRepository.DeleteAsync(order);
                _logger.LogInformation("Заказ {OrderId} успешно удалён", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении заказа {OrderId}", orderId);
                throw;
            }
        }

        /// <summary>
        /// Обновление данных в заказе
        /// </summary>
        /// <param name="order">Объект заказа</param>
        /// <returns></returns>
        public async Task UpdateAsync(Order order) 
        {
            if (order == null)
            {
                _logger.LogWarning("Попытка передачи заказа с null-объектом.");
                return;
            }

            _logger.LogDebug("Обновление заказа {OrderId}", order?.Id);
            try
            {
                await _orderRepository.UpdateAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении заказа {OrderId}", order?.Id);
                throw;
            }
        }

        /// <summary>
        /// Удаление заказа
        /// </summary>
        /// <param name="order">Объект заказа</param>
        /// <returns>true-удачно, false-неудачно</returns>
        public async Task<bool> DeleteAsync(Order order)
        {
            if (order == null)
            {
                _logger.LogWarning("Попытка передачи заказа с null-объектом.");
                return false;
            }

            _logger.LogInformation("Удаление заказа {OrderId}", order.Id);
            try
            {
                await _orderRepository.DeleteAsync(order);
                _logger.LogInformation("Заказ {OrderId} успешно удалён", order.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении заказа {OrderId}", order.Id);
                throw;
            }
        }

        /// <summary>
        /// Добавление в историю
        /// </summary>
        /// <param name="order">Объект заказа</param>
        /// <param name="feedback">Отзыв</param>
        /// <param name="status">Новый статус</param>
        /// <returns></returns>
        public async Task AddToHistory(Order order, string status, string? feedback = null)
        {
            if (order == null)
            {
                _logger.LogWarning("Попытка передачи заказа с null-объектом.");
                return;
            }

            _logger.LogDebug("Добавление записи в историю заказа {OrderId}, статус {Status}", order.Id, status);
            try
            {
                await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
                {
                    Id = order.Id,
                    Order = order,
                    Changed_At = DateTime.UtcNow,
                    FeedBack = feedback,
                    Status = status,
                    OrderId = order.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении в историю заказа {OrderId}", order.Id);
                throw;
            }
        }
    }
}
