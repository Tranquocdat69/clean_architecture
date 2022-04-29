using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events;
using FPTS.FIT.BDRD.BuildingBlocks.SharedKernel.Extensions;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class BusinessHandler : IRingHandler<UpdateCreditLimitEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public BusinessHandler(IUserRepository userRepository, IMediator mediator)
        {
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public void OnEvent(UpdateCreditLimitEvent data, long sequence, bool endOfBatch)
        {
            string message = "";
            bool isSuccess = false;

            var currentUser = _userRepository.GetT(data.UserId);
            if (currentUser is not null)
            {
                decimal oldCreditLimit = currentUser.CreditLimit;
                decimal newCreditLimit = oldCreditLimit - data.TotalCost;

                if (newCreditLimit >= 0)
                {
                    currentUser.DecreaseCash(data.TotalCost);
                    var @event = new DecreaseCreditLimitDomainEvent(
                        offset: data.Offset,
                        userId: data.UserId,
                        creditLimit: newCreditLimit,
                        serializeHandlerId: 1);
                    currentUser.AddDomainEvent(@event);
                    _mediator.DispatchDomainEventsAsync(currentUser);
                    //_mediator.Publish(@event);

                    isSuccess = true;
                    message = "Success";
                }
                else
                {
                    message = "Not enough credit limit";
                }
            }
            else
            {
                message = "User does not exist";
            }

            if (!data.IsCompensatedMessage)
            {
                var @event = new ReplyMessageDomainEvent(
                    userId: data.UserId,
                    replyAddress: data.ReplyAddress,
                    requestId: data.RequestId,
                    isSuccess: isSuccess,
                    message: message);
                _mediator.Publish(@event);
            }
        }
    }
}
