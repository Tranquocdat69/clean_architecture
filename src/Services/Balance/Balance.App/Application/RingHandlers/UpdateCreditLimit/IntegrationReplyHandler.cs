namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class IntegrationReplyHandler : IRingHandler<UpdateCreditLimitReplyEvent>
    {
        private readonly int _handlerId;
        public IntegrationReplyHandler(int handlerId)
        {
            _handlerId = handlerId;
        }
        public void OnEvent(UpdateCreditLimitReplyEvent data, long sequence, bool endOfBatch)
        {
            throw new NotImplementedException();
        }
    }
}
