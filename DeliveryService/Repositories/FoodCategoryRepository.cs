using DeliveryService.Data;
using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Repositories
{
    /// <summary>
    /// Репозиторий для доступа к категориям еды в базе данных
    /// </summary>
    public class FoodCategoryRepository
    {
        private readonly AppDbContext _context;


        public FoodCategoryRepository(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Получение категории еды по id
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Категория еды с указаным id</returns>
        public async Task<Categories?> GetByIdAsync(int categoryId) => await _context.Categories.FindAsync(categoryId);

        /// <summary>
        /// Получение всех категорий еды
        /// </summary>
        /// <returns>Список категорий еды</returns>
        public async Task<List<Categories>> GetAllAsync() => await _context.Categories.ToListAsync();

        /// <summary>
        /// Добавление категории еды
        /// </summary>
        /// <param name="categories">Категория</param>
        public async Task AddAsync(Categories categories)
        {
            await _context.Categories.AddAsync(categories);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновление категории еды в базе данных
        /// </summary>
        /// <param name="categories">Категория</param>
        /// <returns></returns>
        public async Task UpdateAsync(Categories categories)
        {
            _context.Update(categories);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаление категории еды
        /// </summary>
        /// <param name="category">Категория</param>
        public async Task DeleteAsync(Categories category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}