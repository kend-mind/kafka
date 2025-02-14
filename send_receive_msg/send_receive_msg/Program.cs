using send_receive_msg.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Kafka producer service with Dependency Injection
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Khởi động Kafka Topic khi ứng dụng chạy
var kafkaTopic = new KafkaTopicService(app.Services.GetRequiredService<IConfiguration>());
kafkaTopic.CreateTopicAsync();

// Khởi động Kafka Consumer khi ứng dụng chạy
var kafkaConsumer = new KafkaConsumerService(app.Services.GetRequiredService<IConfiguration>());
var cancellationTokenSource = new CancellationTokenSource();
Task.Run(() => kafkaConsumer.ListeningAsync(cancellationTokenSource.Token));

app.Run();
