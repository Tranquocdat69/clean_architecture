using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events;

namespace ECom.Services.Balance.App.Application.DomainEventHandlers
{
    public class TestHandler 
    {
        private readonly IUserRepository _userRepository;

        public TestHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task Handle(DecreaseCreditLimitDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
