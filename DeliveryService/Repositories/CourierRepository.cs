using DeliveryService.Data;
using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Repositories
{
    /// <summary>
    /// Репозиторий для доступа к Курьерам в базе данных
    /// </summary>
    public class CourierRepository
    {
        private readonly AppDbContext _context;


        public CourierRepository(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Получение курьера по id
        /// </summary>
        /// <param name="courierId">ID курьера</param>
        /// <returns>Курьер</returns>
        public async Task<Courier?> GetByIdAsync(int courierId) => await _context.Couriers.FindAsync(courierId);

        /// <summary>
        /// Получение всех курьеров
        /// </summary>
        /// <returns>Список курьеров</returns>
        public async Task<List<Courier>> GetAllAsync()
        {
            return await _context.Couriers
                .Include(o => o.Orders)
                .ToListAsync();
        }

        /// <summary>
        /// Получение всех активных курьеров
        /// </summary>
        /// <returns>Список курьеров, у которых IsActive равен True</returns>
        public async Task<List<Courier>> GetActiveAsync()
        {
            return await _context.Couriers
                .Where(c => c.IsActive)
                .Include(o => o.Orders)
                .ToListAsync();
        }

        /// <summary>
        /// Получение всех свободных от заказов курьеров
        /// </summary>
        /// <returns>Список курьеров, у которых нет активных заказов</returns>
        public async Task<List<Courier>> GetFreeCouriersAsync()
        {
            return await _context.Couriers
                .Where(c => c.IsActive)
                .Where(c => !c.Orders.Any(o => o.Status == "В пути")) // Изменить название на нужные
                .ToListAsync();
        }

        /// <summary>
        /// Добавление курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        public async Task AddAsync(Courier courier)
        {
            await _context.Couriers.AddAsync(courier);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновление курьера в базе данных
        /// </summary>
        /// <param name="courier">Курьер</param>
        public async Task UpdateAsync(Courier courier)
        {
            _context.Couriers.Update(courier);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Переключение статуса IsActive 
        /// </summary>
        /// <param name="courierId">ID курьера</param>
        public async Task ToggleOnlineAsync(int courierId)
        {
            var courier = await _context.Couriers.FindAsync(courierId);

            if (courier != null)
            {
                courier.IsActive = !courier.IsActive;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Удаление курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        public async Task DeleteAsync(Courier courier)
        {
            _context.Couriers.Remove(courier);
            await _context.SaveChangesAsync();
        }
    }
}