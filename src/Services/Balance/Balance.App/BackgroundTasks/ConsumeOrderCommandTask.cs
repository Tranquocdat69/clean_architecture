namespace ECom.Services.Balance.App.BackgroundTasks
{
    public class ConsumeOrderCommandTask : BackgroundService
    {
        private readonly RingBuffer<UpdateCreditLimitEvent> _inputRing;
        private readonly ISubcriber<ConsumerData<string, string>> _consumer;
        private readonly IConfiguration _configuration;
        private readonly string _balanceCommandTopic;
        private const int PartitionID = 0;
        private int _currentDesializeHandler = 1;
        private int _numberOfDeserializeHandlers = 1;

        public ConsumeOrderCommandTask(RingBuffer<UpdateCreditLimitEvent> inputRing, ISubcriber<ConsumerData<string, string>> consumer, IConfiguration configuration)
        {
            _inputRing = inputRing;
            _consumer = consumer;
            _configuration = configuration;
            _balanceCommandTopic = _configuration.GetSection("Kafka").GetSection("CommandTopic").Value;
            _numberOfDeserializeHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfDeserializeHandlers").Value);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /*while (!stoppingToken.IsCancellationRequested)
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
                            data.RequestId = record.Message.Key.Replace("command", "");
                            data.DeserializeHandlerId = _currentDesializeHandler;
                            data.SequenceRing = sequence;
                            _inputRing.Publish(sequence);

                            _currentDesializeHandler++;

                            if (_currentDesializeHandler > _numberOfDeserializeHandlers)
                            {
                                _currentDesializeHandler = 1;
                            }
                        }
                    }, stoppingToken, _balanceCommandTopic, KafkaOffsetSeed.CurrentCommandTopicOffset, PartitionID);
                });
            }*/
        }
    }
}
