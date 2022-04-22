namespace ECom.Services.Ordering.Domain.AggregateModels.OrderAggregate;
#nullable disable
public class Order : BaseEntity, IAggregateRoot
{
    public DateTime OrderDate { get; set; }
    public Address Address { get; private set; }
    public int CustomerId { get; set; }
    private readonly List<OrderItem> _orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public Order(int cusId, Address add) : this()
    {
        CustomerId = cusId;
        Address = add;
        OrderDate = DateTime.Now;
    }

    protected Order()
    {
        _orderItems = new List<OrderItem>();
    }

    // DDD Patterns comment
    // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
    // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
    // in order to maintain consistency between the whole Aggregate. 
    public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
    {
        var existingOrderForProduct = _orderItems.Where(o => o.ProductId == productId)
            .SingleOrDefault();

        if (existingOrderForProduct != null)
        {
            //if previous line exist modify it with higher discount  and units..

            if (discount > existingOrderForProduct.GetCurrentDiscount())
            {
                existingOrderForProduct.SetNewDiscount(discount);
            }

            existingOrderForProduct.AddUnits(units);
        }
        else
        {
            //add validated new order item

            var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
            _orderItems.Add(orderItem);
        }
    }

    public void SetOrderConfirmed()
    {
        var @event = new OrderConfirmedDomainEvent(this);
        AddDomainEvent(@event);
    }

    public void SetOrderRejected(string topic)
    {
        var total                        = OrderItems.Sum(x => x.GetUnits() + x.GetUnitPrice());
        IDictionary<int,int> itemSummary = OrderItems.ToDictionary(x => x.ProductId, x => x.GetUnits());
        var @event                       = new OrderRejectedDomainEvent(CustomerId, total, itemSummary, topic);
        AddDomainEvent(@event);
    }
    /// <summary>
    /// {"ProductName":"12","PictureUrl":"12","UnitPrice":1,"Discount":1,"Units":1,"ProductId":1},
    /// {"ProductName":"12","PictureUrl":"12","UnitPrice":1,"Discount":1,"Units":1,"ProductId":1}
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var orderItems = string.Join(",",_orderItems.Select(x => x.ToString()));
        return "{\"OrderDate\":\""+OrderDate.ToString()+"\",\"Address\":"+Address.ToString()+",\"CustomerId\":"+CustomerId+",\"OrderItems\":["+orderItems+"]}";
    }
}
