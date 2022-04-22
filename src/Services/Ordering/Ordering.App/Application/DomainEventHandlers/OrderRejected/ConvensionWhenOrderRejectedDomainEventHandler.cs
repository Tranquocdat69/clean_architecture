namespace ECom.Services.Ordering.App.Application.DomainEventHandlers.OrderRejected
{
    public class ConvensionWhenOrderRejectedDomainEventHandler : IDomainEventHandler<OrderRejectedDomainEvent>
    {
        private readonly KafkaProducer<string, string> _kafkaProducer;
        private const string KEY_COMMAND = "convension";

        public ConvensionWhenOrderRejectedDomainEventHandler(KafkaProducer<string,string> kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }
        public Task Handle(OrderRejectedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(notification.CompensionTopic))
            {
                IIntegration integration;
                switch (notification.CompensionTopic)
                {
                    case "Balance":
                        integration = new UpdateCreditLimitIntegration(
                            userId: notification.CustomerId,
                            totalCost: notification.TotalCost,
                            replyAddress: ""
                        );
                        break;
                    case "Catalog":
                        integration = new UpdateProductAvaibleStockIntegration(
                            items: notification.Items,
                            replyAddress: ""
                        );
                        break;
                    default:
                        throw new OrderingDomainException("Undefined arrgument "+nameof(notification.CompensionTopic));
                }
                _kafkaProducer.Produce(new Message<string, string> { Value = integration.ToString(), Key = KEY_COMMAND }, notification.CompensionTopic);

            }
            return Task.CompletedTask;
        }
    }
}
