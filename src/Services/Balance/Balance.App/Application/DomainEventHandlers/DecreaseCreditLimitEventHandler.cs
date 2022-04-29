using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events;

namespace ECom.Services.Balance.App.Application.DomainEventHandlers
{
    public class DecreaseCreditLimitEventHandler : IDomainEventHandler<DecreaseCreditLimitDomainEvent>
    {
        private readonly RingBuffer<UpdateCreditLimitPersistentEvent> _ringPersistentBuffer;
        private readonly IUserRepository _userRepository;

        public DecreaseCreditLimitEventHandler(RingBuffer<UpdateCreditLimitPersistentEvent> ringPersistentBuffer, IUserRepository userRepository)
        {
            _ringPersistentBuffer = ringPersistentBuffer;
            _userRepository = userRepository;
        }

        public Task Handle(DecreaseCreditLimitDomainEvent @event, CancellationToken cancellationToken)
        {
            long sq = 0L;
            sq = _ringPersistentBuffer.Next();
            var persistentEvent = _ringPersistentBuffer[sq];
            persistentEvent.UserId = @event.UserId;
            persistentEvent.CreditLimit = @event.CreditLimit;
            persistentEvent.Offset = @event.Offset;
            persistentEvent.SerializeHandlerId = 1;//data.DeserializeHandlerId;
            _ringPersistentBuffer.Publish(sq);

            Console.WriteLine("Current Credit Limit of " + _userRepository.GetT(@event.UserId).Name + " is: " + @event.CreditLimit);

            return Task.CompletedTask;
        }

    }
}
