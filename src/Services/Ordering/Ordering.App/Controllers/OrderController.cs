using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECom.Services.Ordering.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("demo")]
        public async Task<IActionResult> Demo()
        {
            List<OrderItemDTO> list = new();
            for(var i = 1; i < 4; i++)
            {
                list.Add(new OrderItemDTO()
                {
                    Discount = i,
                    PictureUrl = "demo",
                    ProductId = i,
                    ProductName = "ProductName>i",
                    UnitPrice = i,
                    Units = i * 10
                });
            }
            CreateOrderCommand command = new(list, 1, "demo", "demo");
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
