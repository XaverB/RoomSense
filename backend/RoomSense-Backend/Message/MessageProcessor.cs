using MQTTnet.Client;
using System.Text.Json;
using System.Text;
using RoomSense_Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace RoomSense_Backend.Message
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;
        private readonly MonitoringContext _dbContext;

        public MessageProcessor(ILogger<MessageProcessor> logger, MonitoringContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);
                var topic = eventArgs.ApplicationMessage.Topic;

                _logger.LogInformation("Received MQTT message on topic: {Topic}", topic);

                var segments = topic.Split('/');
                if (segments.Length == 3)
                {
                    var roomName = segments[1];
                    var sensorType = segments[2];

                    await ProcessMessage(roomName, sensorType, message);
                }
                else
                {
                    _logger.LogWarning("Invalid topic format: {Topic}", topic);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the MQTT message");
            }
        }

        private async Task ProcessMessage(string roomName, string sensorType, string message)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<Payload>(message);
                _logger.LogInformation("{SensorType} in room {RoomName}: {Value} (Device: {Device}, Timestamp: {Timestamp})",
                    sensorType, roomName, payload.Value, payload.Device, payload.Timestamp);

                var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Name == roomName);
                if (room == null)
                {
                    room = new Room { Name = roomName };
                    _dbContext.Rooms.Add(room);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Created new room: {RoomName}", roomName);
                }

                var sensor = await _dbContext.Sensors.FirstOrDefaultAsync(s => s.RoomId == room.Id && s.Type == sensorType);
                if (sensor == null)
                {
                    sensor = new Sensor { Type = sensorType, RoomId = room.Id };
                    _dbContext.Sensors.Add(sensor);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Created new sensor: {SensorType} for room {RoomName}", sensorType, roomName);
                }

                var reading = new Reading
                {
                    SensorId = sensor.Id,
                    Value = float.Parse(payload.Value),
                    Timestamp = DateTime.Parse(payload.Timestamp)
                };
                _dbContext.Readings.Add(reading);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Stored reading for sensor {SensorId}", sensor.Id);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize payload: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing message for room {RoomName} and sensor {SensorType}", roomName, sensorType);
            }
        }
    }
}
