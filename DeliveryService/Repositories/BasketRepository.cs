using DeliveryService.Data;
using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Repositories
{
    /// <summary>
    /// Репозиторий для доступа к корзине в базе данных
    /// </summary>
    public class BasketRepository
    {
        private readonly AppDbContext _context;


        public BasketRepository(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Получение объекта корзины по id
        /// </summary>
        /// <param name="basketId">ID объекта корзины</param>
        /// <returns>Объект корзины с указаным id</returns>
        public async Task<Basket?> GetByIdAsync(int basketId)
        {
            return await _context.Baskets
                .Include(b => b.Food)
                .FirstOrDefaultAsync(b => b.Id == basketId);
        }

        /// <summary>
        /// Получение объекта корзины по id`шникам пользователя и еды 
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="foodId">ID еды</param>
        /// <returns>Объект корзины с указаными id`ками</returns>
        public async Task<Basket?> GetByUserAndFoodIdAsync(int userId, int foodId)
        {
            return await _context.Baskets
                .Include(b => b.Food)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.FoodId == foodId);
        }

        /// <summary>
        /// Получение объекта корзины, который не привязан к заказу и по id`шникам пользователя и еды
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="foodId">ID еды</param>
        /// <returns>Объект корзины не привязанный к заказу и с указаными id`ками</returns>
        public async Task<Basket?> GetActiveByUserAndFoodIdAsync(int userId, int foodId)
        {
            return await _context.Baskets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.FoodId == foodId && !_context.Orders.Any(o => o.BasketId == b.Id));
        }

        /// <summary>
        /// Получение всех объектов корзины по пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список объектов корзины с указанным пользователем</returns>
        public async Task<List<Basket>> GetUserBasketAsync(int userId)
        {
            return await _context.Baskets
                .Include(b => b.Food)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Получение всех объектов корзины по пользователю, исключая те объекты, которые уже привязаны к заказам
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список объектов корзины, не привязанные к заказам, с указанным пользователем</returns>
        public async Task<List<Basket>> GetUserActiveBasketAsync(int userId)
        {
            return await _context.Baskets
                .Include(b => b.Food)
                .Where(b => b.UserId == userId && !_context.Orders.Any(o => o.BasketId == b.Id))
                .ToListAsync();
        }

        /// <summary>
        /// Добавление объекта корзины
        /// </summary>
        /// <param name="basket">Объект корзины</param>
        public async Task AddAsync(Basket basket)
        {
            await _context.Baskets.AddAsync(basket);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновление объекта корзины в базе данных
        /// </summary>
        /// <param name="basket">Объект корзины</param>
        public async Task UpdateAsync(Basket basket)
        {
            _context.Baskets.Update(basket);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаление объекта корзины
        /// </summary>
        /// <param name="basket">Объект корзины</param>
        public async Task DeleteAsync(Basket basket)
        {
            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаление объектов корзины по id пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        public async Task ClearUserBasketAsync(int userId)
        {
            var basket = _context.Baskets.Where(b => b.UserId == userId).ToList();
            _context.Baskets.RemoveRange(basket);
            await _context.SaveChangesAsync();
        }
    }
}