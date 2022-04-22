using ECom.BuildingBlocks.SharedKernel;

namespace ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate.Rings.UpdateProductAvaibleStock
#nullable disable
{
    public class UpdateProductAvaibleStockEvent : BaseRingEvent
    {
        public IEnumerable<int> ProductIds { get; set; }
        public string ReplyAddress { get; set; }
    }
}
