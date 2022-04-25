namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Rings.UpdateCreditLimit
{
    public class UpdateCreditLimitPersistentEvent : BaseRingEvent
    {
        public long Offset { get; set; }
        public int UserId { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
