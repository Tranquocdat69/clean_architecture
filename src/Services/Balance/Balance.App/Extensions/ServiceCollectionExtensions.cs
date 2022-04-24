﻿using Confluent.Kafka;
using ECom.BuildingBlocks.LogLib.KafkaLogger;
using ECom.BuildingBlocks.LogLib.KafkaLogger.Configs;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue.Configs;
using ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit;
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
                .AddBackgroundService()
                .AddBalanceRingBuffer(configuration);

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
            services.AddDbContext<UserDbContext>(options =>
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
            services.AddSingleton(sp =>
            {
                KafkaConsumerConfig consumerConfig = new KafkaConsumerConfig()
                {
                    BootstrapServers = configuration.GetSection("Kafka").GetSection("BootstrapServers").Value,
                    EnableAutoCommit = false
                };
                return new KafkaConsumer<string, string>(consumerConfig);
            });
            services.AddSingleton(sp =>
            {
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

        private static IServiceCollection AddBalanceRingBuffer(this IServiceCollection services, IConfiguration configuration)
        {
            int inputRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("InputRingSize").Value);
            int persistentRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("PersistentRingSize").Value);
            int replyRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("ReplyRingSize").Value);
            int numberOfLogHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfLogHandlers").Value);
            int numberOfReplyHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfReplyHandlers").Value);

            services.AddSingleton(sp =>
            {
                //var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<LogHandler>();
                var logger = sp.GetRequiredService<ILogger<LogHandler>>();
                var inputDisruptor = new Disruptor<UpdateCreditLimitEvent>(() => new UpdateCreditLimitEvent(), inputRingSize);
                inputDisruptor.HandleEventsWith(GetLogHandlers(logger, numberOfLogHandlers))
                .Then(new BusinessHandler());

                return inputDisruptor.Start();
            });

            services.AddSingleton(sp =>
            {
                var persistentDisruptor = new Disruptor<UpdateCreditLimitPersistentEvent>(() => new UpdateCreditLimitPersistentEvent(), persistentRingSize);
                return persistentDisruptor.Start();
            });

            services.AddSingleton(sp =>
            {
                var replyDisruptor = new Disruptor<UpdateCreditLimitReplyEvent>(() => new UpdateCreditLimitReplyEvent(), replyRingSize);
                replyDisruptor.HandleEventsWith(GetReplyHandlers(numberOfReplyHandlers));

                return replyDisruptor.Start();
            });

            return services;
        }
        private static LogHandler[] GetLogHandlers(ILogger<LogHandler> logger, int size)
        {
            LogHandler[] logHandlers = new LogHandler[size];

            for (int i = 0; i < size; i++)
            {
                logHandlers[i] = new LogHandler(logger, i + 1);
            }

            return logHandlers;
        }
        private static IntegrationReplyHandler[] GetReplyHandlers(int size)
        {
            IntegrationReplyHandler[] replyHandlers = new IntegrationReplyHandler[size];

            for (int i = 0; i < size; i++)
            {
                replyHandlers[i] = new IntegrationReplyHandler(i + 1);
            }

            return replyHandlers;
        }
    }
}
