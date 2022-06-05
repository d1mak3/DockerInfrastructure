using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MessagesServer.Interfaces;

namespace MessagesServer.Controllers
{
    public class RabbitController
    {
        private readonly string URI = Environment.GetEnvironmentVariable("RABBIT_URI") ?? "amqp://guest:guest@localhost:5672";
        private readonly string QUEUE_NAME = Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "messages_queries";

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IDatabaseController _databaseController;
        private readonly ICacheController _cacheController;

        public RabbitController(IDatabaseController databaseController, ICacheController cacheController)
        {
            _databaseController = databaseController;
            _cacheController = cacheController;

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(URI)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            string queueName = _channel.QueueDeclare().QueueName;

            _channel.ExchangeDeclare(
                exchange: "test",
                type: "direct"
            );

            _channel.QueueBind(
                queue: queueName,
                exchange: "test",
                routingKey: QUEUE_NAME
            );

            _channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false
            );
        }        

        public void GetQueryFromTheQueue()
        {
            while (true)
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (sender, data) =>
                {                    
                    var body = data.Body;
                    string message = Encoding.UTF8.GetString(body.ToArray());

                    JObject jsonObject = JObject.Parse(message);

                    switch (jsonObject["Option"].ToString())
                    {
                        case "POST":                            
                            Thread.Sleep(3000);
                            Models.Message messageToSave = JsonConvert.DeserializeObject<Models.Message>(jsonObject["Data"].ToString());
                            _databaseController.SaveMessage(messageToSave);
                            _cacheController.SetValue("select_all", JsonConvert.SerializeObject(_databaseController.GetMessages()));

                            _channel.BasicAck(deliveryTag: data.DeliveryTag, multiple: false);                            
                            break;
                        
                        case "GET":                            
                            Thread.Sleep(3000);
                            List<Models.Message> messages = JsonConvert.DeserializeObject<List<Models.Message>>(_cacheController.GetValueByKey("select_all"));
                            string serializedMessages = JsonConvert.SerializeObject(messages);

                            var replyProps = _channel.CreateBasicProperties();
                            replyProps.CorrelationId = data.BasicProperties.CorrelationId;

                            _channel.BasicPublish(
                                exchange: "",
                                routingKey: data.BasicProperties.ReplyTo,
                                basicProperties: replyProps,
                                body: Encoding.UTF8.GetBytes(serializedMessages)
                            );

                            _channel.BasicAck(deliveryTag: data.DeliveryTag, multiple: false);                            
                            break;
                            
                        default:
                            break;
                    }

                    
                };

                _channel.BasicConsume(
                    queue: QUEUE_NAME,
                    autoAck: false,
                    consumer: consumer
                );
                
            }
        }
    }
}