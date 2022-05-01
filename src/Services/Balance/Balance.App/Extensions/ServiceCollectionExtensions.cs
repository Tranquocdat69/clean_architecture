using Confluent.Kafka;
using ECom.Services.Balance.App.Application.Commands;
using ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events;
using ECom.Services.Balance.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ECom.Services.Balance.App.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string DB_CONNECTION_KEY = "BalanceDBSqlServer";

        public static IServiceCollection UseServiceCollectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScopedServices()
                .AddLoggerConfiguration(configuration)
                .AddPersistentConfiguration(configuration)
                .AddKafkaConfiguration(configuration)
                .AddBalanceRingBuffer(configuration)
                .AddMediatorConfiguration()
                .AddConsumeMessageFromTopicKafkaConfiguration();

            return services;
        }
        private static IServiceCollection AddScopedServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
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
                    var loggerConfiguration = configuration.GetSection("KafkaLogger").Get<LoggerKafkaConfiguration>();
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
            services.AddSingleton<ISubcriber<ConsumerData<string, string>>, KafkaSubcriber<string, string>>(sp =>
            {
                ConsumerBuilderConfiguration consumerConfig = new ConsumerBuilderConfiguration()
                {
                    BootstrapServers = configuration.GetSection("Kafka").GetSection("BootstrapServers").Value,
                    EnableAutoCommit = false
                };
                return new KafkaSubcriber<string, string>(consumerConfig);
            });
            services.AddSingleton<IPublisher<ProducerData<string, string>>, KafkaPublisher<string, string>>(sp =>
            {
                ProducerBuilderConfiguration producerConfig = new ProducerBuilderConfiguration()
                {
                    BootstrapServers = configuration.GetSection("Kafka").GetSection("BootstrapServers").Value
                };
                return new KafkaPublisher<string, string>(producerConfig);
            });
            services.AddSingleton<IPublisher<ProducerData<Null, string>>, KafkaPublisher<Null, string>>(sp =>
            {
                ProducerBuilderConfiguration producerConfig = new ProducerBuilderConfiguration()
                {
                    BootstrapServers = configuration.GetSection("Kafka").GetSection("BootstrapServers").Value
                };
                return new KafkaPublisher<Null, string>(producerConfig);
            });

            return services;
        }

        private static IServiceCollection AddBackgroundService(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection AddBalanceRingBuffer(this IServiceCollection services, IConfiguration configuration)
        {
            int inputRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("InputRingSize").Value);
            int persistentRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("PersistentRingSize").Value);
            int replyRingSize = Int32.Parse(configuration.GetSection("Disruptor").GetSection("ReplyRingSize").Value);
            int numberOfDeserializeHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfDeserializeHandlers").Value);
            int numberOfSerializeHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfSerializeHandlers").Value);

            services.AddSingleton(sp =>
            {
                var userRepository = sp.CreateScope().ServiceProvider.GetRequiredService<IUserRepository>();
                var logger = sp.GetRequiredService<ILogger<DeserializeHandler>>();
                var mediator = sp.GetRequiredService<IMediator>();
                //var persistentDisruptor = sp.GetRequiredService<RingBuffer<UpdateCreditLimitPersistentEvent>>();
                var replyDisruptor = sp.GetRequiredService<RingBuffer<UpdateCreditLimitReplyEvent>>();

                var inputDisruptor = new Disruptor<UpdateCreditLimitEvent>(() => new UpdateCreditLimitEvent(), inputRingSize);
                inputDisruptor.HandleEventsWith(GetDeserializeHandlers(mediator, userRepository, logger, numberOfDeserializeHandlers))
                .Then(new BusinessHandler(replyDisruptor, userRepository, mediator));

                return inputDisruptor.Start();
            });
            services.AddSingleton(sp =>
            {
                var producer = sp.GetRequiredService<IPublisher<ProducerData<Null, string>>>();
                var persistentDisruptor = new Disruptor<UpdateCreditLimitPersistentEvent>(() => new UpdateCreditLimitPersistentEvent(), persistentRingSize);

                persistentDisruptor.HandleEventsWith(GetSerializeHandlers(numberOfSerializeHandlers))
                .Then(new PersistentHandler(producer, configuration));

                return persistentDisruptor.Start();
            });
            services.AddSingleton(sp =>
            {
                var userRepository = sp.CreateScope().ServiceProvider.GetRequiredService<IUserRepository>();
                var replyDisruptor = new Disruptor<UpdateCreditLimitReplyEvent>(() => new UpdateCreditLimitReplyEvent(), replyRingSize);

                replyDisruptor.HandleEventsWith(new ReplyHandler(userRepository));

                return replyDisruptor.Start();
            });

            return services;
        }

        private static IServiceCollection AddMediatorConfiguration(this IServiceCollection services)
        {
            services.AddMediatR(
                typeof(UpdateCreditLimitCommand));
            return services;
        }

        private static IServiceCollection AddConsumeMessageFromTopicKafkaConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IKafkaSubcriberService<string, string>, KafkaSubcriberService<string, string>>(sp =>
            {
                ISubcriber<ConsumerData<string, string>> subcriber = sp.GetRequiredService<ISubcriber<ConsumerData<string, string>>>();
                return new KafkaSubcriberService<string, string>(subcriber);
            });

            return services;
        }


        private static DeserializeHandler[] GetDeserializeHandlers(IMediator mediator, IUserRepository userRepository, ILogger<DeserializeHandler> logger, int size)
        {
            DeserializeHandler[] deserializeHandlers = new DeserializeHandler[size];

            for (int i = 0; i < size; i++)
            {
                deserializeHandlers[i] = new DeserializeHandler(userRepository, logger, i + 1, mediator);
            }

            return deserializeHandlers;
        }
        private static SerializeHandler[] GetSerializeHandlers(int size)
        {
            SerializeHandler[] serializeHandlers = new SerializeHandler[size];

            for (int i = 0; i < size; i++)
            {
                serializeHandlers[i] = new SerializeHandler(i + 1);
            }

            return serializeHandlers;
        }
    }
}
