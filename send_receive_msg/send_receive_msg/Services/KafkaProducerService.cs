using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text.Json.Serialization;

namespace send_receive_msg.Services
{
    public interface IKafkaProducerService
    {
        Task SendMessageAsync(string message);
    }

    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;

        private readonly string _bootstrapServers;
        private readonly string _topic;

        // Constructor to initialize Kafka producer with configuration
        public KafkaProducerService(IConfiguration configuration)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"];
            _topic = configuration["Kafka:Topic"];

            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers // Kafka server details (ensure this is correct)
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        // Method to send message to Kafka topic
        public async Task SendMessageAsync(string message)
        {
            try
            {
                //// Send message to the specified Kafka topic
                //await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
                //Console.WriteLine($"Message '{message}' sent to topic '{_topic}'.");

                // send object to kafka topic
                dynamic obj = new ExpandoObject();
                obj.Id = 10;
                obj.Email = "xxxxx@gmail.com";
                obj.Description = "Email from google";
                var _obj = JsonConvert.SerializeObject(obj);
                await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = _obj });
                Console.WriteLine($"Message Object '{_obj}' sent to topic '{_topic}'.");
            }
            catch (Exception ex)
            {
                // Log any errors encountered while sending message
                Console.WriteLine($"Error sending message to Kafka: {ex.Message}");
                throw;
            }
        }
    }
}
