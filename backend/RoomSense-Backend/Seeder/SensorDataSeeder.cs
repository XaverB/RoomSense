using RoomSense_Backend.Entity;

namespace RoomSense_Backend.Seeder
{
    public static class SensorDataSeeder
    {
        public static void SeedSensorData(MonitoringContext context)
        {
            if (context.Readings.Any())
            {
                Console.WriteLine("SensorData has already been seeded");
                return; // Data has already been seeded
            }

            var rooms = context.Rooms.ToList();
            var sensors = context.Sensors.ToList();

            var random = new Random();
            var baseTemperature = 20.0f;
            var baseCo2 = 500.0f;

            foreach (var room in rooms)
            {
                var temperatureSensor = sensors.FirstOrDefault(s => s.RoomId == room.Id && s.Type == "temperature");
                var co2Sensor = sensors.FirstOrDefault(s => s.RoomId == room.Id && s.Type == "co2");

                if (temperatureSensor != null && co2Sensor != null)
                {
                    var currentTemperature = baseTemperature;
                    var currentCo2 = baseCo2;

                    for (int i = 0; i < 100; i++)
                    {
                        var temperatureReading = new Reading
                        {
                            SensorId = temperatureSensor.Id,
                            Value = currentTemperature,
                            Timestamp = DateTime.UtcNow.AddMinutes(-i)
                        };

                        var co2Reading = new Reading
                        {
                            SensorId = co2Sensor.Id,
                            Value = currentCo2,
                            Timestamp = DateTime.UtcNow.AddMinutes(-i)
                        };

                        context.Readings.Add(temperatureReading);
                        context.Readings.Add(co2Reading);

                        // Simulate trends
                        currentTemperature += (float) random.NextDouble() * 2 - 1; // Random change between -1 and 1
                        currentCo2 += (float) random.NextDouble() * 100 - 50; // Random change between -50 and 50
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
