using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace Balance.App.Controllers;

[ApiController]
[Route("PublishMsgToBalanceCommandTopic")]
public class WeatherForecastController : ControllerBase
{
    private readonly IPublisher<ProducerData<string, string>> _producer;
    private readonly ProducerData<string, string> _message;
    private const string Topic = "balance-command-topic";
    public WeatherForecastController(IPublisher<ProducerData<string, string>> producer, ProducerData<string, string> message)
    {
        _producer = producer;
        _message = message;
    }

    [HttpGet(Name = "PublishMsgToBalanceCommandTopic/{userId}/{totalCost}")]
    public IActionResult Get(int userId, decimal totalCost)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
        };
        var producer = new ProducerBuilder<string, string>(config).Build();
        try
        {
                producer.Produce(
                    Topic,
                    new Message<string, string>
                    {
                        Key = "command" + Guid.NewGuid().ToString(),
                        Value = "{\"TotalCost\": " + totalCost + ",\"UserId\": " + userId + ",\"ReplyAddress\": \"localhost:8888\"}"
                    });
        }
        catch (ProduceException<Null, string> ex)
        {
            if (ex.Error.Code == ErrorCode.Local_QueueFull)
            {
                producer.Poll(TimeSpan.FromSeconds(1.0));
            }

            throw;
        }
       
        //_message.Key = "command" + Guid.NewGuid().ToString();
        //_message.Value = "{\"TotalCost\": " + totalCost + ",\"UserId\": " + userId + ",\"ReplyAddress\": \"localhost:8888\"}";

        return Ok();
    }
}
