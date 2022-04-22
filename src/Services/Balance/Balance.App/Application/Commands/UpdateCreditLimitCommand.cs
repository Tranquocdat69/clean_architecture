namespace ECom.Services.Balance.App.Application.Commands
#nullable disable
{
    public class UpdateCreditLimitCommand : IRequest
    {
        public decimal TotalCost { get; set; }
        public int UserId { get; set; }
        public string ReplyAddress { get; set; }
    }
}
