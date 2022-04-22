
namespace ECom.Services.Ordering.App.Application.Commands
#nullable disable
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ResponseData>
    {
        private readonly RingBuffer<CreateOrderEvent> _ring;
        private readonly InMemoryRequestManagement _requestManagement;
        private readonly IMediator _mediator;

        public CreateOrderCommandHandler(
            RingBuffer<CreateOrderEvent> ring, 
            InMemoryRequestManagement requestManagement, 
            IMediator mediator) 
        {
            _ring              = ring;
            _requestManagement = requestManagement;
            _mediator          = mediator;
        }
        public async Task<ResponseData> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Tạo value object address
            var address           = new Address(request.Street, request.City);
            // Tạo Order
            var order             = new Order(request.UserId, address); 
            // Thêm item vào order
            foreach(var item in request.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }
            // Tạo integration request Id để gửi message qua 2 service là balance và catalog
            var catalogRequestId = _requestManagement.GenernateRequestId();
            var balanceRequestId = _requestManagement.GenernateRequestId();

            // Sử lý với ring
            var res = await RingHandlingAsync(order, balanceRequestId, catalogRequestId);

            // Nếu sử lý ở 2 service thành công thì set order confirmed 
            // Nếu thất bại thì set order reject
            if (res.IsSuccess)
            {
                order.SetOrderConfirmed();
            }
            else
            {
                //var topic = balanceRequestId.Equals(res.RequestId) ? _commandTopic["Balance"] : _commandTopic["Catalog"];
                order.SetOrderRejected(res.Convension);
            }
            // Thực hiện DomainEvent
            await _mediator.DispatchDomainEventsAsync(order);

            return res;
        }

        private async Task<ResponseData> RingHandlingAsync(Order order, string balanceRequestId, string catalogRequestId)
        {
            // Lấy squence
            var sequence = _ring.Next();
            try
            {
                var data = _ring[sequence];
                data.Items = order.OrderItems.ToDictionary(x => x.ProductId, x => x.GetUnits());
                data.TotalCost = order.OrderItems.Sum(x => x.GetUnits() * x.GetUnitPrice());
                data.UserId = order.CustomerId;
                data.BalanceRequestId = balanceRequestId;
                data.CatalogRequestId = catalogRequestId;
            }
            finally
            {
                //Dẩy dữ liệu đi sử lý
                _ring.Publish(sequence);
            }

            // Chờ có phản hồi thử 2 service bằng request Id
            var tasks = await Task.WhenAll(
                _requestManagement.GetResponseAsync(balanceRequestId),
                _requestManagement.GetResponseAsync(catalogRequestId));

            var response = new ResponseData();
            // Kiểm tra có kết quả phản hồi
            if (!tasks.Contains(null) && tasks.Any())
            {
                // Cast object về ResponseData 
                var resList = tasks.Cast<ResponseData>();
                // Lấy ra ResponseData không thành công đầu tiên
                // Nếu không có thì lấy ResponseData đầu tiên
                response = resList.FirstOrDefault(x => !x.IsSuccess) ?? resList.FirstOrDefault();
                // Kiểm tra nếu response là không thành công và các response có ít nhất 1 thành công
                if(!response.IsSuccess && resList.Where(x => x.IsSuccess).Any())
                {
                    // Lấy topic 
                    response.Convension = balanceRequestId.Equals(response.RequestId) ? "Catalog" : "Balance";
                }

            }
            return response;
        }
    }

    public class ResponseData
    {
        public string ReplyAddress { get; set; }
        public bool IsSuccess { get; set; } = false;
        public string Message { get; set; } = "Request timeout";
        public string Convension { get; set; }
        public string RequestId { get; set; }

        public override string ToString()
        {
            return "{\"IsSuccess\":" + IsSuccess + ",\"Message\":\"" + Message
            + "\",\"ReplyAddress\":\"" + ReplyAddress + "\",\"RequestId\":\"" + RequestId + "\"}";
        }

        public static ResponseData FromString(string str)
        {
            var splits = Regex.Replace(str, "[}{\"]", string.Empty).Split(',');
            return new ResponseData
            {
                IsSuccess = bool.Parse(splits[0].Substring(10)),
                Message = splits[1].Substring(8),
                ReplyAddress = splits[2].Substring(13),
                RequestId = splits[3].Substring(10)
            };
        }
    }
}
