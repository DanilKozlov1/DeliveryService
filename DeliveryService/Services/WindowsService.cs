using DeliveryService.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, открывающий окна
    /// </summary>
    public class WindowsService
    {
        private readonly ILogger<WindowsService> _logger;

        private readonly IServiceProvider _services;
        private readonly SessionService _sessionService;

        /// <summary>
        /// Словарь открытых, не модальных, окон
        /// </summary>
        private readonly Dictionary<Type, Window> _openedWindows;


        public WindowsService(ILogger<WindowsService> logger, IServiceProvider services, SessionService sessionService)
        {
            _logger = logger;

            _services = services;
            _sessionService = sessionService;

            _openedWindows = new Dictionary<Type, Window>();
        }


        /// <summary>
        /// Открытие окна
        /// </summary>
        /// <typeparam name="TView">Класс открываемого окна</typeparam>
        private void OpenWindow<TView>() where TView : Window
        {
            var type = typeof(TView);

            if (_openedWindows.TryGetValue(type, out var window) && window.IsVisible)
            {
                _logger.LogInformation("Окно {WindowName} уже открыто. Перевод фокуса на него (Активация).", type.Name);
                window.Activate();
                return;
            }

            _logger.LogInformation("Инициализация и открытие немодального окна: {WindowName}.", type.Name);

            var win = _services.GetRequiredService<TView>();
            win.Closed += (s, e) =>
            {
                _logger.LogInformation("Немодальное окно {WindowName} было закрыто пользователем.", type.Name);
                _openedWindows.Remove(type);
            };

            win.Show();
            _openedWindows[type] = win;
        }
        /// <summary>
        /// Открытие окна как модальное
        /// </summary>
        /// <typeparam name="TView">Класс открываемого окна</typeparam>
        /// <returns>Результат работы окна - DialogResult</returns>
        private bool? OpenModalWindow<TView>() where TView : Window
        {
            var type = typeof(TView);
            _logger.LogInformation(
                "Открытие модального диалогового окна: {WindowName}. Основной интерфейс заблокирован.", 
                type.Name
            );

            var win = _services.GetRequiredService<TView>();
            
            var result = win.ShowDialog();
            _logger.LogInformation(
                "Модальное окно {WindowName} закрыто. DialogResult: {Result}", 
                type.Name, result
            );

            return result;
        }
        /// <summary>
        /// Открывает окно входа
        /// </summary>
        public void OpenEntrance() => OpenWindow<EntranceView>();
        /// <summary>
        /// Открывает окно, которое появляется после совершения заказа для дальнейшего отслеживания его
        /// </summary>
        public void OpenOrderAccept() => OpenWindow<OrderAcceptView>();
        /// <summary>
        /// Открывает меню с едой
        /// </summary>
        public void OpenMenu() => OpenWindow<MenuView>();
        /// <summary>
        /// Открытие NewOrderView
        /// </summary>
        /// <returns>Результат работы окна - DialogResult</returns>
        public bool? OpenNewOrder() => OpenModalWindow<NewOrderView>();
        /// <summary>
        /// Открытие MainWindow
        /// </summary>
        /// <returns>Результат работы окна - DialogResult</returns>
        public void OpenMainWindow() => OpenWindow<MainWindow>();
        /// <summary>
        /// Открытие RegistrationCourier
        /// </summary>
        /// <returns>Результат работы окна - DialogResult</returns>
        public bool? OpenRegistrationCourier() => OpenModalWindow<RegistrationCourier>();

        /// <summary>
        /// Закрытие всех немодальных окон
        /// </summary>
        public void CloseWindows()
        {
            _logger.LogInformation(
                "Запущена команда массового закрытия всех немодальных окон. Всего окон: {Count}", 
                _openedWindows.Count
            );

            var openedWindows = _openedWindows.Values.ToList();
            foreach (var window in openedWindows)
            {
                if (window.IsVisible)
                    window.Close();
            }

            _openedWindows.Clear();
        }


        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="dataContext">Контекст нужного окна</param>
        public void CloseWindow(object dataContext)
        {
            var target = dataContext ?? this;

            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == target);

            if (window != null)
            {
                _logger.LogInformation(
                    "Запрос на принудительное закрытие окна для ViewModel: {ViewModelName}", 
                    target.GetType().Name
                );

                window.Close();
            }
        }
    }
}