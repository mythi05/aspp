using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViolationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ViolationController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================
        // 1. GET ALL
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetAll(string? search, ViolationLevel? level)
        {
            var query = _context.Violations.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v =>
                    v.StudentId.Contains(search) ||
                    v.StudentName.Contains(search));
            }

            if (level != null)
            {
                query = query.Where(v => v.Level == level);
            }

            var data = await query
                .OrderByDescending(v => v.ViolationDate)
                .ToListAsync();

            return Ok(data);
        }

        // =========================================
        // 2. GET BY ID
        // =========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Violations.FindAsync(id);

            if (item == null)
                return NotFound("Violation not found");

            return Ok(item);
        }

        // =========================================
        // 3. CREATE
        // =========================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Violation model)
        {
            model.Status = ViolationStatus.Pending;

            _context.Violations.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // =========================================
        // 4. UPDATE
        // =========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Violation model)
        {
            if (id != model.Id)
                return BadRequest("Id mismatch");

            var existing = await _context.Violations.FindAsync(id);
            if (existing == null)
                return NotFound("Violation not found");

            existing.StudentId = model.StudentId;
            existing.StudentName = model.StudentName;
            existing.Room = model.Room;
            existing.ViolationType = model.ViolationType;
            existing.ViolationDate = model.ViolationDate;
            existing.Level = model.Level;
            existing.Fine = model.Fine;
            existing.Status = model.Status;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // =========================================
        // 5. MARK AS PROCESSED
        // =========================================
        [HttpPost("process/{id}")]
        public async Task<IActionResult> MarkProcessed(int id)
        {
            var item = await _context.Violations.FindAsync(id);

            if (item == null)
                return NotFound("Violation not found");

            item.Status = ViolationStatus.Processed;

            await _context.SaveChangesAsync();

            return Ok(item);
        }

        // =========================================
        // 6. DELETE
        // =========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Violations.FindAsync(id);

            if (item == null)
                return NotFound("Violation not found");

            _context.Violations.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}
