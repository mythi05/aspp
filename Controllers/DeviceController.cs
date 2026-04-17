using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeviceController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL (CÓ LỌC) =================
        // GET: api/device?keyword=...&room=...&conditionId=...
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] string? room, [FromQuery] int? conditionId)
        {
            // Dùng .Include(x => x.Condition) nếu bạn muốn lấy luôn thông tin tên trạng thái
            var query = _context.Devices.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.DeviceName.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(room))
            {
                query = query.Where(x => x.Room == room);
            }

            if (conditionId != null)
            {
                query = query.Where(x => x.ConditionId == conditionId);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _context.Devices.FindAsync(id);
            if (data == null) return NotFound("Device not found");
            return Ok(data);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Device model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Devices.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Device model)
        {
            if (id != model.Id) return BadRequest("Id mismatch");

            var existing = await _context.Devices.FindAsync(id);
            if (existing == null) return NotFound("Device not found");

            existing.DeviceName = model.DeviceName;
            existing.Room = model.Room;
            existing.Type = model.Type;
            existing.Quantity = model.Quantity;
            existing.ConditionId = model.ConditionId; // Cập nhật FK
            existing.PurchaseDate = model.PurchaseDate;
            existing.Value = model.Value;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.Devices.FindAsync(id);
            if (data == null) return NotFound("Device not found");

            _context.Devices.Remove(data);
            await _context.SaveChangesAsync();
            return Ok("Deleted successfully");
        }
    }
}