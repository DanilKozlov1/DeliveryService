using DeliveryService.Models;


namespace DeliveryService.Services
{
    public class SimulationService
    {

        private readonly CourierService _courierService;
        private readonly SessionService _sessionService;
        private readonly OrderService _orderService;
        private CancellationTokenSource? _simulationCts;

        public event Action<double, double>? CourierMoved;
            
        public event Action? CourierFinal;


        public SimulationService(CourierService courierService, SessionService sessionService,OrderService orderService)
        {
            _courierService = courierService;
            _sessionService = sessionService;
            _orderService = orderService;
        }

        
        /// <summary>
        /// Симуляция маршрута
        /// </summary>
        /// <param name="points">Список списков координат маршрута</param>
        /// <returns></returns>
        public async Task StartAsync(List<List<double>> points, Courier courier)
        {
            _simulationCts?.Cancel();
            _simulationCts?.Dispose();
            _simulationCts = new CancellationTokenSource();
            var token = _simulationCts.Token;

            var startIndex = NearestIndexFinder(points, courier.Current_Lat, courier.Current_Lon);
            var remaining = points.Skip(startIndex);

            foreach (var point in remaining)
            {
                if (token.IsCancellationRequested) return;

                var lat = point[0];
                var lon = point[1];

  
                CourierMoved?.Invoke(lat, lon);

                courier.Current_Lat = lat;
                courier.Current_Lon = lon;
                await _courierService.UpdateAsync(courier);
                await Task.Delay(600, token);
            }
            var orderPoint = remaining.Last();
            if (courier.Current_Lat == orderPoint[0] && courier.Current_Lon == orderPoint[1])
            {
                var order = await _orderService.FindOrderByCourierIdAsync(courier.Id);
                if(order==null) return;
                order.Status = "Доставлен";
                order.Courier = null;
                await _orderService.UpdateAsync(order);
                await _orderService.AddToHistory(order, status: "Доставлен");
                CourierFinal?.Invoke();
            }
        }

        /// <summary>
        /// Остановка симуляции
        /// </summary>
        public void Stop()
        {
            _simulationCts?.Cancel();
            _simulationCts?.Dispose();
            _simulationCts = null;
        }

        /// <summary>
        /// Функция, ищущая ближайшую точку к курьеру 
        /// </summary>
        /// <param name="points">Координаты</param>
        /// <param name="curLat">Lat курьера</param>
        /// <param name="curLon">Lon курьера</param>
        /// <returns>Возвращает индекс ближайшей пары точек</returns>
        private int NearestIndexFinder(List<List<double>> points, double curLat, double curLon)
        {
            if (points.Count == 0) return -1;
            int bestIndex = 0;
            double bestDist = double.MaxValue;

            for (int i = 0; i < points.Count - 1; i++)
            {
                double cLat = points[i][0];
                double cLon = points[i][1];

                double Lat = cLat - curLat;
                double Lon = cLon - curLon;
                var fDist = (Lat * Lat) + (Lon * Lon);

                if (fDist < bestDist)
                {
                    bestDist = fDist;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
    }
}
