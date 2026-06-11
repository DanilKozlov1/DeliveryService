using DeliveryService.Data;
using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Repositories
{
    /// <summary>
    /// Репозиторий для доступа к Заказам в базе данных
    /// </summary>
    public class OrderRepository
    {
        private readonly AppDbContext _context;


        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Получение заказа по id
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns>Заказ</returns>
        /// 
        public async Task<Order?> GetByIdAsync(int orderId) => await _context.Orders.FindAsync(orderId);

        /// <summary>
        /// Получение заказа по айди курьера
        /// </summary>
        /// <param name="courierId">айди курьера</param>
        /// <returns></returns>
        public async Task<Order?> GetByCourierIdAsync(int courierId) => await _context.Orders.FirstOrDefaultAsync(x=>x.CourierId == courierId);

        /// <summary>
        /// Получение всех заказов
        /// </summary>
        /// <returns>Список заказов</returns>
        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Courier)
                .Include(o => o.RoutePoints)
                .Include(o => o.StatusHistory)
                .ToListAsync();
        }

        /// <summary>
        /// Получение всех незавершённых заказов
        /// </summary>
        /// <returns>Список незавершённых заказов</returns>
        public async Task<List<Order>> GetActive()
        {
            return await _context.Orders
                .Where(o => o.Status != "Done") // Изменить на нужный статус или добавить ещё условия
                .Include(o => o.Client)
                .Include(o => o.Courier)
                .Include(o => o.RoutePoints)
                .Include(o => o.StatusHistory)
                .ToListAsync();
        }

        /// <summary>
        /// Добавление заказа
        /// </summary>
        /// <param name="order">Заказ</param>
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновление заказа в базе данных
        /// </summary>
        /// <param name="order">Заказ</param>
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаление заказа
        /// </summary>
        /// <param name="order">Заказ</param>
        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Добавление в историю изменения статусов заказов
        /// </summary>
        /// <param name="history">Объект класса OrderStatusHistory</param>
        /// <returns></returns>
        public async Task AddStatusHistoryAsync(OrderStatusHistory history)
        {
            await _context.OrderStatusHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }
    }
}