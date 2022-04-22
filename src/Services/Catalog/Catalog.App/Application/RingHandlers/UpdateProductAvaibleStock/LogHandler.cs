using ECom.BuildingBlocks.SharedKernel.Interfaces;
using ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate.Rings.UpdateProductAvaibleStock;

namespace ECom.Services.Catalog.App.Application.RingHandlers.UpdateProductAvaibleStock
{
    public class LogHandler : IRingHandler<UpdateProductAvaibleStockEvent>
    {
        public void OnEvent(UpdateProductAvaibleStockEvent data, long sequence, bool endOfBatch)
        {
            throw new NotImplementedException();
        }
    }
}
