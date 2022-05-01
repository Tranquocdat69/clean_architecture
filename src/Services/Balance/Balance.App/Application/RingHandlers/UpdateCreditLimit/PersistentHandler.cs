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
        private int _currentBatchSize = 0;
        private int _handlerId1 = 0;
        private int _handlerId2 = 0;
        private int _handlerId3 = 0;
        private int _handlerId4 = 0;
        private int _handlerId5 = 0;
        private int _handlerId6 = 0;

        public PersistentHandler(IPublisher<ProducerData<Null, string>> producer, IConfiguration configuration)
        {
            _producer = producer;
            _balancePersistentTopic = configuration.GetSection("Kafka").GetSection("PersistentTopic").Value;
        }
        public void OnEvent(UpdateCreditLimitPersistentEvent data, long sequence, bool endOfBatch)
        {
            switch (data.SerializeHandlerId)
            {
                case 1:
                    _handlerId1++;
                    break;
                case 2:
                    _handlerId2++;
                    break;
                case 3:
                    _handlerId3++;
                    break;
                case 4:
                    _handlerId4++;
                    break;
                case 5:
                    _handlerId5++;
                    break;
                case 6:
                    _handlerId6++;
                    break;
                default:
                    break;
            }

            if (_dicJsondata.ContainsKey(data.UserId))
            {
                _dicJsondata[data.UserId] = data.UpdateCreditLimitPersistentEventString;
            }
            else
            {
                _currentBatchSize++;
                _dicJsondata.Add(data.UserId, data.UpdateCreditLimitPersistentEventString);
            }

            if (_currentBatchSize == MaxBatchSize || endOfBatch)
            {
                string _valueMessage = data.Offset.ToString();
                foreach (var item in _dicJsondata)
                {
                    _valueMessage += "|" + item.Value;
                }
                var producerData = new ProducerData<Null, string>(_valueMessage, null, _balancePersistentTopic, PartitionId);
                _producer.Publish(producerData);

                _currentBatchSize = 0;
                _dicJsondata.Clear();
            }

            Console.WriteLine("number of handler1: " + _handlerId1);
            Console.WriteLine("number of handler2: " + _handlerId2);
            Console.WriteLine("number of handler3: " + _handlerId3);
            Console.WriteLine("number of handler4: " + _handlerId4);
            Console.WriteLine("number of handler5: " + _handlerId5);
            Console.WriteLine("number of handler6: " + _handlerId6);

        }
    }
}
