using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<RoutePoint> RoutePoints { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Basket> Baskets { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Категории
            modelBuilder.Entity<Categories>().HasData(
                new Categories { Id = 1, Name = "Все" },
                new Categories { Id = 2, Name = "Напитки" },
                new Categories { Id = 3, Name = "Десерты" },
                new Categories { Id = 4, Name = "Горячее" }
            );

            // Еда (CategoriesId указывает на Id выше)
            modelBuilder.Entity<Food>().HasData(
                new Food
                {
                    Id = 1,
                    Title = "Кофе",
                    Description = "Ароматный американо",
                    ImageUrl = "pack://application:,,,/Images/coffee.png",
                    Weight = 200,
                    CategoriesId = 2,
                    Price = 150.00m
                },
                new Food
                {
                    Id = 2,
                    Title = "Чизкейк",
                    Description = "Нежный сливочный чизкейк",
                    ImageUrl = "pack://application:,,,/Images/cheesecake.png",
                    Weight = 150,
                    CategoriesId = 3,
                    Price = 350.00m
                },
                new Food
                {
                    Id = 3,
                    Title = "Круассан",
                    Description = "Свежий слоёный круассан",
                    ImageUrl = "pack://application:,,,/Images/croissant.png",
                    Weight = 100,
                    CategoriesId = 3,
                    Price = 180.00m
                },
                new Food
                {
                    Id = 4,
                    Title = "Плов",
                    Description = "Узбекский плов с бараниной",
                    ImageUrl = "pack://application:,,,/Images/plov.png",
                    Weight = 400,
                    CategoriesId = 4,
                    Price = 450.00m
                },
                new Food
                {
                    Id = 5,
                    Title = "Борщ",
                    Description = "Борщ со сметаной",
                    ImageUrl = "https://images.unsplash.com/photo-1547592180-85f173990554?w=400",
                    Weight = 400,
                    CategoriesId = 4,
                    Price = 320.00m
                },
                new Food
                {
                    Id = 6,
                    Title = "Латте",
                    Description = "Кофе латте с молоком",
                    ImageUrl = "https://images.unsplash.com/photo-1561047029-3000c68339ca?w=400",
                    Weight = 250,
                    CategoriesId = 2,
                    Price = 200.00m
                }
            );

            // Admin пользователь
            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    Id = 1,
                    Name = "admin",
                    Phone = "00000000000",
                    Email = "admin@delivery.ru",
                    Password = "admin",
                    Role = "admin",
                    Created_At = DateTime.UtcNow // Эквивалент NOW() в Postgres
                }
            );
        }
    }
}
