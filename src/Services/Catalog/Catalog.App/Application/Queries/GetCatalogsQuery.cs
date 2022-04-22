using ECom.Services.Catalog.App.Application.DTOs;
using MediatR;

namespace ECom.Services.Catalog.App.Application.Queries
{
    public class GetCatalogsQuery : IRequest<IEnumerable<CatalogDTO>>
    {
    }
}
