using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace Balance.App.Controllers;

[ApiController]
public class PublishMessageController : ControllerBase
{
    private readonly IPublisher<ProducerData<string, string>> _producer;
    private const string Topic = "balance-command-topic";
    public PublishMessageController(IPublisher<ProducerData<string, string>> producer)
    {
        _producer = producer;
    }

    [HttpPost]
    [Route("Publish-Single-Message/{userId}/{totalCost}")]
    public IActionResult PostSingleMsg(int userId, decimal totalCost)
    {
        var keyMsg = "command" + Guid.NewGuid().ToString();
        var valueMsg = "{\"TotalCost\": " + totalCost + ",\"UserId\": " + userId + ",\"ReplyAddress\": \"localhost:8888\"}";

        var producerData = new ProducerData<string, string>(valueMsg, keyMsg, Topic, 0);
        _producer.Publish(producerData);

        return Ok();
    }

    [HttpPost]
    [Route("Publish-Multi-Message")]
    public IActionResult PostMultiMsg()
    {
        for (int i = 1; i <= 1000; i++)
        {
            var keyMsg = "command" + Guid.NewGuid().ToString();
            var valueMsg = "{\"TotalCost\": " + 1000 + ",\"UserId\": " + i + ",\"ReplyAddress\": \"localhost:8888\"}";

            var producerData = new ProducerData<string, string>(valueMsg, keyMsg, Topic, 0);
            _producer.Publish(producerData);
        }

        return Ok();
    }
}
