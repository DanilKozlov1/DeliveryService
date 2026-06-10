using DeliveryService.Data;
using DeliveryService.Repositories;
using DeliveryService.Services;
using DeliveryService.Services.Interfaces;
using DeliveryService.Utils;
using DeliveryService.ViewModels;
using DeliveryService.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace DeliveryService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();


            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(config);

            // Регистрация Serilog
            SerilogConfigurator.ConfigureLogging(services, config);

            Log.Information("Запуск приложения DeliveryService...");

            // БД
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("Default")));


            // Репозитории
            services.AddScoped<OrderRepository>();
            services.AddScoped<CourierRepository>();
            services.AddScoped<ClientRepository>();
            services.AddScoped<FoodCategoryRepository>();
            services.AddScoped<FoodRepository>();
            services.AddScoped<BasketRepository>();

            // Сервисы
            services.AddSingleton<SessionService>();
            services.AddSingleton<WindowsService>();
            services.AddSingleton<IConfigService, ConfigService>();

            services.AddScoped<SimulationService>();
            services.AddScoped<OrderService>();
            services.AddScoped<CourierService>();
            services.AddScoped<ClientService>();
            services.AddScoped<FoodCategoryService>();
            services.AddScoped<FoodService>();
            services.AddScoped<BasketService>();

            // ViewModels
            services.AddTransient<MainWindowModel>();
            services.AddTransient<ListCouriersViewModel>();
            services.AddTransient<OrderListViewModel>();
            services.AddTransient<NewOrderViewModel>();
            services.AddTransient<RegistrationCourierModel>();
            services.AddTransient<DispatcherViewModel>();
            services.AddTransient<EntranceViewModel>();
            services.AddTransient<RegistrationViewModel>();
            services.AddTransient<MenuViewModel>();
            services.AddTransient<OrderAcceptViewModel>();
            services.AddTransient<AuthorizationViewModel>();

            // View
            services.AddTransient<MainWindow>();
            services.AddTransient<NewOrderView>();
            services.AddTransient<RegistrationCourier>();
            services.AddTransient<EntranceView>();
            services.AddTransient<MenuView>();
            services.AddTransient<OrderAcceptView>();

            //контейнер
            Services = services.BuildServiceProvider();

            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (db.Database.CanConnect())
                    Log.Information("Успешное подключение к СУБД PostgreSQL.");
                else
                {
                    Log.Fatal("КРИТИЧЕСКАЯ ОШИБКА: База данных PostgreSQL недоступна или строка подключения неверна.");
                    MessageBox.Show("Ошибка запуска. Проверьте логи.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    Shutdown();
                    return;
                }

                db.Database.Migrate();

                var startupScope = Services.CreateScope();
                var win = startupScope.ServiceProvider.GetRequiredService<EntranceView>();
                win.Closed += (_, _) => startupScope.Dispose();
                win.Show();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Критическая ошибка при инициализации базы данных.");
                MessageBox.Show("Ошибка запуска. Проверьте логи.", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Приложение завершает свою работу.");
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}