using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomSense_Backend.Dto;
using RoomSense_Backend.Entity;

namespace RoomSense_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlarmsController : ControllerBase
    {
        private readonly MonitoringContext _context;

        public AlarmsController(MonitoringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlarmDto>>> GetAlarms(int page = 1, int pageSize = 10)
        {
            var alarms = await _context.Alarms
                .Include(a => a.Room)
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AlarmDto
                {
                    Id = a.Id,
                    RoomId = a.RoomId,
                    RoomName = a.Room.Name,
                    Message = a.Message,
                    Timestamp = a.Timestamp
                })
                .DistinctBy(a => a.RoomId)
                .ToListAsync();

            if (alarms == null || alarms.Count == 0)
            {
                return NotFound();
            }

            return Ok(alarms);
        }
    }

}
