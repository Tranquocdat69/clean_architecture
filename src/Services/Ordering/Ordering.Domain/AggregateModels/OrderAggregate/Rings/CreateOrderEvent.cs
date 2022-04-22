namespace ECom.Services.Ordering.Domain.AggregateModels.OrderAggregate.Rings
#nullable disable
{
    public class CreateOrderEvent : BaseRingEvent
    {
        public decimal TotalCost { get; set; }
        public Dictionary<int, int> Items { get; set; }
        public int UserId { get; set; }
        public string CatalogRequestId { get; set; }
        public string BalanceRequestId { get; set; }
    }   
}
