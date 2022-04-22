using ECom.BuildingBlocks.SharedKernel.Interfaces;
using ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate.Rings.UpdateProductAvaibleStock;

namespace ECom.Services.Catalog.App.Application.RingHandlers.UpdateProductAvaibleStock
{
    public class UpdateProductAvaibleStockReplyEventHandler : IRingHandler<UpdateProductAvaibleStockReplyEvent>
    {
        public void OnEvent(UpdateProductAvaibleStockReplyEvent data, long sequence, bool endOfBatch)
        {
            throw new NotImplementedException();
        }
    }
}
