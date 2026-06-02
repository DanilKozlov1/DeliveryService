using DeliveryService.Services;
using DeliveryService.Utils;
using DeliveryService.ViewModels;
using System.Windows;


namespace DeliveryService.Views
{
    /// <summary>
    /// Interaction logic for NewOrderView.xaml
    /// </summary>
    public partial class NewOrderView : Window
    {
        public NewOrderView(NewOrderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OnAddressSelected(double lat, double lon, string address)
        {
            if (DataContext is NewOrderViewModel vm)
            {
                vm.SetSelectedAddress(lat, lon, address);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            MapInitializer.AddressSelected -= OnAddressSelected;
            base.OnClosed(e);
            Map.Dispose();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is NewOrderViewModel vm)
            {
                await MapInitializer.Initialize(Map, vm.MapApiKey);
                MapInitializer.AddressSelected += OnAddressSelected;
            }
        }
    }
}
