using Microsoft.EntityFrameworkCore;
using RoomSense_Backend.Entity;

namespace RoomSense_Backend.Service
{
    public class AlarmProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly double _TemperatureThreshold = 30.0;
        private readonly double _Co2Threshold = 1000.0;
        private readonly int _DelayMinutes = 1;


        public AlarmProcessingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            if (double.TryParse(Environment.GetEnvironmentVariable("TEMPERATURE_THRESHOLD"), out double temp))
            {
                _TemperatureThreshold = temp;
            }
            if (double.TryParse(Environment.GetEnvironmentVariable("CO2_THRESHOLD"), out double co2))
            {
                _Co2Threshold = co2;
            }
            if (int.TryParse(Environment.GetEnvironmentVariable("ALARM_PROCESSING_SERVICE_DELAY_MINUTES"), out int min))
            {
                _DelayMinutes = min;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MonitoringContext>();
                    await ProcessAlarmsAsync(dbContext);
                }

                await Task.Delay(TimeSpan.FromMinutes(_DelayMinutes), stoppingToken);
            }
        }

        private async Task ProcessAlarmsAsync(MonitoringContext dbContext)
        {
            // Retrieve the latest readings for each sensor
            var latestReadings = await dbContext.Readings
                .GroupBy(r => r.SensorId)
                .Select(g => g.OrderByDescending(r => r.Timestamp).FirstOrDefault())
                .ToListAsync();

            foreach (var reading in latestReadings)
            {
                var sensor = await dbContext.Sensors.FindAsync(reading.SensorId);
                var room = await dbContext.Rooms.FindAsync(sensor.RoomId);

                if (sensor.Type == "temperature" && reading.Value > _TemperatureThreshold)
                {
                    // Check if an alarm already exists for the same room and timestamp
                    var existingAlarm = await dbContext.Alarms
                        .FirstOrDefaultAsync(a => a.RoomId == room.Id && a.Timestamp == reading.Timestamp);

                    if (existingAlarm == null)
                    {
                        var alarm = new Alarm
                        {
                            RoomId = room.Id,
                            Message = $"High temperature detected in {room.Name}. Current value: {reading.Value}°C",
                            Timestamp = reading.Timestamp
                        };
                        await dbContext.Alarms.AddAsync(alarm);
                    }
                }
                else if (sensor.Type == "co2" && reading.Value > _Co2Threshold)
                {
                    // Check if an alarm already exists for the same room and timestamp
                    var existingAlarm = await dbContext.Alarms
                        .FirstOrDefaultAsync(a => a.RoomId == room.Id && a.Timestamp == reading.Timestamp);

                    if (existingAlarm == null)
                    {
                        var alarm = new Alarm
                        {
                            RoomId = room.Id,
                            Message = $"High CO2 level detected in {room.Name}. Current value: {reading.Value} ppm",
                            Timestamp = reading.Timestamp
                        };
                        await dbContext.Alarms.AddAsync(alarm);
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
