using Confluent.Kafka;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class PersistentHandler : IRingHandler<UpdateCreditLimitPersistentEvent>
    {
        private readonly IPublisher<ProducerData<Null, string>> _producer;
        private readonly string _balancePersistentTopic;
        private Dictionary<int, string> _dicJsondata = new();
        private const int MaxBatchSize = 24;
        private const int PartitionId = 0;

        public PersistentHandler(IPublisher<ProducerData<Null, string>> producer, IConfiguration configuration)
        {
            _producer = producer;
            _balancePersistentTopic = configuration.GetSection("Kafka").GetSection("PersistentTopic").Value;
        }
        public void OnEvent(UpdateCreditLimitPersistentEvent data, long sequence, bool endOfBatch)
        {
            if (_dicJsondata.Count < MaxBatchSize)
            {
                if (_dicJsondata.ContainsKey(data.UserId))
                {
                    _dicJsondata[data.UserId] = data.UpdateCreditLimitPersistentEventString;
                }
                else
                {
                    _dicJsondata.Add(data.UserId, data.UpdateCreditLimitPersistentEventString);
                }
            }

            if (_dicJsondata.Count == MaxBatchSize || endOfBatch)
            {
                string _valueMessage = data.Offset.ToString();
                foreach (var item in _dicJsondata)
                {
                    _valueMessage += "|" + item.Value;
                }
                var producerData = new ProducerData<Null, string>(_valueMessage, null, _balancePersistentTopic, PartitionId);
                _producer.Publish(producerData);

                _dicJsondata.Clear();
            }
        }
    }
}
