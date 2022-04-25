namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Rings.UpdateCreditLimit
{
    public class UpdateCreditLimitReplyEvent : BaseRingEvent
    {
        public string ReplyAddress { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }

        public override string ToString()
        {
            return "{\"IsSuccess\":" + IsSuccess + ",\"Message\":\"" + Message
            + "\",\"ReplyAddress\":\"" + ReplyAddress + "\",\"RequestId\":\"" + RequestId + "\"}";
        }
    }
}
