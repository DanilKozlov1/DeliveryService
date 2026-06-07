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

## Документация API сервисов
Описание всех публичных методов сервисов, их параметров, возвращаемых значений и исключений. См. папке **Docs** файл **API-Documentation.md**.

## Документация API сервисов
Mermaid-диаграммы сценариев использования. См. в папке **Docs** файл **Architecture.md**.

---

# Инструкция по запуску базы данных проекта DeliveryService в Docker
Проект использует PostgreSQL 16, запущенную в Docker-контейнере. Для упрощения управления применяется Docker Compose. Конфиденциальные данные (пароль) вынесены в файл .env, который не попадает в Git.

## Требования
Установленный Docker и Docker Compose (или Docker Desktop с WSL2 на Windows)
Для Windows - Docker Desktop в режиме Windows-контейнеров (или WSL2)
Для Ubuntu – стандартный Docker Engine + docker-compose-plugin

## Пошаговый запуск базы данных
Все команды выполняются из корневой директории проекта (там, где лежат Dockerfile, docker-compose.yml и .env.example).

- Шаг 1. Создание файла с переменными окружения
Скопируйте шаблон .env.example в .env:

Ubuntu (терминал):
```bash
cp .env.example .env
```

Windows (CMD или PowerShell):
cmd 
copy .env.example .env

- Шаг 2. Запуск контейнера через Docker Compose
В терминале (из корня проекта) выполните:

Ubuntu (возможно, потребуется sudo):
```bash
sudo docker-compose up -d
```

Windows (Docker Desktop):
cmd
docker-compose up -d

При первом запуске:
1. Соберётся образ PostgreSQL на основе Dockerfile.
2. Создастся том pg_delivery_data для хранения данных.
3. Запустится контейнер с именем delivery-postgres.
4. База будет доступна на порту 5433 хост-машины.

- Шаг 3. Проверка работоспособности
```bash
docker ps | grep delivery-postgres
```
Вы должны увидеть работающий контейнер. Также можно подключиться через любой PostgreSQL-клиент (DBeaver, pgAdmin, DataGrip) по адресу localhost:5433, пользователь postgres, пароль из .env.

## Настройка подключения в WPF приложении
В файле appsettings.json проекта строка подключения уже настроена на порт 5433 и параметры из .env:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5433;Database=delivery_db;Username=postgres;Password=admin"
  }
}
```

При первом запуске WPF-приложения Entity Framework Core автоматически:
1. Применит миграции.
2. Заполнит базу начальными данными.

## Полезные команды для управления
Остановка контейнера (данные сохраняются):
```bash
docker-compose stop
```

Запуск ранее остановленного контейнера:
```bash
docker-compose start
```

Полная остановка и удаление контейнера (том с данными остаётся):
```bash
docker-compose down
```

Полный сброс (удаление контейнера + тома данных):
```bash
docker-compose down -v
```

---
