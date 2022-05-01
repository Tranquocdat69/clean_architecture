using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events.UpdateCreditLimit;
using FPTS.FIT.BDRD.BuildingBlocks.SharedKernel.Extensions;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class BusinessHandler : IRingHandler<UpdateCreditLimitEvent>
    {
        private readonly RingBuffer<UpdateCreditLimitReplyEvent> _ringRelplyBuffer;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly int _numberOfSerializeHandlers;
        private int _currentSerializeHandler = 1;

        public BusinessHandler(
            RingBuffer<UpdateCreditLimitReplyEvent> ringRelplyBuffer, 
            IUserRepository userRepository, 
            IMediator mediator,
            IConfiguration configuration)
        {
            _ringRelplyBuffer = ringRelplyBuffer;
            _userRepository = userRepository;
            _mediator = mediator;
            _numberOfSerializeHandlers = Int32.Parse(configuration.GetSection("Disruptor").GetSection("NumberOfSerializeHandlers").Value);
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
                    data.SerializeHandlerId = _currentSerializeHandler;
                    _currentSerializeHandler++;
                    if (_currentSerializeHandler > _numberOfSerializeHandlers)
                    {
                        _currentSerializeHandler = 1;
                    }

                    currentUser.DecreaseCash(data.TotalCost, data);
                    _mediator.DispatchDomainEventsAsync(currentUser);

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
                long sq = 0L;
                sq = _ringRelplyBuffer.Next();
                var replyEvent = _ringRelplyBuffer[sq];
                replyEvent.IsSuccess = isSuccess;
                replyEvent.Message = message;
                replyEvent.UserId = data.UserId;
                replyEvent.ReplyAddress = data.ReplyAddress;
                replyEvent.RequestId = data.RequestId;
                _ringRelplyBuffer.Publish(sq);

            }
        }
    }
}
