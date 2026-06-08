using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DeliveryService.Utils
{
    /// <summary>
    /// Класс настройки Serilog проекта
    /// </summary>
    public static class SerilogConfigurator
    {
        /// <summary>
        /// Шаблон логов
        /// </summary>
        private static readonly string OUTPUT_TEMPLATE
                = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] - {Message:lj}{NewLine}{Exception}";
        /// <summary>
        /// Название папки где хранятся логи
        /// </summary>
        private static readonly string LOGS_DIRECTORY = "logs";
        /// <summary>
        /// Название файла
        /// </summary>
        private static readonly string LOGFILE_NAME = "app.log";
        /// <summary>
        /// Максимальный размер файла логов
        /// </summary>
        private static readonly int LOGFILE_MAX_SIZE = 10 * 1024 * 1024;
        /// <summary>
        /// Лимит файлов в папке
        /// </summary>
        private static readonly int LOGFILE_COUNT_LIMIT = 5;


        /// <summary>
        /// Настройка и установка логгера в контейнер
        /// </summary>
        /// <param name="services">Контейнер модулей проекта</param>
        /// <param name="configuration">Конфигурация проекта</param>
        public static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOGS_DIRECTORY, LOGFILE_NAME);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
                .WriteTo.Async(a => a.File(
                    path: logFilePath,
                    fileSizeLimitBytes: LOGFILE_MAX_SIZE,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: LOGFILE_COUNT_LIMIT,
                    outputTemplate: OUTPUT_TEMPLATE,
                    shared: true
                ))
                .CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        }
    }
}