using MediatR;

namespace ECom.Services.Catalog.App.Application.Commands
{
    public class UpdateProductAvaibleStockCommandHandler : IRequestHandler<UpdateProductAvaibleStockCommand>
    {
        public Task<Unit> Handle(UpdateProductAvaibleStockCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
