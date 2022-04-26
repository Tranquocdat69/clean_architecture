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
            long sq = 0;

            if (_userRepository.Exist(data.UserId))
            {
                decimal oldCreditLimit = _userRepository.GetT(data.UserId).CreditLimit;
                decimal newCreditLimit = oldCreditLimit - data.TotalCost;

                if (newCreditLimit >= 0)
                {
                    InMemoryUser currentInMemoryUser = _userRepository.GetT(data.UserId);
                    currentInMemoryUser.CreditLimit = newCreditLimit;
                    _userRepository.Add(data.UserId, currentInMemoryUser);

                    isSuccess = true;
                    message = "Success";

                    sq = _ringPersistentBuffer.Next();
                    var persistentEvent = _ringPersistentBuffer[sq];
                    persistentEvent.Offset = data.Offset;
                    persistentEvent.UserId = data.UserId;
                    persistentEvent.CreditLimit = newCreditLimit;
                    _ringPersistentBuffer.Publish(sq);

                    Console.WriteLine("Current Credit Limit: "+currentInMemoryUser.CreditLimit);
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
                _ringRelplyBuffer.Publish(sq);
            }
        }
    }
}
