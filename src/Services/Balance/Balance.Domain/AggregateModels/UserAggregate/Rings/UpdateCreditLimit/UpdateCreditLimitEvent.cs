namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Rings.UpdateCreditLimit
{
    public class UpdateCreditLimitEvent : BaseRingEvent
    {
        public int UserId { get; set; }
        public decimal TotalCost { get; set; }
        public string ReplyAddress { get; set; }
        public long Offset { get; set; }
        public bool IsCompensatedMessage { get; set; }
        public string UpdateCreditLimitCommandString { get; set; }
    }
}
