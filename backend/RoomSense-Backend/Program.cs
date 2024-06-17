
using Microsoft.EntityFrameworkCore;
using RoomSense_Backend.Entity;
using RoomSense_Backend.Message;
using RoomSense_Backend.Seeder;
using RoomSense_Backend.Service;

namespace RoomSense_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MonitoringContext>(options =>
            {
                options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStringRoomSenseDB") ?? throw new ArgumentNullException("ConnectionStringRoomSenseDB not defined"));
            });

            builder.Services.AddSingleton<IMqttConnectionService, MqttConnectionService>();
            builder.Services.AddSingleton<IMessageProcessor, MessageProcessor>();
            builder.Services.AddSingleton<MqttConnectionOptions>();
            builder.Services.AddSingleton<IHostedService, MqttHostedService>();
            builder.Services.AddSingleton<Func<IMessageProcessor>>(serviceProvider => () => serviceProvider.GetRequiredService<IMessageProcessor>());

            builder.Services.AddHostedService<RecommendationProcessingService>();
            builder.Services.AddHostedService<AlarmProcessingService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    bld =>
                    {
                        bld.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();


                    });
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors();
            app.MapControllers();

            // Pass the IServiceProvider to the EnsureDatabase method
            EnsureDatabase(app.Services);

            app.Run();
        }

        private static void EnsureDatabase(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MonitoringContext>();
                context.Database.EnsureCreated();
                Console.WriteLine("Database created");

                // Seed initial data
                if (!context.Rooms.Any())
                {
                    var room = new Room { Name = "Room 1", Location = "Floor 1" };
                    context.Rooms.Add(room);
                    context.SaveChanges();
                    Console.WriteLine("Database seeded");
                }
                SensorDataSeeder.SeedSensorData(context);
            }
        }
    }
}
