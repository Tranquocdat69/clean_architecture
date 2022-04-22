using ECom.BuildingBlocks.SharedKernel.Interfaces;
using ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate.Rings.UpdateProductAvaibleStock;

namespace ECom.Services.Catalog.App.Application.RingHandlers.UpdateProductAvaibleStock
{
    public class UpdateProductAvaibleStockPersistentEventHandler : IRingHandler<UpdateProductAvaibleStockPersistentEvent>
    {
        public void OnEvent(UpdateProductAvaibleStockPersistentEvent data, long sequence, bool endOfBatch)
        {
            throw new NotImplementedException();
        }
    }
}
