using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;
using NetMQ.Sockets;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class IntegrationReplyHandler : IRingHandler<UpdateCreditLimitReplyEvent>
    {
        private readonly int _handlerId;
        private IDictionary<string, PushSocket> _dicPushSocket;
        private readonly IUserRepository _userRepository;

        public IntegrationReplyHandler(IUserRepository userRepository, int handlerId)
        {
            _handlerId = handlerId;
            _userRepository = userRepository;
            _dicPushSocket = _dicPushSocket ?? new Dictionary<string, PushSocket>();
        }

        public void OnEvent(UpdateCreditLimitReplyEvent data, long sequence, bool endOfBatch)
        {
            if (_userRepository.Exist(data.UserId) && _handlerId == _userRepository.GetT(data.UserId).ReplyHandlerId)
            {
                Console.WriteLine(data.Message);
            }
        }
    }
}
