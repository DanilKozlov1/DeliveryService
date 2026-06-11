using DeliveryService.Models;
using DeliveryService.Repositories;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, работающий с Корзиной
    /// </summary>
    public class BasketService
    {
        private readonly ILogger<BasketService> _logger;

        private readonly BasketRepository _basketRepository;
        private readonly OrderRepository _orderRepository;
        private readonly FoodRepository _foodRepository;


        public BasketService(ILogger<BasketService> logger,
            BasketRepository basketRepository, OrderRepository orderRepository, FoodRepository foodRepository)
        {
            _logger = logger;
            _basketRepository = basketRepository;
            _orderRepository = orderRepository;
            _foodRepository = foodRepository;
        }


        /// <summary>
        /// Получение объекта корзины по id
        /// </summary>
        /// <param name="basketId">ID объекта корзины</param>
        /// <returns>Объект корзины</returns>
        public async Task<Basket?> GetByIdAsync(int basketId)
        {
            _logger.LogDebug("Запрос позиции корзины по ID {BasketId}", basketId);
            try
            {
                return await _basketRepository.GetByIdAsync(basketId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении позиции корзины {BasketId}", basketId);
                throw;
            }
        }

        /// <summary>
        /// Полуение списка корзины и полной суммы по пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список корзины и полная сумма</returns>
        public async Task<(List<Basket> userBasket, decimal totalPrice)> GetUserBasketAsync(int userId)
        {
            _logger.LogDebug("Запрос корзины пользователя {UserId} (включая оформленные)", userId);
            try
            {
                var basket = await _basketRepository.GetUserBasketAsync(userId);
                decimal totalPrice = basket.Sum(b => b.Price);

                _logger.LogDebug(
                    "Корзина пользователя {UserId}: {Count} позиций, сумма {TotalPrice}", 
                    userId, basket.Count, totalPrice
                );
                return (basket, totalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении корзины пользователя {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Полуение списка корзины, исключая те объекты, которые уже привязаны к заказам и полной суммы по пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список объектов корзины, c не привязанными к заказам и полная сумма</returns>
        public async Task<(List<Basket> userBasket, decimal totalPrice)> GetUserActiveBasketAsync(int userId)
        {
            _logger.LogDebug("Запрос активной корзины пользователя {UserId} (не оформленные в заказ)", userId);
            try
            {
                var basket = await _basketRepository.GetUserActiveBasketAsync(userId);
                decimal totalPrice = basket.Sum(b => b.Price);

                _logger.LogDebug(
                    "Активная корзина пользователя {UserId}: {Count} позиций, сумма {TotalPrice}", 
                    userId, basket.Count, totalPrice
                );
                return (basket, totalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активной корзины пользователя {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Создание нового объекта корзины
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="foodId">ID еды</param>
        /// <param name="quantity">Количество</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> AddNewBasketItemAsync(int userId, int foodId, int quantity)
        {
            _logger.LogInformation(
                "Добавление новой позиции в корзину: userId={UserId}, foodId={FoodId}, quantity={Quantity}", 
                userId, foodId, quantity
            );
            try
            {
                var food = await _foodRepository.GetByIdAsync(foodId);
                if (food == null)
                {
                    _logger.LogWarning("Блюдо {FoodId} не найдено при добавлении в корзину пользователя {UserId}", foodId, userId);
                    return false;
                }

                decimal price = food.Price * quantity;
                var item = new Basket
                {
                    UserId = userId,
                    FoodId = foodId,
                    Quantity = quantity,
                    Price = price
                };

                await _basketRepository.AddAsync(item);

                _logger.LogInformation(
                    "Позиция корзины добавлена: BasketId={BasketId}, userId={UserId}, foodId={FoodId}", 
                    item.Id, userId, foodId
                );
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении новой позиции в корзину (userId={UserId}, foodId={FoodId})", 
                    userId, foodId);
                throw;
            }
        }

        /// <summary>
        /// Создание нового объекта корзины или обновление уже существующего
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="foodId">ID еды</param>
        /// <param name="quantity">Количество</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> AddOrUpdateBasketItemAsync(int userId, int foodId, int quantity)
        {
            _logger.LogInformation(
                "Добавление или обновление позиции в корзине: userId={UserId}, foodId={FoodId}, quantity={Quantity}",
                userId, foodId, quantity
            );
            try
            {
                var food = await _foodRepository.GetByIdAsync(foodId);
                if (food == null)
                {
                    _logger.LogWarning("Блюдо {FoodId} не найдено при обновлении корзины пользователя {UserId}", foodId, userId);
                    return false;
                }

                var existingItem = await _basketRepository.GetActiveByUserAndFoodIdAsync(userId, foodId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.Price = food.Price * existingItem.Quantity;

                    await _basketRepository.UpdateAsync(existingItem);
                    _logger.LogInformation(
                        "Позиция корзины обновлена (увеличено количество): BasketId={BasketId}, новое quantity={Quantity}, " +
                        "новая цена={Price}", 
                        existingItem.Id, existingItem.Quantity, existingItem.Price
                    );
                }
                else
                {
                    decimal price = food.Price * quantity;
                    var newItem = new Basket
                    {
                        UserId = userId,
                        FoodId = foodId,
                        Quantity = quantity,
                        Price = price
                    };
                    await _basketRepository.AddAsync(newItem);
                    _logger.LogInformation(
                        "Создана новая позиция корзины: BasketId={BasketId}, userId={UserId}, foodId={FoodId}", 
                        newItem.Id, userId, foodId
                    );
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении/добавлении позиции в корзину (userId={UserId}, foodId={FoodId})", 
                    userId, foodId);
                throw;
            }
        }

        /// <summary>
        /// Удаление объекта из корзины
        /// </summary>
        /// <param name="basketId">ID объекта корзины</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> RemoveItemAsync(int basketId)
        {
            _logger.LogInformation("Удаление позиции корзины: BasketId={BasketId}", basketId);
            try
            {
                var item = await _basketRepository.GetByIdAsync(basketId);
                if (item == null)
                {
                    _logger.LogWarning("Позиция корзины {BasketId} не найдена для удаления", basketId);
                    return false;
                }

                await _basketRepository.DeleteAsync(item);
                _logger.LogInformation("Позиция корзины {BasketId} успешно удалена", basketId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении позиции корзины {BasketId}", basketId);
                throw;
            }
        }

        /// <summary>
        /// Очистка всей всех объектов корзины по пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        public async Task ClearUserBasketAsync(int userId)
        {
            _logger.LogInformation("Очистка всей корзины пользователя {UserId}", userId);
            try
            {
                await _basketRepository.ClearUserBasketAsync(userId);
                _logger.LogInformation("Корзина пользователя {UserId} полностью очищена", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при очистке корзины пользователя {UserId}", userId);
                throw;
            }
        }
    }
}