# API сервисов приложения DeliveryService

## WindowsService
Сервис управления окнами приложения. Использует DI для создания экземпляров окон и отслеживает уже открытые немодальные окна, чтобы не создавать дубликаты. Позволяет открывать окна как модальные, так и немодальные, а также закрывать окна по контексту или все сразу.

### void OpenEntrance()
**Описание**: Открывает немодальное окно входа (EntranceView). Если окно уже открыто – активирует его (не создаёт новый экземпляр).

**Пример вызова**:
```csharp
_windowsService.OpenEntrance();
```

### void OpenOrderAccept()
**Описание**: Открывает немодальное окно подтверждения заказа (OrderAcceptView), которое появляется после успешного оформления заказа для дальнейшего отслеживания.

**Пример вызова**:
```csharp
_windowsService.OpenOrderAccept();
```

### void OpenMenu()
**Описание**: Открывает немодальное окно меню с едой (MenuView).

**Пример вызова**:
```csharp
_windowsService.OpenMenu();
```

### bool? OpenNewOrder()
**Описание**: Открывает модальное окно создания нового заказа (NewOrderView). Возвращает результат диалога (true/false/null в зависимости от того, как закрыто окно).

**Возвращает**: bool? – true, если диалог завершён успешно, false – если отклонён, null – если закрыт без выбора.

**Пример вызова**:
```csharp
bool? result = _windowsService.OpenNewOrder();
if (result == true) 
{
    // Обновить список заказов
}
```

### void OpenMainWindow()
**Описание**: Открывает немодальное главное окно приложения (MainWindow). Обычно вызывается после успешного входа.

**Пример вызова**:
```csharp
_windowsService.OpenMainWindow();
```

### bool? OpenRegistrationCourier()
**Описание**: Открывает модальное окно регистрации курьера (RegistrationCourier). Возвращает результат диалога.

**Возвращает**: bool? – true при успешной регистрации, false – при отмене, null – при закрытии без сохранения.

**Пример вызова**:
```csharp
bool? registered = _windowsService.OpenRegistrationCourier();
```

### void CloseWindows()
**Описание**: Закрывает все немодальные окна, которые были открыты через этот сервис (отслеживаются во внутреннем словаре). Модальные окна этим методом не закрываются.

**Пример вызова**: 
```csharp
_windowsService.CloseWindows();
```

### void CloseWindow(object dataContext)
**Описание**: Закрывает окно, у которого свойство DataContext совпадает с переданным объектом. Удобно для закрытия текущего окна из ViewModel.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `dataContext` | `object` | Контекст данных окна |

**Пример вызова**:
```csharp
_windowsService.CloseWindow(this);
```

## SessionService
Сервис для хранения текущего пользователя (клиента) и текущего заказа в рамках сессии приложения. Позволяет получить или изменить эти данные, а также подписаться на событие об их изменении.

### Свойства
#### Client? CurrentClient
**Описание**: Текущий аутентифицированный пользователь (клиент). При установке нового значения автоматически вызывает событие CurrentUserChanged.
**Тип**: Client? (может быть null – если пользователь не вошёл)

**Пример использования**:
```csharp
_sessionService.CurrentClient = client;

// Получение текущего клиента
var client = _sessionService.CurrentClient;
```

#### Order? CurrentOrder
**Описание**: Текущий заказ, выбранный или создаваемый пользователем. При изменении также вызывает событие CurrentUserChanged.
**Тип**: Order?

**Пример использования**:
```csharp
_sessionService.CurrentOrder = order;
var order = _sessionService.CurrentOrder;
```

### Событие
#### event Action? CurrentUserChanged
**Описание**: Возникает при изменении свойства CurrentClient или CurrentOrder. Позволяет другим компонентам приложения реагировать на смену пользователя или заказа.

**Пример подписки**:
```csharp
_sessionService.CurrentUserChanged += () =>
{
    // Обновить UI, перезагрузить данные и т.д.
    Console.WriteLine("Текущий пользователь или заказ изменились");
};
```

### Используемые модели
* **Client**

* **Order**

## SimulationService
Сервис симуляции движения курьера по маршруту. Позволяет запустить анимацию перемещения курьера по заданным координатам, обновляя его позицию в базе данных, а также уведомлять подписчиков о каждом шаге и о завершении маршрута. Поддерживает остановку симуляции.

### Методы
#### Task StartAsync(List<List<double>> points, Courier courier)
**Описание**: Запускает симуляцию движения курьера по заданному маршруту. Находит ближайшую к текущему положению курьера точку и последовательно перемещает его по оставшимся точкам. На каждом шаге:
 - Вызывается событие CourierMoved с новыми координатами.
 - Обновляются координаты курьера в объекте и в базе данных (через CourierService.Update).
 - Делается пауза 600 мс (задержка для визуализации движения).
 - При достижении последней точки маршрута:
    1. Статус заказа, назначенного на этого курьера, меняется на «Доставлен».
    2. Заказ добавляется в историю со статусом «Доставлен».
    3. Вызывается событие CourierFinal.
Если симуляция уже запущена, предыдущая отменяется.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `points` | `List<List<double>>` | Список точек маршрута. Каждая точка – список из двух чисел: [широта, долгота] |
| `courier` | `Courier` | Курьер, перемещение которого симулируется |

**Исключения**:
`TaskCanceledException` – если симуляция была остановлена вызовом метода Stop() или отменена через CancellationToken.
`ArgumentNullException` – если points или courier равны null.

**Пример вызова**:
```csharp
await _simulationService.StartAsync(points, selectedCourier);
```

#### void Stop()
**Описание**: Останавливает текущую симуляцию (если она активна). Отменяет внутренний CancellationToken, прерывая выполнение метода StartAsync.

**Пример вызова**:
```csharp
_simulationService.Stop();
```

### События
#### event Action<double, double>? CourierMoved
**Описание**: Возникает при каждом перемещении курьера на новую точку маршрута (на каждом шаге цикла симуляции). Передаёт новые координаты курьера (широту и долготу).

**Параметры события**:

| Имя | Тип | Описание |
|---|---|---|
| `lat` | `double` | Широта (latitude) |
| `lon` | `double` | Долгота (longitude) |

**Пример подписки**:
```csharp
_simulationService.CourierMoved += (lat, lon) =>
{
    // Обновить положение маркера курьера на карте
    UpdateCourierMarker(lat, lon);
};
```

#### event Action? CourierFinal
**Описание**: Возникает, когда курьер достиг конечной точки маршрута и заказ успешно помечен как «Доставлен». Сигнализирует об окончании симуляции.

**Пример подписки**:
```csharp
_simulationService.CourierFinal += () =>
{
    // Перезагрузить список заказов, показать уведомление
    LoadOrdersCommand.Execute(null);
    MessageBox.Show("Заказ доставлен!");
};
```

### Используемые модели
* **Courier**

## ConfigService
Сервис конфигурации, предоставляющий доступ к настройкам приложения из файлов appsettings.json и переменных окружения.

### Методы
#### string GetMapApiKey()
**Описание**: Возвращает API-ключ для карты из конфигурации (секция ApiMap:Key). Если ключ отсутствует или имеет значение null, выбрасывает исключение.

**Возвращает**: string – API-ключ.

**Исключения**:
`InvalidOperationException` – API-ключ не найден в конфигурации.

**Пример вызова**:
```csharp
string apiKey = _configService.GetMapApiKey();
MapInitializer.Initialize(map, apiKey);
```

## BasketService
Сервис управления корзиной пользователя. Позволяет добавлять, обновлять, удалять позиции, получать содержимое корзины и очищать её.

### Task<(List<Basket> userBasket, decimal totalPrice)> GetUserBasketAsync(int userId)
**Описание**: Возвращает все позиции корзины указанного пользователя и их общую стоимость.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `userId` | `int` | ID пользователя |

**Возвращает**: кортеж (List<Basket> userBasket, decimal totalPrice) – список позиций корзины и суммарная цена (сумма полей Price).

**Пример вызова**:
```csharp
var (basket, total) = await _basketService.GetUserBasketAsync(5);
Console.WriteLine($"Всего позиций: {basket.Count}, на сумму: {total}");
```

### Task<(List<Basket> userBasket, decimal totalPrice)> GetUserActiveBasketAsync(int userId)
**Описание**: Возвращает только активные позиции корзины и их общую стоимость.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `userId` | `int` | ID пользователя |

**Возвращает**: кортеж (List<Basket> userBasket, decimal totalPrice) – отфильтрованный список и сумма.

**Пример вызова**:
```csharp
var (activeBasket, total) = await _basketService.GetUserActiveBasketAsync(5);
```

### Task<bool> AddNewBasketItemAsync(int userId, int foodId, int quantity)
**Описание**: Создаёт новую запись в корзине для указанного пользователя и позиции меню.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `userId` | `int` | ID пользователя |
| `foodId` | `int` | ID блюда из справочника |
| `quantity` | `int` | Количество единиц |

**Возвращает**: true, если операция успешна (блюдо найдено и запись добавлена), иначе false.

**Пример вызова**:
```csharp
bool added = await _basketService.AddNewBasketItemAsync(1, 42, 2);
```

### Task<bool> AddOrUpdateBasketItemAsync(int userId, int foodId, int quantity)
**Описание**: Добавляет или увеличивает количество существующей позиции в активной корзине. Если блюдо уже есть в корзине (активной) – увеличивает Quantity и пересчитывает Price. Если нет – создаёт новую запись.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `userId` | `int` | ID пользователя |
| `foodId` | `int` | ID блюда |
| `quantity` | `int` | Количество для добавления |

**Возвращает**: true, если операция выполнена (блюдо найдено), иначе false.

**Пример вызова**:
```csharp
await _basketService.AddOrUpdateBasketItemAsync(1, 42, 1);
```

### Task<bool> RemoveItemAsync(int basketId)
**Описание**: Удаляет конкретную запись корзины по её идентификатору.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `basketId` | `int` | ID записи корзины |

**Возвращает**: true, если запись существовала и была удалена; false, если запись не найдена.

**Пример вызова**:
```csharp
await _basketService.RemoveItemAsync(10);
```

### Task ClearUserBasketAsync(int userId)
**Описание**: Удаляет все записи корзины указанного пользователя. Обычно вызывается после оформления заказа.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `userId` | `int` | ID пользователя |

**Возвращает**: ничего.

**Пример вызова**:
```csharp
await _basketService.ClearUserBasketAsync(1);
```

### Используемые модели
* **Basket**

## FoodService
Сервис для работы с блюдами (позициями меню). Позволяет получать блюда по идентификатору, все блюда, фильтровать по категории, а также добавлять новые блюда.

### Методы
#### Task<Food?> GetByIdAsync(int foodId)
**Описание**: Возвращает объект блюда по его уникальному идентификатору.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `foodId` | `int` | ID блюда |

Возвращает: Food? – найденный объект или null, если блюдо не существует.

**Пример вызова**:
```csharp
var food = await _foodService.GetByIdAsync(42);
if (food != null) 
{
    // Отобразить информацию о блюде
}
```

#### Task<List<Food>> GetAllAsync()
**Описание**: Возвращает список всех блюд, доступных в меню.

**Возвращает**: List<Food> – список всех блюд (может быть пустым, если в базе нет ни одного блюда).

**Пример вызова**:
```csharp
var allFood = await _foodService.GetAllAsync();
foreach (var food in allFood) 
{
    Console.WriteLine($"{food.Title} – {food.Price} руб.");
}
```

#### Task<List<Food>> GetAllFromCategoryAsync(int categoryId)
**Описание**: Возвращает список блюд, принадлежащих указанной категории (например, «Пиццы», «Напитки»).
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `categoryId` | `int` | ID категории |

**Возвращает**: List<Food> – список блюд в данной категории. Если блюд нет – пустой список.

Пример вызова:
```csharp
var pizzaList = await _foodService.GetAllFromCategoryAsync(1);
```

#### Task<bool> AddAsync(Food food)
**Описание**: Добавляет новое блюдо в базу данных.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `food` | `Food` | Объект блюда для добавления |

**Возвращает**: true, если операция выполнена (объект не null и добавление прошло успешно). false, если передан null.

**Пример вызова**:
```csharp
var newFood = new Food { 
    Title="Название", Description="Описание", 
    ImageUrl="Ссылка на изображение", Weight=2,
    CategoriesId=2, Price=350 
    };
bool added = await _foodService.AddAsync(newFood);
if (!added) 
{
    MessageBox.Show("Не удалось добавить блюдо");
}
```

### Используемые модели
* **Food**

## FoodCategoryService
Сервис для работы с категориями блюд. Позволяет получать категории по ID, список всех категорий и добавлять новые.

### Методы
#### Task<Categories?> GetById(int categoryId)
**Описание**: Возвращает категорию по её идентификатору.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `categoryId` | `int` | ID категории | 

**Возвращает**: Categories? – объект категории, если найден; иначе null.

**Пример вызова**:
```csharp
var cat = await _foodCategoryService.GetById(3);
```

#### Task<List<Categories>> GetAllAsync()
**Описание**: Возвращает список всех категорий блюд.

**Возвращает**: List<Categories> – список категорий (может быть пустым).

**Пример вызова**:
```csharp
var categories = await _foodCategoryService.GetAllAsync();
```

#### Task<bool> AddAsync(Categories categories)
**Описание**: Добавляет новую категорию в базу данных.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `categories` | `Categories` | Объект категории | 

**Возвращает**: true, если категория добавлена (объект не null); false, если передан null.

**Пример вызова**:
```csharp
var newCat = new Categories { Name = "Название категории" };
bool added = await _foodCategoryService.AddAsync(newCat);
```

### Используемые модели
* **Categories**

## OrderService
Сервис для работы с заказами. Позволяет получать список заказов, создавать новые, изменять статус, отменять, удалять, добавлять в историю статусов и искать заказ по ID курьера.

### Методы
#### Task<List<Order>> GetAllAsync()
**Описание**: Возвращает список всех заказов.

**Возвращает**: List<Order> – коллекция всех заказов.

**Пример вызова**:
```csharp
var allOrders = await _orderService.GetAllAsync();
```

#### Task<List<Order>> GetActiveOrdersAsync()
**Описание**: Возвращает список активных заказов.

**Возвращает**: List<Order> – активные заказы.

**Пример вызова**:
```csharp
var active = await _orderService.GetActiveOrdersAsync();
```

#### Task<Order?> GetByIdAsync(int id)
**Описание**: Возвращает заказ по его идентификатору.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `id` | `int` | ID заказа |

**Возвращает**: Order? – найденный заказ или null.

**Пример вызова**:
```csharp
var order = await _orderService.GetByIdAsync(123);
```

#### Task<bool> CreateOrderAsync(Client client, Order order)
**Описание**: Создаёт новый заказ. Если переданный клиент имеет Id == 0, он сначала добавляется в БД. Затем заказу присваивается статус «Новый», дата создания (UTC) и привязывается к клиенту. Заказ сохраняется.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `client` | `Client` | Объект клиента |
| `order` | `Order` | Объект заказа |

**Возвращает**: true, если операция выполнена (клиент не null и адреса заполнены); иначе false.

**Пример вызова**:
```csharp
// не полные параметры модели
var client = new Client { Name = "Иван", Phone = "123456" };
var order = new Order { Address_From = "ул. Ленина 1", Address_To = "ул. Гагарина 10" };
bool created = await _orderService.CreateOrderAsync(client, order);
```

#### Task<bool> ChangeStatusAsync(int orderId, string newStatus, string? feedback = null)
**Описание**: Изменяет статус заказа и добавляет запись в историю статусов.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `orderId` | `int` | ID заказа |
| `newStatus` | `string` | Новый статус |
| `feedback` | `string?` | Необязательный отзыв или комментарий |

**Возвращает**: true, если заказ найден и статус обновлён; false – заказ не найден.

**Пример вызова**:
```csharp
await _orderService.ChangeStatusAsync(5, "В пути");
```

#### Task<bool?> CancelOrderAsync(int orderId, string? feedback = null)
**Описание**: Отменяет заказ, добавляет запись в историю.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `orderId` | `int` | ID заказа |
| `feedback` | `string?` | Причина отмены или комментарий |

**Возвращает**: bool? – true при отмене, false если заказ не найден.

**Пример вызова**:
```csharp
bool? cancelled = await _orderService.CancelOrderAsync(10, "Передумал");
```

#### Task<Order?> FindOrderByCourierIdAsync(int courierId)
**Описание**: Находит заказ, назначенный на конкретного курьера.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `courierId` | `int` | ID курьера |

**Возвращает**: Order? – заказ, если найден; иначе null.

**Пример вызова**:
```csharp
var order = await _orderService.FindOrderByCourierIdAsync(courier.Id);
```

#### Task<bool> RemoveOrderAsync(int orderId)
**Описание**: Удаляет заказ по его идентификатору.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `orderId` | `int` | ID заказа |

**Возвращает**: true – удалён; false – заказ не найден.

**Пример вызова**:
```csharp
await _orderService.RemoveOrderAsync(15);
```

#### Task Update(Order order)
**Описание**: Обновляет существующий заказ в базе данных (полностью заменяет его поля).
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `order` | `Order` | Объект заказа с изменёнными данными |

**Пример вызова**:
```csharp
order.Status = "Доставлен";
await _orderService.Update(order);
```

#### Task<bool> DeleteAsync(Order order)
**Описание**: Удаляет заказ, переданный как объект.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `order` | `Order` | Объект заказа |

**Возвращает**: true – удалён; false – передан null.

**Пример вызова**:
```csharp
await _orderService.DeleteAsync(order);
```

#### Task AddToHistory(Order order, string status, string? feedback = null)
**Описание**: Добавляет запись в историю изменения статусов для указанного заказа.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `order` | `Order` | Объект заказа | 
| `status` | `string` | Новый статус |
| `feedback` | `string?` | Комментарий |

**Пример вызова**:
```csharp
await _orderService.AddToHistory(order, "Доставлен", "Клиент доволен");
```

### Используемые модели
* **Order**
* **Client**
* **OrderStatusHistory**

## ClientService
Сервис для работы с клиентами. Позволяет добавлять новых клиентов, получать клиента по идентификатору или по имени (логину).

### Методы
#### Task<bool> AddClientAsync(Client client)
**Описание**: Добавляет нового клиента в базу данных.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `client` | `Client` | Объект клиента |

**Возвращает**: true, если клиент не null и операция выполнена; false, если передан null.

**Пример вызова**:
```csharp
var newClient = new Client { Name = "Анна", Phone = "+79991234567" };
bool added = await _clientService.AddClientAsync(newClient);
```

#### Task<Client?> GetClientById(int userId)
**Описание**: Возвращает клиента по его уникальному идентификатору.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `userId` | `int` | ID клиента |

**Возвращает**: Client? – найденный клиент или null, если клиент не существует.

**Пример вызова**:
```csharp
var client = await _clientService.GetClientById(5);
if (client != null) Console.WriteLine(client.Name);
```

#### Task<Client?> GetClientByName(string name)
**Описание**: Возвращает клиента по его имени (логину). Предполагается, что имена уникальны.
**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `name` | `string` | Имя (логин) клиента |

**Возвращает**: Client? – найденный клиент или null.

**Пример вызова**:
```csharp
var client = await _clientService.GetClientByName("Анна");
```

### Используемые модели
* **Client**

## CourierService
Сервис для работы с курьерами. Позволяет получать списки курьеров (всех, активных, свободных), добавлять новых, назначать на заказы, переключать онлайн-статус, обновлять координаты, удалять.

### Методы
#### Task<List<Courier>> GetAllAsync()
**Описание**: Возвращает список всех курьеров.

**Возвращает**: List<Courier> – коллекция всех курьеров.

**Пример вызова**:
```csharp
var allCouriers = await _courierService.GetAllAsync();
```

#### Task<List<Courier>> GetActiveCouriersAsync()
**Описание**: Возвращает список активных (онлайн) курьеров.

**Возвращает**: List<Courier> – активные курьеры.

**Пример вызова**:
```csharp
var online = await _courierService.GetActiveCouriersAsync();
```

#### Task<List<Courier>> GetFreeCouriersAsync()
**Описание**: Возвращает список курьеров, которые онлайн и не имеют назначенного заказа.

**Возвращает**: List<Courier> – свободные курьеры.

**Пример вызова**:
```csharp
var free = await _courierService.GetFreeCouriersAsync();
```

#### Task<bool> AddCourierAsync(Courier courier)
**Описание**: Добавляет нового курьера в базу. Автоматически устанавливает:
 - IsActive = true (онлайн)
 - Created_At = DateTime.UtcNow
 - Current_Lat = 0.0, Current_Lon = 0.0

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `courier` | `Courier` | Объект курьера |

**Возвращает**: true, если курьер не null; false – если передан null.

**Пример вызова**:
```csharp
var courier = new Courier { Name = "Иван", Phone = "123" };
bool added = await _courierService.AddCourierAsync(courier);
```

#### Task<bool> AssignCourierToOrderAsync(int courierId, int orderId)
**Описание**: Назначает конкретного курьера на заказ. Проверяет, что:
 1. Курьер и заказ существуют.
 2. Заказ ещё не назначен другому курьеру.
При успехе обновляет:
 1. order.CourierId = courierId
 2. order.Status = "В пути"
 3. courier.Current_Lat/Lon = координаты отправления (order.Lat_From/Lon_From)

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `courierId` | `int` |	ID курьера |
| `orderId` | `int` | ID заказа |

**Возвращает**: true, если назначение выполнено; false – если курьер или заказ не найдены, или заказ уже назначен.

**Пример вызова**:
```csharp
bool assigned = await _courierService.AssignCourierToOrderAsync(5, 10);
```

#### Task<bool> AssignFreeCourierToOrderAsync(Order order)
**Описание**: Автоматически назначает случайного свободного курьера на переданный заказ. Если свободных курьеров нет, сбрасывает order.CourierId = null и order.Status = "Новый". Выбирает курьера случайным образом из списка свободных.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `order` | `Order` | Заказ, на который нужно назначить курьера |

**Возвращает**: true, если назначение выполнено; false – если заказ null, нет свободных курьеров или произошла ошибка.

**Пример вызова**:
```csharp
bool assigned = await _courierService.AssignFreeCourierToOrderAsync(order);
```

#### Task<bool> ToggleCourierOnlineAsync(int courierId)
**Описание**: Переключает статус курьера (онлайн/офлайн). Если курьер не найден – возвращает false.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `courierId` | `int` | ID курьера |

**Возвращает**: true, если статус успешно изменён; false – курьер не найден.

**Пример вызова**:
```csharp
await _courierService.ToggleCourierOnlineAsync(3);
```

#### Task<bool> RemoveCourierAsync(int courierId)
**Описание**: Удаляет курьера из базы данных по его ID.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `courierId` | `int` | ID курьера |

**Возвращает**: true, если курьер существовал и удалён; false – если не найден.

**Пример вызова**:
```csharp
bool removed = await _courierService.RemoveCourierAsync(7);
```

#### Task<Courier?> GetById(int id)
**Описание**: Возвращает курьера по его идентификатору.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `id` | `int` | ID курьера |

**Возвращает**: Courier? – найденный курьер или null.

**Пример вызова**:
```csharp
var courier = await _courierService.GetById(5);
```

#### Task Update(Courier courier)
**Описание**: Обновляет существующего курьера в базе данных (полностью заменяет его поля).

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `courier` | `Courier` | Объект курьера с изменёнными данными |

**Возвращает**: Task (ничего не возвращает).

**Пример вызова**:
```csharp
courier.IsActive = false;
await _courierService.Update(courier);
```

#### Task SaveCourierCoords(double Lat, double Lon, Courier courier)
**Описание**: Метод для сохранения текущих координат курьера. Устанавливает Current_Lat и Current_Lon в переданном объекте и вызывает Update.

**Параметры**:

| Имя | Тип | Описание |
|---|---|---|
| `Lat` | `double` | Широта |
| `Lon` | `double` | Долгота |
| `courier` | `Courier` | Курьер, которому задаются координаты |

**Пример вызова**:
```csharp
await _courierService.SaveCourierCoords(55.751, 37.618, selectedCourier);
```

### Используемые модели
* **Courier**