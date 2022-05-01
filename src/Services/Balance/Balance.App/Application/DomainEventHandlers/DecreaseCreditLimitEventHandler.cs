﻿using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events.UpdateCreditLimit;

namespace ECom.Services.Balance.App.Application.DomainEventHandlers
{
    public class DecreaseCreditLimitEventHandler : IDomainEventHandler<DecreaseCreditLimitDomainEvent>
    {
        private readonly RingBuffer<UpdateCreditLimitPersistentEvent> _ringPersistentBuffer;
        private readonly IUserRepository _userRepository;
        private readonly int _numberOfSerializeHandlers;
        private int _currentSerializeHandler = 1;

        public DecreaseCreditLimitEventHandler(RingBuffer<UpdateCreditLimitPersistentEvent> ringPersistentBuffer,
            IUserRepository userRepository, IConfiguration configuration)
        {
            _ringPersistentBuffer = ringPersistentBuffer;
            _userRepository = userRepository;
            _numberOfSerializeHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfSerializeHandlers").Value);
        }

        public Task Handle(DecreaseCreditLimitDomainEvent @event, CancellationToken cancellationToken)
        {
            long sq = 0L;
            sq = _ringPersistentBuffer.Next();
            var persistentEvent = _ringPersistentBuffer[sq];
            persistentEvent.UserId = @event.UserId;
            persistentEvent.CreditLimit = @event.CreditLimit;
            persistentEvent.Offset = @event.Offset;
            persistentEvent.SerializeHandlerId = _currentSerializeHandler;

            _currentSerializeHandler++;
            if (_currentSerializeHandler > _numberOfSerializeHandlers)
            {
                _currentSerializeHandler = 1;
            }

            _ringPersistentBuffer.Publish(sq);

            Console.WriteLine("Current Credit Limit of " + _userRepository.GetT(@event.UserId).Name + " is: " + @event.CreditLimit);

            return Task.CompletedTask;
        }

    }
}
