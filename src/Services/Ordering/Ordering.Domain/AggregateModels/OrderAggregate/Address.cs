namespace ECom.Services.Ordering.Domain.AggregateModels.OrderAggregate
#nullable disable
{
    public class Address : ValueObject
    {
        public String Street { get; private set; }
        public String City { get; private set; }

        public Address(string street, string city)
        {
            Street = street;
            City = city;
        }

        public override string ToString()
        {
            return "{\"Street\":\""+Street+"\",\"City\":\""+City+"\"}";
        }
    }
}
