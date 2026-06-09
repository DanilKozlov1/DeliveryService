using DeliveryService.Commands;
using DeliveryService.Models;
using DeliveryService.Services;
using DeliveryService.Services.Interfaces;
using System.Windows;
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика взаимодействия пользователя и базы данных с NewOrderView
    /// </summary>
    public class NewOrderViewModel : BaseViewModel
    {
        private readonly IConfigService _configService;
        private readonly SessionService _sessionService;

        private readonly OrderService _orderService;
        private readonly ClientService _clientService;
        private readonly BasketService _basketService;
        private readonly WindowsService _windowService;
        private readonly CourierService _courierService;

        /// <summary>
        /// API-ключ для карты
        /// </summary>
        private readonly string _mapApiKey;
        /// <summary>
        /// Имя клиента
        /// </summary>
        private string _clientName;
        /// <summary>
        /// Номер клиента
        /// </summary>
        private string _clientPhone;
        /// <summary>
        /// Номер клиента, "очищенный" от всего, кроме цифр
        /// </summary>
        private string _cleanedPhoneNumber;

        private List<Basket> _clientBasket;

        /// <summary>
        /// Адрес отправки
        /// </summary>
        private string _addressFrom;
        /// <summary>
        /// Ширина адреса отправки
        /// </summary>
        private double _latFrom;
        /// <summary>
        /// Долгота адреса отправки
        /// </summary>
        private double _lonFrom;
        
        /// <summary>
        /// Адрес доставки
        /// </summary>
        private string _addressTo;
        /// <summary>
        /// Ширина адреса доставки
        /// </summary>
        private double _latTo;
        /// <summary>
        /// Долгота адреса доставки
        /// </summary>
        private double _lonTo;
        
        /// <summary>
        /// Цена
        /// </summary>
        private decimal _price;

        /// <summary>
        /// Переменная, необходимая для переключения режима откуда/куда
        /// </summary>
        private bool _isFromMode;

        /// <summary>
        /// API-ключ для карты
        /// </summary>
        public string MapApiKey
        {
            get => _mapApiKey;
        }
        /// <summary>
        /// Переменная, необходимая для переключения режима откуда/куда
        /// </summary>
        public bool IsFromMode
        {
            get => _isFromMode;
            set => SetProperty(ref _isFromMode, value);
        }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string ClientName
        {
            get => _clientName;
            set => SetProperty(ref _clientName, value);
        }
        /// <summary>
        /// Номер клиента
        /// </summary>
        public string ClientPhone
        {
            get => _clientPhone;
            set => SetProperty(ref _clientPhone, value);
        }
        /// <summary>
        /// Адрес отправки
        /// </summary>
        public string AddressFrom
        {
            get => _addressFrom;
            set => SetProperty(ref _addressFrom, value);
        }
        /// <summary>
        /// Ширина адреса отправки
        /// </summary>
        public double LatFrom
        {
            get => _latFrom;
            set => SetProperty(ref _latFrom, value);
        }
        /// <summary>
        /// Долгота адреса отправки
        /// </summary>
        public double LonFrom
        {
            get => _lonFrom;
            set => SetProperty(ref _lonFrom, value);
        }
        /// <summary>
        /// Адрес доставки
        /// </summary>
        public string AddressTo
        {
            get => _addressTo;
            set => SetProperty(ref _addressTo, value);
        }
        /// <summary>
        /// Ширина адреса доставки
        /// </summary>
        public double LatTo
        {
            get => _latTo;
            set => SetProperty(ref _latTo, value);
        }
        /// <summary>
        /// Долгота адреса доставки
        /// </summary>
        public double LonTo
        {
            get => _lonTo;
            set => SetProperty(ref _lonTo, value);
        }
        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        /// <summary>
        /// Команда создания и сохранения заказа 
        /// </summary>
        public ICommand SaveCommand { get; }
        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public ICommand CloseCommand { get; }
        /// <summary>
        /// Команда загрузки пользователя
        /// </summary>
        public ICommand LoadUserCommand { get; }


        public NewOrderViewModel(SessionService sessionService, IConfigService configService,
            OrderService orderService, ClientService clientService, BasketService basketService, WindowsService windowService, CourierService courierService)
        {
            _configService = configService;
            _mapApiKey = _configService.GetMapApiKey();

            _sessionService = sessionService;
            _orderService = orderService;
            _clientService = clientService;
            _basketService = basketService;
            _windowService = windowService;
            _courierService = courierService;

            _clientBasket = new List<Basket>();

            SaveCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(SaveOrderAsync, "Ошибка создания заказа"),
                canExecute: () => !IsBusy
            );

            CloseCommand = new RelayCommand(_ => CloseWindow(false));

            LoadUserCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(LoadUser, "Ошибка загрузки пользователя"),
                canExecute: () => !IsBusy
            );

            LoadUserCommand.Execute(null);

            IsFromMode = true;
        }


        /// <summary>
        /// Загрузка имени и телефона пользователя
        /// </summary>
        private async Task LoadUser()
        {
            ClientName = _sessionService.CurrentClient.Name;
            ClientPhone = _sessionService.CurrentClient.Phone.ToString();

            var (userBasket, totalPrice) = await _basketService.GetUserActiveBasketAsync(_sessionService.CurrentClient.Id);
            _clientBasket = userBasket;
            Price = totalPrice;
        }

        /// <summary>
        /// Проверка валидации ClientName, AddressFrom, AddressTo, и Price
        /// </summary>
        /// <returns>true, если все поля валидны, иначе false</returns>
        private bool ValidateProperty()
        {
            if (string.IsNullOrWhiteSpace(ClientName))
            {
                ErrorMessage = "Введите имя клиента";
                return false;
            }
            if (string.IsNullOrWhiteSpace(AddressFrom))
            {
                ErrorMessage = "Укажите адрес отправления";
                return false;
            }
            if (string.IsNullOrWhiteSpace(AddressTo))
            {
                ErrorMessage = "Укажите адрес доставки";
                return false;
            }
            if (Price <= 0)
            {
                ErrorMessage = "Цена должна быть больше нуля";
                return false;
            }

            return true;
        }
        /// <summary>
        /// Проверка валидации ClientPhone и "очишение" от не-цифр
        /// </summary>
        /// <returns>true, если валиден, иначе false</returns>
        private bool ValidatePhoneNumber()
        {
            if (string.IsNullOrWhiteSpace(ClientPhone))
            {
                ErrorMessage = "Введите номер телефона";
                _cleanedPhoneNumber = null;
                return false;
            }

            string cleaned = new string(ClientPhone.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(cleaned))
            {
                ErrorMessage = "Номер телефона должен содержать хотя бы одну цифру";
                _cleanedPhoneNumber = null;
                return false;
            }
            if (cleaned.Length < 10 || cleaned.Length > 11)
            {
                ErrorMessage = "Номер телефона должен содержать 10–11 цифр";
                _cleanedPhoneNumber = null;
                return false;
            }

            _cleanedPhoneNumber = cleaned;
            return true;
        }


        private async Task SaveOrderAsync()
        {
            ErrorMessage = null;

            if (!ValidateProperty())
                return;

            if (_clientBasket == null || _clientBasket.Count == 0)
            {
                ErrorMessage = "Корзина пуста. Невозможно оформить заказ.";
                return;
            }

            Client? client = await _clientService.GetClientByIdAsync(_sessionService.CurrentClient.Id);
            if (client == null)
            {
                if (!int.TryParse(ClientPhone, out int phoneNumber))
                {
                    ErrorMessage = "Номер телефона должен содержать только цифры";
                    return;
                }
            }

            var order = new Order
            {
                ClientId = _sessionService.CurrentClient.Id,
                Address_From = AddressFrom,
                Lat_From = LatFrom,
                Lon_From = LonFrom,
                Address_To = AddressTo,
                Lat_To = LatTo,
                Lon_To = LonTo,
                Price = Price,             
                Status = "Новый",
                Created_At = DateTime.UtcNow,
                BasketId = _clientBasket[0].Id, 
            };
            bool success = await _orderService.CreateOrderAsync(client, order);
            if (!success)
            {
                ErrorMessage = "Не удалось создать заказ";
                return;
            }
            _sessionService.CurrentOrder = order;

            foreach (var item in _clientBasket)
                await _basketService.RemoveItemAsync(item.Id);
            _windowService.OpenOrderAccept();
            CloseWindow(true);
        }
       
        /// <summary>
        /// Устанавливает выбранный адрес в поля для ввода
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="address"></param>
        public void SetSelectedAddress(double lat, double lon, string address)
        {
            if (IsFromMode)
            {
                LatFrom = lat;
                LonFrom = lon;
                AddressFrom = address;
                IsFromMode = !IsFromMode;
            }
            else
            {
                LatTo = lat;
                LonTo = lon;
                AddressTo = address;
                IsFromMode = !IsFromMode;

            }
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="result">Результат работы окна</param>
        private void CloseWindow(bool result)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            if (window != null)
            {
                window.DialogResult = result;
                window.Close();
            }
        }
    }
}