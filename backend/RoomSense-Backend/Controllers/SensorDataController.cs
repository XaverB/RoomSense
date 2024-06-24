using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomSense_Backend.Dto;
using RoomSense_Backend.Entity;

namespace RoomSense_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorDataController : ControllerBase
    {
        private readonly MonitoringContext _context;

        public SensorDataController(MonitoringContext context)
        {
            _context = context;
        }

        [HttpGet("sensors")]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetSensors()
        {
            var sensors = await _context.Sensors
                .Include(s => s.Room)
                .Select(s => new SensorDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    RoomId = s.RoomId,
                    RoomName = s.Room.Name
                })
                .ToListAsync();

            if (sensors == null || sensors.Count == 0)
            {
                return NotFound();
            }

            return Ok(sensors);
        }

        /// <summary>
        /// Get sensor data per room.
        /// Valid type values: 'humidity', 'co2' or 'temperature'.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<Reading>>> GetSensorDataByRoom(int roomId, string type, int page = 1, int pageSize = 10)
        {
            if (!IsValidSensorType(type))
            {
                return BadRequest($"Invalid sensor type. Valid types are: 'humidity', 'co2' or 'temperature'");
            }

            var sensorData = await _context.Readings
                .Where(r => r.Sensor.RoomId == roomId && r.Sensor.Type == type)
                .OrderByDescending(r => r.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (sensorData == null || sensorData.Count == 0)
            {
                return NotFound();
            }

            return Ok(sensorData.OrderBy(x => x.Timestamp).ToList());
        }

        private bool IsValidSensorType(string type)
        {
            string[] validTypes = { "humidity", "co2", "temperature" };
            return validTypes.Contains(type);
        }
    }
}
