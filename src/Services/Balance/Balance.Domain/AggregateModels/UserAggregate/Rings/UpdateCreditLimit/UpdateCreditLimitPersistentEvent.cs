namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Rings.UpdateCreditLimit
{
    public class UpdateCreditLimitPersistentEvent : BaseRingEvent
    {
        public long Offset { get; set; }
        public string UserId { get; set; }
        public double Balance { get; set; }
    }
}
