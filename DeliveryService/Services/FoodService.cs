using DeliveryService.Models;
using DeliveryService.Repositories;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, работающий с Едой
    /// </summary>
    public class FoodService
    {
        private readonly ILogger<FoodService> _logger;

        private readonly FoodRepository _foodRepository;


        public FoodService(ILogger<FoodService> logger, FoodRepository foodRepository)
        {
            _logger = logger;
            _foodRepository = foodRepository;
        }


        /// <summary>
        /// Получение объекта еды по id
        /// </summary>
        /// <param name="foodId">ID еды</param>
        /// <returns>Объект еды</returns>
        public async Task<Food?> GetByIdAsync(int foodId)
        {
            _logger.LogDebug("Запрос блюда по ID {FoodId}", foodId);
            try
            {
                return await _foodRepository.GetByIdAsync(foodId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении блюда {FoodId}", foodId);
                throw;
            }
        }

        /// <summary>
        /// Получение всей еды
        /// </summary>
        /// <returns>Список всеё еды</returns>
        public async Task<List<Food>> GetAllAsync()
        {
            _logger.LogDebug("Запрос всех блюд");
            try
            {
                return await _foodRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка всех блюд");
                throw;
            }
        }

        /// <summary>
        /// Получение всей еды по категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Список объектов еды с указанной категорией. Если таких нет - пустой список</returns>
        public async Task<List<Food>> GetAllFromCategoryAsync(int categoryId)
        {
            _logger.LogDebug("Запрос блюд по категории {CategoryId}", categoryId);
            try
            {
                return await _foodRepository.GetAllFromCategoryAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении блюд категории {CategoryId}", categoryId);
                throw;
            }
        }

        /// <summary>
        /// Добавление еды в базы данных
        /// </summary>
        /// <param name="food">Объект еды</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> AddAsync(Food food)
        {
            if (food == null)
            {
                _logger.LogWarning("Попытка добавить блюдо с null-объектом");
                return false;
            }

            _logger.LogInformation("Добавление нового блюда: {FoodName}", food.Title);
            try
            {
                await _foodRepository.AddAsync(food);
                _logger.LogInformation("Блюдо {FoodName} успешно добавлено", food.Title);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении блюда {FoodName}", food.Title);
                throw;
            }
        }
    }
}