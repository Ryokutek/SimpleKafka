using Microsoft.AspNetCore.Mvc;
using SimpleKafka.Interfaces;

namespace SimpleKafkaTest;

[ApiController]
[Route("startTest")]
public class ProduceController : Controller
{
    private readonly IKafkaProducer<string> _kafkaProducer;

    public ProduceController(IKafkaProducer<string> kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    [HttpGet]
    public void StartTest()
    {
        _kafkaProducer.ProduceAsync(new KafkaTestEvent
        {
            Id = Guid.NewGuid(),
            Message = "Hello, world!"
        });
    }
}