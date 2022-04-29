using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events;

namespace ECom.Services.Balance.App.Application.DomainEventHandlers
{
    public class ReplyMessageDomainEventHandler : IDomainEventHandler<ReplyMessageDomainEvent>
    {
        private readonly RingBuffer<UpdateCreditLimitReplyEvent> _ringRelplyBuffer;

        public ReplyMessageDomainEventHandler(RingBuffer<UpdateCreditLimitReplyEvent> ringRelplyBuffer)
        {
            _ringRelplyBuffer = ringRelplyBuffer;
        }

        public Task Handle(ReplyMessageDomainEvent @event, CancellationToken cancellationToken)
        {
            long sq = 0L;
            sq = _ringRelplyBuffer.Next();
            var replyEvent = _ringRelplyBuffer[sq];
            replyEvent.IsSuccess = @event.IsSuccess;
            replyEvent.Message = @event.Message;
            replyEvent.UserId = @event.UserId;
            replyEvent.ReplyAddress = @event.ReplyAddress;
            replyEvent.RequestId = @event.RequestId;
            _ringRelplyBuffer.Publish(sq);

            return Task.CompletedTask;
        }
    }
}
