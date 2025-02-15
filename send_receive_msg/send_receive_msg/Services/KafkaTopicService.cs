using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace send_receive_msg.Services
{
    public class KafkaTopicService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;

        // Constructor to initialize Kafka topic with configuration
        public KafkaTopicService(IConfiguration configuration)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"];
            _topic = configuration["Kafka:Topic"];
        }

        // Method to send message to Kafka topic
        public void CreateTopicAsync()
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build();

            try
            {
                // Lấy danh sách các topic hiện có
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                var topics = new HashSet<string>();

                foreach (var topic in metadata.Topics)
                {
                    topics.Add(topic.Topic);
                }

                // Nếu topic chưa tồn tại, tạo mới
                if (!topics.Contains(_topic))
                {
                    var newTopics = new List<TopicSpecification>
                    {
                        new() { Name = _topic, NumPartitions = 1, ReplicationFactor = 1 }
                    };

                    adminClient.CreateTopicsAsync(newTopics);
                    Console.WriteLine($"Topic '{_topic}' created.");
                }
                else
                {
                    Console.WriteLine($"Topic '{_topic}' is existed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when check/create topic: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteTopicAsync(string topicName)
        {
            var config = new AdminClientConfig { BootstrapServers = _bootstrapServers };

            using var adminClient = new AdminClientBuilder(config).Build();

            try
            {
                Console.WriteLine($"Deleting topic: {topicName}...");
                await adminClient.DeleteTopicsAsync(new[] { topicName });
                Console.WriteLine($"Topic {topicName} deleted successfully.");
            }
            catch (DeleteTopicsException e)
            {
                Console.WriteLine($"Failed to delete topic {topicName}: {e.Results[0].Error.Reason}");
            }
        }
    }
}
