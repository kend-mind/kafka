using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using System.Dynamic;
using static Confluent.Kafka.ConfigPropertyNames;

namespace send_receive_msg.Services
{
    public class KafkaConsumerService
    {
        private readonly IConsumer<Null, string> _consumer;

        private readonly string _bootstrapServers;
        private readonly string _topic;
        private readonly string _groupId;

        public KafkaConsumerService(IConfiguration configuration)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"];
            _topic = configuration["Kafka:Topic"];
            _groupId = configuration["Kafka:GroupId"];

            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Null, string>(config).Build();
        }

        public async Task ListeningAsync(CancellationToken cancellationToken)
        {
            // Consumer Subscribe topic
            _consumer.Subscribe(_topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    dynamic receiveValue = JsonConvert.DeserializeObject<ExpandoObject>(consumeResult.Message.Value);
                    Console.WriteLine($"Received message: {receiveValue.ReceiveMessage}");
                    Console.WriteLine($"Received object: {consumeResult.Message.Value}");
                }
            }
            catch (OperationCanceledException)
            {
                _consumer.Close();
            }
        }
    }
}
