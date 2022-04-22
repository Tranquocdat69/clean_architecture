namespace ECom.Services.Ordering.Domain.AggregateModels.OrderAggregate.Events
{
    public class OrderConfirmedDomainEvent : BaseDomainEvent
    {
        public Order Order { get; set; }

        public OrderConfirmedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}
