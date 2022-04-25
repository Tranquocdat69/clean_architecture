using Confluent.Kafka;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue;
using ECom.Services.Balance.App.Application.Commands;
using ECom.Services.Balance.App.Application.RingHandlers;

namespace ECom.Services.Balance.App.BackgroundTasks
{
    public class ConsumeOrderCommandTask : BackgroundService
    {
        private readonly KafkaConsumer<string, string> _consumer;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly string BalanceCommandTopic = "";

        public ConsumeOrderCommandTask(RingBuffer<UpdateCreditLimitEvent> inputRing, KafkaConsumer<string, string> consumer, IConfiguration configuration, IMediator mediator)
        {
            _consumer = consumer;
            _configuration = configuration;
            BalanceCommandTopic = _configuration.GetSection("Kafka").GetSection("CommandTopic").Value;
            _mediator = mediator;
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
                            UpdateCreditLimitCommand updateCreditLimitCommand = new UpdateCreditLimitCommand().FromString(record.Message.Value);
                            updateCreditLimitCommand.IsCompensatedMessage = record.Message.Key.Contains("command") ? false : true;
                            updateCreditLimitCommand.Offset = record.Offset.Value;

                            _mediator.Send(updateCreditLimitCommand);
                        }
                    }, stoppingToken, BalanceCommandTopic, 0);
                });
            }
        }
    }
}
