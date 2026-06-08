# 🚚 DeliveryService

Система онлайн доставки заказов. Разработано на C# WPF.

## Возможности

- **Диспетчерская** — интерактивная карта с позициями курьеров и точками доставки в реальном времени
- **Управление заказами** — создание, назначение курьера, отслеживание статусов
- **Курьеры** — список, статус онлайн/офлайн, статистика по выполненным заказам
- **Симуляция доставки** — визуализация движения курьера по маршруту на карте

## Технологии

| Слой | Технология |
|---|---|
| UI | WPF (.NET 8), XAML |
| Паттерн | MVVM |
| БД | PostgreSQL + Entity Framework Core |
| Карта | Yandex Maps API |
| DI | Microsoft.Extensions.DependencyInjection |

## Структура проекта

```
DeliveryService/                          # корень репозитория
├── DeliveryService.slnx
└── DeliveryService/
   ├── App.xaml
    ├── App.xaml.cs                       # DI
├── appsettings.json                     
    ├── Models/                           # сущности БД             
    │
    ├── Data/                             # EF Core
    │   
    │
    ├── Migrations/                       # миграции
    │   
    │
    ├── Repositories/                     # слой доступа к данным
    │  
    │
    ├── Services/                         # бизнес-логика
    │  
    │
    ├── DTO/                              # объекты передачи данных
    │ 
    │
    ├── ViewModels/                       # MVVM
    │   
    │
    ├── Views/                            # XAML-экраны
    │  
    │
    ├── Commands/                         # RelayCommand для MVVM
    │   
    │
    ├── Utils/                            # конвертеры, карта, скрипт
    │  
    │
    ├── Styles/                           # общие стили WPF
    │   
    │
    └── Images/                           # ресурсы меню
       
```

## Запуск

### Требования

- Visual Studio 2022
- .NET 8 SDK
- PostgreSQL 15+

### Установка

1. Клонировать репозиторий:
   ```bash
   git clone https://github.com/aurusxd/DeliveryService
   cd DeliveryService
   ```

2. Создать базу данных в PostgreSQL:
   ```sql
   CREATE DATABASE delivery_db;
   ```

3. Настроить строку подключения в `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Host=localhost;Port=5432;Database=delivery_db;Username=postgres;Password=ваш пароль от postgres"
     }
   }
   ```

4. Открыть `DeliveryService.sln` в Visual Studio и запустить (F5)

## Команда

| Участник | Зона ответственности |
|---|---|
| aurusxd | Backend + Frontend + TL|
| DanilKozlov1 | Backend |
| s-k-1-n-1 | Frontend |
| romchik-ww | Frontend |


---

# Документация
Все файлы документаций хранятся в корне проекта в папке **Dock**. 

## Документация API сервисов
Описание всех публичных методов сервисов, их параметров, возвращаемых значений и исключений. См. файл **API-Documentation.md**.

## Сценарии использования программы
Mermaid-диаграммы сценариев использования. См. файл **Architecture.md**.

## Документация по запуску базы данных в Docker
Полная инструкция по запуску контейнера с базой данных в Docker. См. файл **Docker-Databace.md**.

---