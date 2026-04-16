using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET =================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();

            foreach (var r in rooms)
            {
                r.Status = r.CurrentOccupancy >= r.MaxCapacity
                    ? "Đã đầy"
                    : "Còn trống";
            }

            return Ok(rooms);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound();

            room.Status = room.CurrentOccupancy >= room.MaxCapacity
                ? "Đã đầy"
                : "Còn trống";

            return Ok(room);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            if (await _context.Rooms.AnyAsync(r => r.RoomName == room.RoomName))
                return BadRequest("Phòng đã tồn tại");

            room.CurrentOccupancy = 0;
            room.Status = "Còn trống";

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, Room room)
        {
            if (id != room.Id)
                return BadRequest();

            var existing = await _context.Rooms.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.RoomName = room.RoomName;
            existing.Building = room.Building;
            existing.Floor = room.Floor;
            existing.RoomType = room.RoomType;
            existing.MaxCapacity = room.MaxCapacity;
            existing.Price = room.Price;

            existing.Status = existing.CurrentOccupancy >= existing.MaxCapacity
                ? "Đã đầy"
                : "Còn trống";

            await _context.SaveChangesAsync();

            return Ok("Cập nhật thành công");
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }
    }
}