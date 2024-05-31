
using RoomSense_Backend.Entity;

namespace RoomSense_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            EnsureDatabase();

            app.Run();
        }

        private static void EnsureDatabase()
        {
            using (var context = new MonitoringContext())
            {
                context.EnsureCreated();
                Console.WriteLine("Database created");

                // Seed initial data
                if (!context.Rooms.Any())
                {
                    var room = new Room { Name = "Room 1", Location = "Floor 1" };
                    context.Rooms.Add(room);
                    context.SaveChanges();
                    Console.WriteLine("Database seeded");
                }
            }
        }
    }
}
