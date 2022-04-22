using ECom.Services.Catalog.Domain.AggregateModels.CatalogAggregate;

namespace ECom.Services.Catalog.Infrastructure
#nullable disable
{
    public class CatalogRepository : ICatalogRepository
    {
        private static IDictionary<int, CatalogType> s_dataStore;
        public CatalogRepository()
        {
            s_dataStore = s_dataStore ?? new Dictionary<int, CatalogType>();
        }
        public void Add(int id, CatalogType t)
        {
            s_dataStore.Add(id, t);
        }

        public bool Exist(int id)
        {
            return s_dataStore.TryGetValue(id, out CatalogType t);
        }

        public CatalogType GetT(int id)
        {
            return s_dataStore[id];
        }
    }
}
