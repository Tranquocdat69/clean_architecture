using Confluent.Kafka;
using ECom.BuildingBlocks.MessageQueue.KafkaMessageQueue;
using Microsoft.AspNetCore.Mvc;

namespace Balance.App.Controllers;

[ApiController]
[Route("PublishMsgToBalanceCommandTopic")]
public class WeatherForecastController : ControllerBase
{
    private readonly KafkaProducer<string, string> _producer;

    public WeatherForecastController(KafkaProducer<string, string> producer)
    {
        _producer = producer;
    }

    [HttpGet(Name = "PublishMsgToBalanceCommandTopic/{userId}/{totalCost}")]
    public IActionResult Get(int userId, decimal totalCost)
    {
        _producer.Produce(new Message<string, string>() { Key = "command"+Guid.NewGuid().ToString() ,Value = "{\"TotalCost\": "+totalCost+",\"UserId\": "+userId+",\"ReplyAddress\": \"localhost:8888\"}"},"balance-command-topic");
        return Ok();
    }
}
