namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class LogHandler : IRingHandler<UpdateCreditLimitEvent>
    {
        private readonly ILogger<LogHandler> _logger;
        private readonly int _handlerId;

        public LogHandler(ILogger<LogHandler> logger, int handlerId)
        {
            _logger = logger;
            _handlerId = handlerId;
        }

        public void OnEvent(UpdateCreditLimitEvent data, long sequence, bool endOfBatch)
        {
            if (_handlerId == 1)
            {
                Console.WriteLine(data.Offset);
            }
        }
    }
}
