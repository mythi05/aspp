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

        // =========================
        // GET: api/room
        // Lấy tất cả phòng
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return Ok(await _context.Rooms.ToListAsync());
        }

        // =========================
        // GET: api/room/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound("Không tìm thấy phòng");

            return Ok(room);
        }

        // =========================
        // POST: api/room
        // Thêm phòng
        // =========================
        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            // Check trùng tên phòng
            if (await _context.Rooms.AnyAsync(r => r.RoomName == room.RoomName))
            {
                return BadRequest("Phòng đã tồn tại");
            }

            // Set trạng thái ban đầu
            room.CurrentOccupancy = 0;
            room.Status = "Còn trống";

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }

        // =========================
        // PUT: api/room/{id}
        // Cập nhật phòng
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, Room room)
        {
            if (id != room.Id)
                return BadRequest("ID không khớp");

            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null)
                return NotFound("Không tìm thấy phòng");

            // Update dữ liệu
            existingRoom.RoomName = room.RoomName;
            existingRoom.Building = room.Building;
            existingRoom.Floor = room.Floor;
            existingRoom.RoomType = room.RoomType;
            existingRoom.MaxCapacity = room.MaxCapacity;
            existingRoom.Price = room.Price;

            // Update trạng thái theo số người
            if (existingRoom.CurrentOccupancy >= existingRoom.MaxCapacity)
            {
                existingRoom.Status = "Đã đầy";
            }
            else
            {
                existingRoom.Status = "Còn trống";
            }

            await _context.SaveChangesAsync();

            return Ok("Cập nhật thành công");
        }

        // =========================
        // DELETE: api/room/{id}
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound("Không tìm thấy phòng");

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }
    }
}
