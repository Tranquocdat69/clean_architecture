namespace ECom.Services.Ordering.App.Application.DomainEventHandlers.OrderConfirmed
{
    public class PersistentWhenOrderConfirmedDomainEventHandler : IDomainEventHandler<OrderConfirmedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;

        public PersistentWhenOrderConfirmedDomainEventHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task Handle(OrderConfirmedDomainEvent notification, CancellationToken cancellationToken)
        {
            _orderRepository.Add(notification.Order);
            await _orderRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
