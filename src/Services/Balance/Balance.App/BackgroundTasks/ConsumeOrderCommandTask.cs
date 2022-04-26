using Confluent.Kafka;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue;
using ECom.Services.Balance.App.Application.Commands;
using ECom.Services.Balance.App.Application.RingHandlers;
using ECom.Services.Balance.Domain.AggregateModels.KafkaOffsetAggregate;

namespace ECom.Services.Balance.App.BackgroundTasks
{
    public class ConsumeOrderCommandTask : BackgroundService
    {
        private readonly RingBuffer<UpdateCreditLimitEvent> _inputRing;
        private readonly KafkaConsumer<string, string> _consumer;
        private readonly IConfiguration _configuration;
        private readonly string _balanceCommandTopic;
        private const int PartitionID = 0;

        public ConsumeOrderCommandTask(RingBuffer<UpdateCreditLimitEvent> inputRing, KafkaConsumer<string, string> consumer, IConfiguration configuration)
        {
            _inputRing = inputRing;
            _consumer = consumer;
            _configuration = configuration;
            _balanceCommandTopic = _configuration.GetSection("Kafka").GetSection("CommandTopic").Value;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Factory.StartNew(() =>
                {
                    _consumer.Consume(record =>
                    {
                        if (record != null)
                        {
                            long sequence = _inputRing.Next();

                            UpdateCreditLimitEvent data = _inputRing[sequence];
                            data.Offset = record.Offset.Value;
                            data.IsCompensatedMessage = record.Message.Key.Contains("command") ? false : true;
                            data.UpdateCreditLimitCommandString = record.Message.Value;
                            //data.RequestId = record.Message.Key.Replace("command", "");

                            _inputRing.Publish(sequence);
                        }
                    }, stoppingToken, _balanceCommandTopic, PartitionID, KafkaOffset.CommandOffset);
                });
            }
        }
    }
}
