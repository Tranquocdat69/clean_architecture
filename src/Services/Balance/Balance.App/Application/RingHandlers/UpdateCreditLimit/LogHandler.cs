using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class LogHandler : IRingHandler<UpdateCreditLimitEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<LogHandler> _logger;
        private readonly int _handlerId;

        public LogHandler(IUserRepository userRepository, ILogger<LogHandler> logger, int handlerId)
        {
            _userRepository = userRepository;
            _logger = logger;
            _handlerId = handlerId;
        }

        public void OnEvent(UpdateCreditLimitEvent data, long sequence, bool endOfBatch)
        {
            if (_userRepository.Exist(data.UserId) && _handlerId == _userRepository.GetT(data.UserId).LogHandlerId)
            {
                _logger.LogInformation(data.ToString());
            }
        }
    }
}
