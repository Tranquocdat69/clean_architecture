using ECom.Services.Balance.App.Application.Commands;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using System.Text.Json;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class DeserializeHandler : IRingHandler<UpdateCreditLimitEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DeserializeHandler> _logger;
        private readonly int _handlerId;
        private readonly IMediator _mediator;

        public DeserializeHandler(IUserRepository userRepository, ILogger<DeserializeHandler> logger, int handlerId, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _logger = logger;
            _handlerId = handlerId;
        }

        public void OnEvent(UpdateCreditLimitEvent data, long sequence, bool endOfBatch)
        {
            if (data.DeserializeHandlerId == _handlerId)
            {
                UpdateCreditLimitCommand updateCreditLimitCommand = new();
                updateCreditLimitCommand = JsonSerializer.Deserialize<UpdateCreditLimitCommand>(data.UpdateCreditLimitCommandString);
                updateCreditLimitCommand.Offset = data.Offset;
                updateCreditLimitCommand.IsCompensatedMessage = data.IsCompensatedMessage;
                updateCreditLimitCommand.RequestId = data.RequestId;
                updateCreditLimitCommand.SequenceRing = data.SequenceRing;

                _mediator.Send(updateCreditLimitCommand);

                _logger.LogInformation(data.ToString());
            }
        }
    }
}
