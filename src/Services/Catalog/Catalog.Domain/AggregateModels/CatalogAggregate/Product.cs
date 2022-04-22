using ECom.BuildingBlocks.SharedKernel;

namespace ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate
#nullable disable
{
    public class Product : BaseEntity
    {
        public Product(int id, string name, int quantity, double price)
        {
            Id = id;
            Name = name;
            AvailableStock = quantity;
            Price = price;
        }
        public string Name { get; private set; }
        public int AvailableStock { get; private set; }
        public double Price { get; private set; }

        public void UpdateStock(int quantity)
        {
            AvailableStock += quantity;
        }
    }
}
