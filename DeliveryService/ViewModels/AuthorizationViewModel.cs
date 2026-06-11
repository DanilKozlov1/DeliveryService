using DeliveryService.Commands;
using DeliveryService.Services;
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика взаимодействия пользователя с базой данных для входа в приложение
    /// </summary>
    public class AuthorizationViewModel : BaseViewModel
    {
        private readonly WindowsService _windowsService;

        private readonly EntranceViewModel _entranceViewModel;
        private readonly RegistrationViewModel _registrationViewModel;
        
        /// <summary>
        /// Текущее view
        /// </summary>
        private object _currentView;

        /// <summary>
        /// Текущее view
        /// </summary>
        public object CurrentView
        {
            get => _currentView;
            set{ SetProperty(ref _currentView, value);}
        }

        /// <summary>
        /// Команда для входа в аккаунт
        /// </summary>
        public ICommand LoginCommand { get; }
        /// <summary>
        /// Команда для регистрации аккаунта
        /// </summary>
        public ICommand RegCommand { get; }
        /// <summary>
        /// Команда для закрытия окна
        /// </summary>
        public ICommand CloseCommand { get; }


        public AuthorizationViewModel(
            EntranceViewModel EntViewModel,
            RegistrationViewModel RegistrationViewModel,
            WindowsService windowService
            )
        {
            _entranceViewModel = EntViewModel;
            _registrationViewModel = RegistrationViewModel;
            _windowsService = windowService;
            _entranceViewModel.CloseRequested += () => _windowsService.CloseWindow(this);
            _registrationViewModel.RegistrationSuccess += () => CurrentView = EntViewModel;


            CurrentView = _entranceViewModel;

            LoginCommand = new RelayCommand(() => CurrentView = _entranceViewModel);
            RegCommand = new RelayCommand(() => CurrentView = _registrationViewModel);
            CloseCommand = new RelayCommand(() => _windowsService.CloseWindow(this));
        }
    }
}
