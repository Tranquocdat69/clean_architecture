using System.Text.RegularExpressions;

namespace ECom.Services.Balance.App.Application.Commands
#nullable disable
{
    public class UpdateCreditLimitCommand : IRequest
    {
        public int UserId { get; set; }
        public decimal TotalCost { get; set; }
        public string ReplyAddress { get; set; }
        public long Offset { get; set; }
        public bool IsCompensatedMessage { get; set; }

        public UpdateCreditLimitCommand FromString(string str)
        {
            var splits = Regex.Replace(str, "[}{\"]", string.Empty).Split(',');

            var totalCostRaw = splits[0];
            var userIdRaw = splits[1];
            var replyAddressRaw = splits[2];

            var userId = Int32.Parse(userIdRaw.Substring(userIdRaw.ToString().IndexOf(":") + 1));
            var totalCost = Int32.Parse(totalCostRaw.Substring(totalCostRaw.ToString().IndexOf(":") + 1));
            var replyAddress = replyAddressRaw.Substring(replyAddressRaw.ToString().IndexOf(":") + 1);

            return new UpdateCreditLimitCommand()
            {
                UserId = userId,
                TotalCost = totalCost,
                ReplyAddress = replyAddress,
            };
        }
    }
}
