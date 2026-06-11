using DeliveryService.Data;
using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Repositories
{
    /// <summary>
    /// Репозиторий для доступа к еде в базе данных
    /// </summary>
    public class FoodRepository
    {
        private readonly AppDbContext _context;


        public FoodRepository(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Получение еды по id
        /// </summary>
        /// <param name="foodId">ID еды</param>
        /// <returns>Еда с указаным id</returns>
        public async Task<Food?> GetByIdAsync(int foodId) => await _context.Foods.FindAsync(foodId);

        /// <summary>
        /// Получение всей еды
        /// </summary>
        /// <returns>Список еды</returns>
        public async Task<List<Food>> GetAllAsync()
        {
            return await _context.Foods
                .Include(f => f.Categories)
                .ToListAsync();
        }

        /// <summary>
        /// Получение еды по категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Список еды с указанной категорией</returns>
        public async Task<List<Food>> GetAllFromCategoryAsync(int categoryId)
        {
            return await _context.Foods
                .Include(f => f.Categories)
                .Where(f => f.CategoriesId == categoryId)
                .ToListAsync();
        }

        /// <summary>
        /// Добавление еды
        /// </summary>
        /// <param name="food">Объект еды</param>
        public async Task AddAsync(Food food)
        {
            await _context.Foods.AddAsync(food);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновление еды в базе данных
        /// </summary>
        /// <param name="food">Объект еды</param>
        public async Task UpdateAsync(Food food)
        {
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаление еды
        /// </summary>
        /// <param name="food">Объект еды</param>
        public async Task DeleteAsync(Food food)
        {
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
        }
    }
}