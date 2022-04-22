namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate
#nullable disable
{
    public class User : BaseEntity, IAggregateRoot
    {
        public User(string name, decimal credit)
        {
            Name = name;
            CreditLimit = credit;
        }
        public string Name { get; private set; }
        public decimal CreditLimit { get; private set; }

        public void DecreaseCash(decimal num)
        {
            this.CreditLimit -= num;
        }

        public void IncreaseCash(decimal num)
        {
            this.CreditLimit += num;
        }
    }
}
