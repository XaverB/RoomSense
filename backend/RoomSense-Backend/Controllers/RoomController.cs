using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomSense_Backend.Entity;

namespace RoomSense_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly MonitoringContext _context;

        public RoomsController(MonitoringContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();

            if (rooms == null || rooms.Count == 0)
            {
                return NotFound("There are currently no monitored rooms.");
            }

            return Ok(rooms);
        }
    }
}
