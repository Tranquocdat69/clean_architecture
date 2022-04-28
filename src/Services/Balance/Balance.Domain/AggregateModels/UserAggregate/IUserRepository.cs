using FPTS.FIT.BDRD.BuildingBlocks.SharedKernel.Interfaces;

namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate
{
    public interface IUserRepository : IKeyValuePairRepository<User, int>
    {
    }
}
