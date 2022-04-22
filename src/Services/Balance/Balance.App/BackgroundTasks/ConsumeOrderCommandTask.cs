using Confluent.Kafka;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue;

namespace ECom.Services.Balance.App.BackgroundTasks
{
    public class ConsumeOrderCommandTask : BackgroundService
    {
        //private readonly string _balanceTopic = "balance-command-topic";
        private readonly KafkaConsumer<string, string> _consumer;
        private readonly IConfiguration _configuration;

        public ConsumeOrderCommandTask(KafkaConsumer<string, string> consumer, IConfiguration configuration)
        {
            _consumer = consumer;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    _consumer.Consume(record =>
                    {
                        if (record != null)
                        {
                            Console.WriteLine(record);
                        }
                    }, stoppingToken, _configuration.GetSection("Kafka").GetSection("CommandTopic").Value);
                });
            }
        }
    }
}
