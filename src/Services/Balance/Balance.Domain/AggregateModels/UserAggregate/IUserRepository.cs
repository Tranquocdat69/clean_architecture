namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        void Update(User Balance);
        User Get(int id);
    }
}
