namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events
{
    public class ReplyMessageDomainEvent : BaseDomainEvent
    {
        public int UserId { get; set; }
        public string ReplyAddress { get; set; }
        public string RequestId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public ReplyMessageDomainEvent(int userId, string replyAddress, string requestId, bool isSuccess, string message)
        {
            UserId = userId;
            ReplyAddress = replyAddress;
            RequestId = requestId;
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
