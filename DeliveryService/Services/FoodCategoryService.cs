using DeliveryService.Models;
using DeliveryService.Repositories;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, работающий с Категориями еды
    /// </summary>
    public class FoodCategoryService
    {
        private readonly ILogger<FoodCategoryService> _logger;

        private readonly FoodCategoryRepository _foodCategoryRepository;


        public FoodCategoryService(ILogger<FoodCategoryService> logger, FoodCategoryRepository foodCategoryRepository)
        {
            _logger = logger;
            _foodCategoryRepository = foodCategoryRepository;
        }


        /// <summary>
        /// Получение категории еды по id
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Категория еды</returns>
        public async Task<Categories?> GetByIdAsync(int categoryId)
        {
            _logger.LogDebug("Запрос категории по ID {CategoryId}", categoryId);
            try
            {
                return await _foodCategoryRepository.GetByIdAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении категории {CategoryId}", categoryId);
                throw;
            }
        }

        /// <summary>
        /// Получение всех категорий еды
        /// </summary>
        /// <returns>Список категорий еды</returns>
        public async Task<List<Categories>> GetAllAsync()
        {
            _logger.LogDebug("Запрос всех категорий");
            try
            {
                return await _foodCategoryRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка всех категорий");
                throw;
            }
        }

        /// <summary>
        /// Добавление категории еды в базу данных
        /// </summary>
        /// <param name="categories">Категория еды</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> AddAsync(Categories categories)
        {
            if (categories == null)
            {
                _logger.LogWarning("Попытка добавить категорию с null-объектом");
                return false;
            }

            _logger.LogInformation("Добавление новой категории: {CategoryName}", categories.Name);
            try
            {
                await _foodCategoryRepository.AddAsync(categories);
                _logger.LogInformation("Категория {CategoryName} успешно добавлена", categories.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении категории {CategoryName}", categories.Name);
                throw;
            }
        }
    }
}