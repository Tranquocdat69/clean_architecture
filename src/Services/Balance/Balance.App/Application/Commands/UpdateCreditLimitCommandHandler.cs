namespace ECom.Services.Balance.App.Application.Commands
{
    public class UpdateCreditLimitCommandHandler : IRequestHandler<UpdateCreditLimitCommand>
    {
        private readonly RingBuffer<UpdateCreditLimitEvent> _inputRing;

        public UpdateCreditLimitCommandHandler(RingBuffer<UpdateCreditLimitEvent> inputRing)
        {
            _inputRing = inputRing;
        }

        public Task<Unit> Handle(UpdateCreditLimitCommand request, CancellationToken cancellationToken)
        {
            var sequence = _inputRing.Next();

            var data = _inputRing[sequence];
            data.UserId = request.UserId;
            data.TotalCost = request.TotalCost;
            data.ReplyAddress = request.ReplyAddress;
            data.Offset = request.Offset;
            data.IsCompensatedMessage = request.IsCompensatedMessage;

            _inputRing.Publish(sequence);

            return Task.FromResult(Unit.Value);
        }
    }
}
