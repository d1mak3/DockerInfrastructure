using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MessagesServer.Controllers
{
    public class RabbitController
    {
        private readonly string URI = Environment.GetEnvironmentVariable("RABBIT_URI") ?? "amqp://guest:guest@localhost:5672";
        private readonly string QUEUE_NAME = Environment.GetEnvironmentVariable("QUEUE_NAME") ?? "messages_queries";

        private IConnection _connection;
        private IModel _channel;
        private IDatabaseController _databaseController;

        public RabbitController(IDatabaseController databaseController)
        {
            _databaseController = databaseController;

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(URI)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QUEUE_NAME,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
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
                            Models.Message messageToSave = JsonConvert.DeserializeObject<Models.Message>(jsonObject["Data"].ToString());
                            _databaseController.SaveMessage(messageToSave);
                            
                            _channel.BasicAck(deliveryTag: data.DeliveryTag, multiple: false);
                            break;
                        
                        case "GET":
                            List<Models.Message> messages = _databaseController.GetMessages();
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

                _channel.BasicConsume(queue: QUEUE_NAME,
                                             autoAck: false,
                                             consumer: consumer);
            }
        }
    }
}