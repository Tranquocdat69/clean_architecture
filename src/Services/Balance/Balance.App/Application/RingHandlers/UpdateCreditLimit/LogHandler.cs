using ECom.Services.Balance.App.Application.Commands;
using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using System.Text.Json;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class LogHandler : IRingHandler<UpdateCreditLimitEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LogHandler> _logger;
        private readonly int _handlerId;
        private readonly IMediator _mediator;

        public LogHandler(IUserRepository userRepository, ILogger<LogHandler> logger, int handlerId, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _logger = logger;
            _handlerId = handlerId;
        }

        public void OnEvent(UpdateCreditLimitEvent data, long sequence, bool endOfBatch)
        {
            if (_userRepository.Exist(data.UserId) && _handlerId == _userRepository.GetT(data.UserId).LogHandlerId)
            {
                UpdateCreditLimitCommand updateCreditLimitCommand = new();
                updateCreditLimitCommand = JsonSerializer.Deserialize<UpdateCreditLimitCommand>(data.UpdateCreditLimitCommandString);
                updateCreditLimitCommand.Offset = data.Offset;
                updateCreditLimitCommand.IsCompensatedMessage = data.IsCompensatedMessage;
                //updateCreditLimitCommand.RequestId = data.RequestId;

                _mediator.Send(updateCreditLimitCommand);

                _logger.LogInformation(data.ToString());
            }
        }
    }
}
