using Confluent.Kafka;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue;
using ECom.Services.Balance.App.Application.RingHandlers;

namespace ECom.Services.Balance.App.BackgroundTasks
{
    public class ConsumeOrderCommandTask : BackgroundService
    {
        private readonly RingBuffer<UpdateCreditLimitEvent> _inputRing;
        private readonly KafkaConsumer<string, string> _consumer;
        private readonly IConfiguration _configuration;


        public ConsumeOrderCommandTask(RingBuffer<UpdateCreditLimitEvent> inputRing, KafkaConsumer<string, string> consumer, IConfiguration configuration)
        {
            _inputRing = inputRing;
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
                            var sequence = _inputRing.Next();
                            var data = _inputRing[sequence];
                            data.Offset = record.Offset.Value;
                            Console.WriteLine(record.Message.Value);

                            _inputRing.Publish(sequence);
                        }
                    }, stoppingToken, _configuration.GetSection("Kafka").GetSection("CommandTopic").Value, 0);
                });
            }
        }
    }
}
