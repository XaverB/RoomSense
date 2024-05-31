using System.Security.Claims;

namespace RoomSense_Backend.Entity
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public ICollection<Sensor> Sensors { get; set; }
        public ICollection<Recommendation> Recommendations { get; set; }
        public ICollection<Alarm> Alarms { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
