using MediatR;

namespace ECom.Services.Catalog.App.Application.Commands
#nullable disable
{
    public class UpdateProductAvaibleStockCommand : IRequest
    {
        public List<int> ProductIds { get; private set; }
        public string RequestId { get; private set; }
        public string ReplyAddress { get; private set; }
    }
}
