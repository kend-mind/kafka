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

        async Task EnsureTopicExistsAsync()
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

                    await adminClient.CreateTopicsAsync(newTopics);
                    Console.WriteLine($"✅ Topic '{_topic}' đã được tạo.");
                }
                else
                {
                    Console.WriteLine($"⚡ Topic '{_topic}' đã tồn tại.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi kiểm tra/tạo topic: {ex.Message}");
            }
        }

        // Method to send message to Kafka topic
        public void CreateTopicAsync()
        {
            try
            {
                // Đảm bảo topic tồn tại trước khi gửi
                EnsureTopicExistsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
