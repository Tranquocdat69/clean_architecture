using Confluent.Kafka;
using ECom.BuildingBlocks.LogLib.KafkaLogger;
using ECom.BuildingBlocks.LogLib.KafkaLogger.Configs;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue.Configs;
using ECom.Services.Balance.App.BackgroundTasks;
using ECom.Services.Balance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ECom.Services.Balance.App.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string DB_CONNECTION_KEY = "BalanceDB";

        public static IServiceCollection UseServiceCollectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLoggerConfiguration(configuration)
                .AddPersistentConfiguration(configuration)
                .AddKafkaConfiguration(configuration)
                .AddBackgroundService();

            return services;
        }

        private static IServiceCollection AddLoggerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            bool isAllowedConsoleLog = configuration.GetValue("AllowConsoleLog", false);
            services.AddLogging(config =>
            {
                if (!isAllowedConsoleLog)
                {
                    config.ClearProviders();
                }

                config.AddKafkaLogger(configKL =>
                {
                    var loggerConfiguration = configuration.GetSection("KafkaLogger").Get<KafkaLoggerConfiguration>();
                    configKL.BootstrapServers = loggerConfiguration.BootstrapServers;
                    configKL.Targets = loggerConfiguration.Targets;
                    configKL.Rules = loggerConfiguration.Rules;
                });
            });

            return services;
        }

        private static IServiceCollection AddPersistentConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(DB_CONNECTION_KEY);
            services.AddDbContext<CustomerDbContext>(options =>
            {
                if (String.IsNullOrEmpty(connectionString))
                {
                    options.UseInMemoryDatabase(DB_CONNECTION_KEY);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }
            });

            return services;
        }

        private static IServiceCollection AddKafkaConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp => {
                KafkaConsumerConfig consumerConfig = new KafkaConsumerConfig()
                {
                    BootstrapServers = configuration.GetSection("Kafka").GetSection("BootstrapServers").Value,
                    EnableAutoCommit = false
                };
                return new KafkaConsumer<string, string>(consumerConfig);
            });
            services.AddSingleton(sp => {
                KafkaProducerConfig producerConfig = new KafkaProducerConfig()
                {
                    BootstrapServers = configuration.GetSection("Kafka").GetSection("BootstrapServers").Value
                };
                return new KafkaProducer<string, string>(producerConfig);
            });

            return services;
        }

        private static IServiceCollection AddBackgroundService(this IServiceCollection services)
        {
            services.AddHostedService<ConsumeOrderCommandTask>();

            return services;
        }
    }
}
