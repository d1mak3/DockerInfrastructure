using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;

namespace BrokerServer.Controllers
{
    [Route("api/producer")]
    [ApiController]
    public class RabbitProduceController : ControllerBase
    {
        private readonly string URI = Environment.GetEnvironmentVariable("RABBIT_URI") ?? "amqp://guest:guest@localhost:5672";
        private readonly string QUEUE_NAME = Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "messages_queries";

        private readonly IConnection _connection;
        private readonly IModel _channel;        
        private readonly IBasicProperties _properties;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _nameToReply;
        private readonly BlockingCollection<string> _responsesQueue = new BlockingCollection<string>();        

        public RabbitProduceController()
        {
            var factory = new ConnectionFactory() { Uri = new Uri(URI) };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _consumer = new EventingBasicConsumer(_channel);

            _properties = _channel.CreateBasicProperties();
            _nameToReply = _channel.QueueDeclare().QueueName;            

            var correlationId = Guid.NewGuid().ToString();
            _properties.CorrelationId = correlationId;
            _properties.ReplyTo = _nameToReply;

            _consumer.Received += (sender, data) =>
            {
                string resultString = Encoding.UTF8.GetString(data.Body.ToArray());

                if (data.BasicProperties.CorrelationId == _properties.CorrelationId)
                {
                    _responsesQueue.Add(resultString);
                }
            };

            _channel.BasicConsume(
                consumer: _consumer,
                queue: _nameToReply,
                autoAck: true
            );
        }

        [HttpPost]
        public IActionResult AddPostQuery([FromBody] Models.Message newMessage)
        {
            string serializedCommand = JsonConvert.SerializeObject(new Models.Command()
            {
                Option = "POST",
                Data = JsonConvert.SerializeObject(newMessage)
            });

            _channel.BasicPublish(
                exchange: "",
                routingKey: QUEUE_NAME,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(serializedCommand)
            );

            return Created("", null);
        }

        [HttpGet]
        public IActionResult AddGetQuery()
        {
            string serializedCommand = JsonConvert.SerializeObject(new Models.Command()
            {
                Option = "GET",
                Data = ""
            });  

            _channel.BasicPublish(
                exchange: "",
                routingKey: QUEUE_NAME,
                basicProperties: _properties,
                body: Encoding.UTF8.GetBytes(serializedCommand)
            );          

            List<Models.Message> messages = JsonConvert.DeserializeObject<List<Models.Message>>(_responsesQueue.Take());

            return Ok(messages);
        }
    }
}