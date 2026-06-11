using DeliveryService.Commands;
using DeliveryService.Services;
using DeliveryService.Services.Interfaces;
using System.Windows.Input;
using System.Windows.Threading;


namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика для OrderAcceptView
    /// </summary>
    public class OrderAcceptViewModel : BaseViewModel
    {
        private readonly IConfigService _configService;
        private readonly WindowsService _windowService;
        private readonly SimulationService _simulationService;
        private readonly SessionService _sessionService;
        private readonly CourierService _courierService;

        /// <summary>
        /// API-ключ для карты
        /// </summary>
        private readonly string _mapApiKey;

        /// <summary>
        /// Таймер, который перезагружает данные
        /// </summary>
        private DispatcherTimer _refreshTimer;
        /// <summary>
        /// Интервал таймера
        /// </summary>
        public double TIMER_INTERVAL = 3;
        /// <summary>
        /// Статус заказа
        /// </summary>
        private string _status;
        /// <summary>
        /// Сообщение об статусе
        /// </summary>
        private string _statusMessage;
        /// <summary>
        /// Номер заказа
        /// </summary>
        private string _orderNumber;
        /// <summary>
        /// Адрес откуда
        /// </summary>
        private string _addressFrom;
        /// <summary>
        /// Адрес куда
        /// </summary>
        private string _addressTo;

        /// <summary>
        /// API-ключ для карты
        /// </summary>
        public string MapApiKey
        {
            get => _mapApiKey;
        }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        /// <summary>
        /// Сообщение об статусе
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        /// <summary>
        /// Номер заказа
        /// </summary>
        public string OrderNumber
        {
            get => _orderNumber;
            set => SetProperty(ref _orderNumber, value);
        }
        /// <summary>
        /// Адрес откуда
        /// </summary>
        public string AddressFrom
        {
            get => _addressFrom;
            set => SetProperty(ref _addressFrom, value);    
        }
        /// <summary>
        /// Адрес куда
        /// </summary>
        public string AddressTo
        {
            get => _addressTo;
            set => SetProperty(ref _addressTo, value);
        }

        /// <summary>
        /// Команда возвращения на превыдущее окно
        /// </summary>
        public ICommand ReturnCommand { get; set; }

        /// <summary>
        /// Событие, когда курьер назначается на заказ
        /// </summary>
        public event Func<Task>? CourierAssigned;
        /// <summary>
        /// Событие, нужное для закрывания окна
        /// </summary>
        public event Action? ClosedRequested;


        public OrderAcceptViewModel(IConfigService configService,
            WindowsService windowService, SessionService sessionService, CourierService courierService, SimulationService simulationService)
        {
            _configService = configService;
            _mapApiKey = _configService.GetMapApiKey();

            _windowService = windowService;
            _sessionService = sessionService;
            _courierService = courierService;
            _simulationService = simulationService;
            ReturnCommand = new RelayCommand(
                execute: () => TryRunTaskAsync(ReturnToMenu, "Ошибка возврата в меню"),
                canExecute: () => !IsBusy);

            Status = "Ищем ближайщего курера..";
            ClosedRequested += () => _windowService.CloseWindow(this);
            _simulationService.CourierFinal += ChangeStatus;
            InitializeTimer();


        }


        /// <summary>
        /// Изменение статуса заказа на "Ваш заказ доставлен!"
        /// </summary>
        private void ChangeStatus()
        {
            StatusMessage = "Ваш заказ доставлен!";
        }

        /// <summary>
        /// Возвращение в MenuView
        /// </summary>
        private async Task ReturnToMenu()
        {
            _windowService.OpenMenu();
            ClosedRequested.Invoke();
        }

        /// <summary>
        /// Инициализация таймера
        /// </summary>
        private async void InitializeTimer()
        {
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(TIMER_INTERVAL);
            _refreshTimer.Tick += OnTimerTick;
            _refreshTimer.Start();
        }

        /// <summary>
        /// Обновление данных об курьере
        /// </summary>
        private async void OnTimerTick(object? sender, EventArgs e)
        {

            if (_sessionService?.CurrentOrder?.Courier == null)
            {

                bool success = await _courierService.AssignFreeCourierToOrderAsync(_sessionService?.CurrentOrder);
                if (success == false) return;
                Status = $"Ваш заказ принял: {_sessionService?.CurrentOrder?.Courier?.Name}";
                StatusMessage = "Курьер уже в пути";
                OrderNumber = $"№ ORD-{_sessionService?.CurrentOrder?.Id}";
                AddressFrom = _sessionService?.CurrentOrder?.Address_From;
                AddressTo = _sessionService?.CurrentOrder?.Address_To;
                CourierAssigned?.Invoke();
                _refreshTimer.Stop();

            }
        }
    }
}
