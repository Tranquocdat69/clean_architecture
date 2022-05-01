
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events.UpdateCreditLimit;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Rings.UpdateCreditLimit;
using FPTS.FIT.BDRD.BuildingBlocks.SharedKernel;
using FPTS.FIT.BDRD.BuildingBlocks.SharedKernel.Interfaces;

namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate
#nullable disable
{
    public class User : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public decimal CreditLimit { get; private set; }

        public User(string name, decimal creditLimit)
        {
            Name = name;
            CreditLimit = creditLimit;
        }

        public void DecreaseCash(decimal num, UpdateCreditLimitEvent ringEventData)
        {
            this.CreditLimit -= num;
            var @event = new DecreaseCreditLimitDomainEvent(
                        offset: ringEventData.Offset,
                        userId: ringEventData.UserId,
                        creditLimit: this.CreditLimit,
                        serializeHandlerId: ringEventData.SerializeHandlerId);
            AddDomainEvent(@event);
        }

        public void IncreaseCash(decimal num)
        {
            this.CreditLimit += num;
        }
    }
}
