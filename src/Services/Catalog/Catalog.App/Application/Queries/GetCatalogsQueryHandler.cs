using ECom.Services.Catalog.App.Application.DTOs;
using MediatR;

namespace ECom.Services.Catalog.App.Application.Queries
{
    public class GetCatalogsQueryHandler : IRequestHandler<GetCatalogsQuery, IEnumerable<CatalogDTO>>
    {
        public Task<IEnumerable<CatalogDTO>> Handle(GetCatalogsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
