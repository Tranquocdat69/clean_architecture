using ECom.Services.Ordering.App.Application.RingHandlers.CreateOrder;

namespace ECom.Services.Ordering.App.Extensions
{
    public static class RegisterRingBufferExtensions
    {
        public static IServiceCollection AddOrderRingBuffer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();
                var disruptor = new Disruptor.Dsl.Disruptor<CreateOrderEvent>(() => new CreateOrderEvent(), 2048, TaskScheduler.Current, producerType: ProducerType.Multi, new BlockingWaitStrategy());
                var commandTopics = configuration.GetSection("Kafka")?.GetSection("CommandTopic");//["Balance"] ?? "balance-command-topic"
                var producer = sp.GetRequiredService<KafkaProducer<string,string>>();
                var replyAddress = configuration["ExternalAddress"];
                var balanceCommandTopic = commandTopics?["Balance"] ?? "balance-command-topic";
                var catalogCommandTopic = commandTopics?["Catalog"] ?? "catalog-command-topic";
                disruptor.HandleEventsWith(
                    new BalanceIntegrationHandler(producer, replyAddress, balanceCommandTopic), 
                    new CatalogIntegrationHandler(producer, replyAddress, catalogCommandTopic));
                return disruptor.Start();
            });

            return services;
        }
    }
}
