namespace ECom.Services.Ordering.App.Application.Integrations
#nullable disable
{
    public class UpdateProductAvaibleStockIntegration : IIntegration
    {
        public UpdateProductAvaibleStockIntegration(IDictionary<int, int> items, string replyAddress)
        {
            OrderItems = items;
            ReplyAddress = replyAddress;
        }

        public IDictionary<int, int> OrderItems { get;}
        public string ReplyAddress { get;}

        /// <summary>
        /// Convert object hiện tại sang Json string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var items  = OrderItems.Select(d => string.Format("\"{0}\": {1}", d.Key, d.Value));
            string ids = string.Join(",", items);
            return "{\"OrderItems\":{" + ids+"},\"ReplyAddress\":\""+ReplyAddress+"\"}";
        }
    }
}
