namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate
{
    public interface IUserRepository : IKeyValuePairRepository<InMemoryUser, int>
    {
    }
}
