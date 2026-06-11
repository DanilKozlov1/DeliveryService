using DeliveryService.Models;
using DeliveryService.Repositories;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Services
{
    /// <summary>
    /// Репозиторий для доступа к Клиентам в базе данных
    /// </summary>
    public class ClientService
    {
        private readonly ILogger<ClientService> _logger;

        private readonly ClientRepository _clientRepository;


        public ClientService(ILogger<ClientService> logger, ClientRepository clientRepository)
        {
            _logger = logger;
            _clientRepository = clientRepository;
        }


        /// <summary>
        /// Добавление нового клиента в БД
        /// </summary>
        /// <param name="client">Объект клиента</param>
        /// <returns></returns>
        public async Task<bool> AddClientAsync(Client client)
        {
            if (client == null)
            {
                _logger.LogWarning("Попытка добавления клиента с null-объектом.");
                return false;
            }

            _logger.LogInformation("Добавление нового клиента: {ClientName}", client.Name);

            try
            {
                await _clientRepository.AddAsync(client);
                _logger.LogInformation("Клиент {ClientName} успешно добавлен.", client.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении клиента {ClientName}", client.Name);
                return false;
            }
        }

        /// <summary>
        /// Получение клиента по id
        /// </summary>
        /// <param name="userId">ID клиента</param>
        /// <returns>Клиент. Если был не найден то null</returns>
        public async Task<Client?> GetClientByIdAsync(int userId)
        {
            _logger.LogDebug("Запрос клиента по ID: {UserId}", userId);
            try
            {
                var client = await _clientRepository.GetByIdAsync(userId);

                if (client == null)
                {
                    _logger.LogWarning("Клиент с ID {UserId} не найден.", userId);
                    return null;
                }
                
                _logger.LogDebug("Клиент {UserId} найден: {ClientName}", userId, client.Name);
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении клиента по ID {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Получение клиента по Name
        /// </summary>
        /// <param name="name">Логин клиента</param>
        /// <returns>Клиент. Если был не найден то null</returns>
        public async Task<Client?> GetClientByNameAsync(string name)
        {
            _logger.LogDebug("Запрос клиента по Name: {name}", name);

            try
            {
                Client? client = await _clientRepository.GetByNameAsync(name);

                if (client == null)
                {
                    _logger.LogWarning("Клиент с таким {name} не найден.", name);
                    return null;
                }

                _logger.LogDebug("Клиент найден: {ClientName}", client.Name);
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении клиента по Name {name}", name);
                return null;
            }
        }
    }
}