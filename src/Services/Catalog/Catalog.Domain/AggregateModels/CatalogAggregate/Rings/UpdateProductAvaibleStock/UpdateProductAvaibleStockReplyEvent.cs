using ECom.BuildingBlocks.SharedKernel;

namespace ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate.Rings.UpdateProductAvaibleStock
#nullable disable
{
    public class UpdateProductAvaibleStockReplyEvent : BaseRingEvent
    {
        public string Message { get; set; }
        public string ReplyAddress { get; set; }
        public bool IsSuccess { get; set; }
    }
}
