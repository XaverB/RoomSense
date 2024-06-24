using Microsoft.EntityFrameworkCore;
using RoomSense_Backend.Entity;

namespace RoomSense_Backend.Service
{
    public class RecommendationProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly int _DelayMinutes = 5;

        public RecommendationProcessingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            if (int.TryParse(Environment.GetEnvironmentVariable("RECOMMENDATION_PROCESSING_SERVICE_DELAY_MINUTES"), out int min))
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
                    await ProcessRecommendationsAsync(dbContext);
                }

                // Wait for a certain interval before the next execution
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessRecommendationsAsync(MonitoringContext dbContext)
        {
            var rooms = await dbContext.Rooms.ToListAsync();

            foreach (var room in rooms)
            {
                var temperatureSensor = await dbContext.Sensors
                    .FirstOrDefaultAsync(s => s.RoomId == room.Id && s.Type == "temperature");

                var co2Sensor = await dbContext.Sensors
                    .FirstOrDefaultAsync(s => s.RoomId == room.Id && s.Type == "co2");

                if (temperatureSensor != null && co2Sensor != null)
                {
                    var temperatureReadings = await dbContext.Readings
                        .Where(r => r.SensorId == temperatureSensor.Id)
                        .OrderByDescending(r => r.Timestamp)
                        .Take(10)
                        .ToListAsync();

                    var co2Readings = await dbContext.Readings
                        .Where(r => r.SensorId == co2Sensor.Id)
                        .OrderByDescending(r => r.Timestamp)
                        .Take(10)
                        .ToListAsync();

                    if (temperatureReadings.Count >= 2 && co2Readings.Count >= 2)
                    {
                        var latestTemperature = temperatureReadings.First().Value;
                        var previousTemperature = temperatureReadings.Last().Value;
                        var temperatureTrend = latestTemperature - previousTemperature;

                        var latestCo2 = co2Readings.First().Value;
                        var previousCo2 = co2Readings.Last().Value;
                        var co2Trend = latestCo2 - previousCo2;

                        var temperatureThreshold = 1.0;
                        var co2Threshold = 50.0;

                        var isTemperatureHigh = latestTemperature > 24.0; // Example threshold for high temperature
                        var isCo2High = latestCo2 > 500.0; // Example threshold for high CO2 level

                        if ((isTemperatureHigh && temperatureTrend > temperatureThreshold) ||
                            (isCo2High && co2Trend > co2Threshold))
                        {
                            var recommendationMessage = "Consider venting the room.";

                            if (isTemperatureHigh && temperatureTrend > temperatureThreshold)
                            {
                                recommendationMessage += " Temperature is rising.";
                            }

                            if (isCo2High && co2Trend > co2Threshold)
                            {
                                recommendationMessage += " CO2 level is rising.";
                            }

                            var recommendation = new Recommendation
                            {
                                RoomId = room.Id,
                                Message = $"{recommendationMessage} Current temperature: {latestTemperature}°C, CO2 level: {latestCo2} ppm.",
                                Timestamp = DateTime.UtcNow
                            };

                            await dbContext.Recommendations.AddAsync(recommendation);
                        }
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
