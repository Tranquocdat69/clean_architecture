using ECom.Services.Balance.Domain.AggregateModels.UserAggregate;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class BusinessHandler : IRingHandler<UpdateCreditLimitEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly RingBuffer<UpdateCreditLimitPersistentEvent> _ringPersistentBuffer;
        private readonly RingBuffer<UpdateCreditLimitReplyEvent> _ringRelplyBuffer;

        public BusinessHandler(IUserRepository userRepository, 
            RingBuffer<UpdateCreditLimitPersistentEvent> ringPersistentBuffer, 
            RingBuffer<UpdateCreditLimitReplyEvent> ringRelplyBuffer)
        {
            _userRepository = userRepository;
            _ringPersistentBuffer = ringPersistentBuffer;
            _ringRelplyBuffer = ringRelplyBuffer;
        }

        public void OnEvent(UpdateCreditLimitEvent data, long sequence, bool endOfBatch)
        {
            string message = "";
            bool isSuccess = false;
            long sq = 0L;

            if (_userRepository.Exist(data.UserId))
            {
                decimal oldCreditLimit = _userRepository.GetT(data.UserId).CreditLimit;
                decimal newCreditLimit = oldCreditLimit - data.TotalCost;

                if (newCreditLimit >= 0)
                {
                    User currentUser = _userRepository.GetT(data.UserId);
                    currentUser.DecreaseCash(data.TotalCost);
                    _userRepository.Add(data.UserId, currentUser);

                    isSuccess = true;
                    message = "Success";

                    sq = _ringPersistentBuffer.Next();
                    var persistentEvent = _ringPersistentBuffer[sq];
                    persistentEvent.Offset = data.Offset;
                    persistentEvent.UserId = data.UserId;
                    persistentEvent.CreditLimit = newCreditLimit;
                    _ringPersistentBuffer.Publish(sq);

                    Console.WriteLine("Current Credit Limit of " + _userRepository.GetT(data.UserId).Name + " is: " + currentUser.CreditLimit);
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
