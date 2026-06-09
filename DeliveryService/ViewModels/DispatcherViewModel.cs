using DeliveryService.Commands;
using DeliveryService.Models;
using DeliveryService.Services;
using DeliveryService.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;

namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика взаимодействия пользователя и базы данных с DispatcherView
    /// </summary>
    public class DispatcherViewModel : BaseViewModel
    {
        private readonly IConfigService _configService;

        /// <summary>
        /// Интервал таймера
        /// </summary>
        private const int TIMER_INTERVAL = 30;
        /// <summary>
        /// API-ключ для карты
        /// </summary>
        private readonly string _mapApiKey;

        /// <summary>
        /// Таймер, который перезагружает данные
        /// </summary>
        private DispatcherTimer _refreshTimer;
        /// <summary>
        /// Активен ли таймер
        /// </summary>
        private bool _isTimerActive = false;

        private readonly OrderService _orderService;
        private readonly CourierService _courierService;
        private SimulationService _simulationService;

        public event Action? DisposeRequested;
        

        // ВАЖНО: Поменять названия статусов в комментариях
        /// <summary>
        /// Счётчик заказов со статусом "New"
        /// </summary>
        private int _newOrderCount;
        /// <summary>
        /// Счётчик заказов со статусом "InDelivery"
        /// </summary>
        private int _inTransitOrderCount;
        /// <summary>
        /// Счётчик заказов со статусом "Done"
        /// </summary>
        private int _completedOrderCount;
        /// <summary>
        /// Выбранный заказ
        /// </summary>
        private Order _selectedOrder;
        /// <summary>
        /// Выбранный курьер
        /// </summary>
        private Courier _selectedCourier;
        /// <summary>
        /// Выбранный курьер
        /// </summary>

        /// <summary>
        /// API-ключ для карты
        /// </summary>
        public string MapApiKey
        {
            get => _mapApiKey;
        }

        public Courier SelectedCourier
        {
            get => _selectedCourier;
            set => SetProperty(ref _selectedCourier, value);
        }

        public SimulationService SimulationService
        {
            get => _simulationService;
            set => SetProperty(ref _simulationService, value);
        }

        /// <summary>
        /// Выбранный Заказ
        /// </summary>
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        /// <summary>
        /// Список активных заказов
        /// </summary>
        public ObservableCollection<Order> ActiveOrders { get; }
        /// <summary>
        /// Список курьеров, которые онлайн
        /// </summary>
        public ObservableCollection<Courier> OnlineCouriers { get; }

        /// <summary>
        /// Список свободных курьеров, которые онлайн
        /// </summary>
        public ObservableCollection<Courier> FreeCouriers { get; }

        // ВАЖНО: Поменять названия статусов в комментариях
        /// <summary>
        /// Счётчик заказов со статусом "New"
        /// </summary>
        public int NewOrderCount
        {
            get => _newOrderCount;
            set => SetProperty(ref _newOrderCount, value);
        }
        /// <summary>
        /// Счётчик заказов со статусом "InDelivery"
        /// </summary>
        public int InTransitOrderCount
        {
            get => _inTransitOrderCount;
            set => SetProperty(ref _inTransitOrderCount, value);
        }
        /// <summary>
        /// Счётчик заказов со статусом "Done"
        /// </summary>
        public int CompletedOrderCount
        {
            get => _completedOrderCount;
            set => SetProperty(ref _completedOrderCount, value);
        }

        /// <summary>
        /// Команда загрузки данных
        /// </summary>
        public ICommand LoadDataCommand { get; }

        /// <summary>
        /// Команда назначения курьера на заказ
        /// </summary>
        public ICommand AssignCourierCommand { get; }
        /// <summary>
        /// Команда для выбора заказа
        /// </summary>
        public ICommand SelectOrderCommand { get; }

        /// <summary>
        /// Команда для выбора курьера
        /// </summary>
        public ICommand SelectCourierCommand { get; }

        /// <summary>
        /// Событие, которое вызывается при выборе заказа 
        /// </summary>
        public event Action<Order>? OrderSelected;
        /// <summary>
        /// Событие, которое вызывается при выборе курьера 
        /// </summary>
        public event Action<double,double,double,double,double,double>? CourierSelected;

        public DispatcherViewModel(IConfigService config, 
            OrderService orderService, CourierService courierService, SimulationService simulationService)
        {
            _configService = config;
            _mapApiKey = _configService.GetMapApiKey();

            _orderService = orderService;
            _courierService = courierService;
            _simulationService = simulationService;
 
            ActiveOrders = new ObservableCollection<Order>();
            OnlineCouriers = new ObservableCollection<Courier>();
            FreeCouriers = new ObservableCollection<Courier>();
            _simulationService.CourierFinal += _simulationService_CourierFinal;

            LoadDataCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(LoadDataAsync, "Ошибка загрузки"),
                canExecute: () => !IsBusy
            );

            AssignCourierCommand = new RelayCommandAsync(async order =>
            {
                if (SelectedCourier == null || order == null) return;
                Order ord = (Order)order;
                bool success = await _courierService.AssignCourierToOrderAsync(SelectedCourier.Id, ord.Id);
                if (success) await LoadDataAsync();
            });

            SelectOrderCommand = new RelayCommandAsync(async order =>
            {
                if (order == null) return;
                SelectedOrder = (Order)order;
                OrderSelected?.Invoke((Order)order);
            });


            SelectCourierCommand = new RelayCommandAsync(async parameter =>
            {
                if (parameter is not Courier courier)
                    return;
                SelectedCourier = courier;

                SelectedOrder = await _orderService
                    .FindOrderByCourierIdAsync(courier.Id);

                if (SelectedOrder == null) return;
                if (SelectedCourier == null) return;

                CourierSelected?.Invoke(SelectedOrder.Lat_From, SelectedOrder.Lon_From, SelectedOrder.Lat_To, SelectedOrder.Lon_To, SelectedCourier.Current_Lat, SelectedCourier.Current_Lon);
            });

        }


        /// <summary>
        /// Функция, срабатывающая при достижении курьером финальной точки
        /// </summary>
        private void _simulationService_CourierFinal()=> LoadDataCommand.Execute(null);

        /// <summary>
        /// Загрузка данных о заказах
        /// </summary>
        private async Task LoadOrdersAsync()
        {
            var allOrders = await _orderService.GetAllAsync();

            if (allOrders != null && allOrders.Any())
            {
                var activeOrders = allOrders.Where(o => o.Status != "Доставлен").OrderByDescending(o => o.Created_At).ToList();

                ActiveOrders.Clear();
                foreach (var order in activeOrders) 
                    ActiveOrders.Add(order);

                NewOrderCount = activeOrders.Count(o => o.Status == "Новый");
                InTransitOrderCount = activeOrders.Count(o => o.Status == "В пути");
                CompletedOrderCount = allOrders.Count(o => o.Status == "Доставлен");
            }
            else
            {
                ActiveOrders.Clear();

                NewOrderCount = 0;
                InTransitOrderCount = 0;
                CompletedOrderCount = 0;

                return;
            }
        }
        /// <summary>
        /// Загрузка свободных курьеров
        /// </summary>
        /// <returns></returns>
        private async Task LoadFreeCouriersAsync()
        {
            var freeCouriers = await _courierService.GetFreeCouriersAsync();
            if (freeCouriers == null)
                return;

            var onlineCouriers = freeCouriers.Where(c => c.IsActive).ToList();

            FreeCouriers.Clear();
            foreach (var courier in onlineCouriers)
                FreeCouriers.Add(courier);
;

        }

        /// <summary>
        /// Назначение курьера на заказ
        /// </summary>
        /// <param name="courierId">Айди курьера</param>
        /// <param name="orderId">Айди заказа</param>
        /// <returns></returns>
        public async Task AssignCourier(int courierId, int orderId) => await _courierService.AssignCourierToOrderAsync(courierId, orderId);

        /// <summary>
        /// Загрузка данных об курьерах
        /// </summary>
        private async Task LoadCouriersAsync()
        {
            var allCouriers = await _courierService.GetAllAsync();
            if (allCouriers == null) 
                return;

            var onlineCouriers = allCouriers.Where(c => c.IsActive).ToList();

            OnlineCouriers.Clear();
            foreach (var courier in onlineCouriers)
                OnlineCouriers.Add(courier);
        }

        /// <summary>
        /// Загрузка данных
        /// </summary>
        private async Task LoadDataAsync()
        {
            await LoadOrdersAsync();
            await LoadCouriersAsync();
            await LoadFreeCouriersAsync();
        }
        /// <summary>
        /// Старт таймера
        /// </summary>
        public void TimerStart()
        {
            Debug.WriteLine("Timer start");
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(TIMER_INTERVAL);
            _refreshTimer.Tick += OnTimerTick;
            _refreshTimer.Start();
            _isTimerActive = true;
        }
        /// <summary>
        /// Остановка таймера
        /// </summary>
        public void TimerStop()
        {
            if (_isTimerActive)
            {
                Debug.WriteLine("Timer stop");
                _refreshTimer.Stop();
                _refreshTimer.Tick -= OnTimerTick;
                _isTimerActive = false;
            }
        }

        /// <summary>
        /// Логика таймера
        /// </summary>
        private void OnTimerTick(object? sender, EventArgs e) => LoadDataCommand.Execute(null);

        public async Task SaveCoords(double v1, double v2)
        {
            SelectedCourier.Current_Lat = v1;
            SelectedCourier.Current_Lon = v2;
            await _courierService.UpdateAsync(SelectedCourier);
        }
    }
}