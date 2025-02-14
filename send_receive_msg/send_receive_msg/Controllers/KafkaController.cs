using send_receive_msg.Services;
using Microsoft.AspNetCore.Mvc;

namespace send_receive_msg.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KafkaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IKafkaProducerService _producerService;

        public KafkaController(IKafkaProducerService producerService, IConfiguration configuration)
        {
            _producerService = producerService;
            _configuration = configuration;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromQuery] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Both 'topic' and 'message' query parameters are required.");
            }

            await _producerService.SendMessageAsync(message);
            return Ok($"Message '{message}' sent successfully to current topic.");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteTopic([FromQuery] string topicName)
        {
            var kafkaTopic = new KafkaTopicService(_configuration);
            await kafkaTopic.DeleteTopicAsync(topicName);
            return Ok($"Topic {topicName} deleted successfully.");
        }
    }
}
