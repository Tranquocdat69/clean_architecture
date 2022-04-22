namespace ECom.Services.Ordering.App.Application.RingHandlers.CreateOrder
{
    public class CatalogIntegrationHandler : IRingHandler<CreateOrderEvent>
    {
        private readonly KafkaProducer<string, string> _kafkaProducer;
        private readonly string _replyAddress;
        private readonly string _topic;
        private const string KEY_COMMAND = "command";

        public CatalogIntegrationHandler(KafkaProducer<string, string> kafkaProducer, string replyAddress, string topic)
        {
            _kafkaProducer     = kafkaProducer;
            _replyAddress      = replyAddress;
            _topic             = topic;
        }

        public void OnEvent(CreateOrderEvent data, long sequence, bool endOfBatch)
        {
            var integration = new UpdateProductAvaibleStockIntegration(
                items: data.Items,
                replyAddress: _replyAddress
                );
            _kafkaProducer.Produce(new Message<string, string> { Value = integration.ToString() , Key = KEY_COMMAND+data.BalanceRequestId}, _topic);
        }
    }
}
