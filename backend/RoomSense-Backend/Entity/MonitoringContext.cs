using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace RoomSense_Backend.Entity
{

    public class MonitoringContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Reading> Readings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Alarm> Alarms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("ConnectionStringRoomSenseDB");
        }

        public void EnsureCreated()
        {
            Database.EnsureCreated();
        }
    }
}
