namespace ECom.Services.Balance.App.Application.Commands
#nullable disable
{
    public class UpdateCreditLimitCommand : IRequest
    {
        public int UserId { get; set; }
        public decimal TotalCost { get; set; }
        public string ReplyAddress { get; set; }
        public string RequestId { get; set; }

    }
}
