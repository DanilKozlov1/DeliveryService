# Mermaid-диаграммы сценариев использования приложения

## Вход в приложение

```sequenceDiagram
    participant User as Пользователь
    participant View as View (EntranceView)
    participant ViewModel as ViewModel (EntranceViewModel)
    participant Service as Сервис (ClientService)
    participant Repository as Репозиторий (ClientRepository)
    participant EFCore as EF Core (DbContext)
    participant DB as База данных (PostgreSQL)

    User->>View: Вводит логин/пароль
    User->>View: Нажимает кнопку "Вход"
    View->>ViewModel: Вызывает команду LoginCommand
    ViewModel->>Service: GetClientByName(login)
    Service->>Repository: GetByName(name)
    Repository->>EFCore: LINQ-запрос (FirstOrDefault)
    EFCore->>DB: Формирует и выполняет SQL-запрос
    DB-->>EFCore: Возвращает запись (Client)
    EFCore-->>Repository: Материализует объект Client
    Repository-->>Service: Возвращает Client (или null)
    Service-->>ViewModel: Возвращает Client (или null)
    alt Клиент найден и пароль совпадает
        ViewModel->>Service: Сохраняет текущего клиента (SessionService)
        ViewModel-->>View: Команда завершается успешно
        View->>User: Открывает главное окно (MenuView)
    else Клиент не найден или пароль неверен
        ViewModel-->>View: Сообщение об ошибке
        View->>User: Показывает ошибку входа
    end
```