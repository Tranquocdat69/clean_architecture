namespace ECom.Services.Ordering.Domain.AggregateModels.OrderAggregate
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        bool Add(Order order);
    }
}
