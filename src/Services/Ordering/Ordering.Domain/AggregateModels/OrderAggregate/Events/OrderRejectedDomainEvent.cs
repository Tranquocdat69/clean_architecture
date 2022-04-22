namespace ECom.Services.Ordering.Domain.AggregateModels.OrderAggregate.Events
{
    public class OrderRejectedDomainEvent : BaseDomainEvent
    {
        public int CustomerId { get; private set; }
        public decimal TotalCost { get; private set; }
        public IDictionary<int,int> Items { get; private set; }
        public string CompensionTopic { get; private set; }

        public OrderRejectedDomainEvent(int customerId, decimal totalCost, IDictionary<int, int> items, string compensionTopic)
        {
            CustomerId = customerId;
            TotalCost = totalCost;
            Items = items;
            CompensionTopic = compensionTopic;
        }
    }
}
