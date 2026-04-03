using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aspp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        // ================= GET ALL =================
        // GET: api/staff
        [HttpGet]
        public async Task<IActionResult> GetAll(string? keyword, int? departmentId, int? status)
        {
            var query = _context.Staffs
                .Include(s => s.Department)
                .Include(s => s.Role)
                .AsQueryable();

            // 🔍 Search
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s =>
                    s.FullName.Contains(keyword) ||
                    s.Email.Contains(keyword) ||
                    s.Phone.Contains(keyword));
            }

            // 📂 Filter phòng ban
            if (departmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == departmentId);
            }

            // 📊 Filter trạng thái
            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

        // ================= GET BY ID =================
        // GET: api/staff/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var staff = await _context.Staffs
                .Include(s => s.Department)
                .Include(s => s.Role)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (staff == null) return NotFound();

            return Ok(staff);
        }

        // ================= CREATE =================
        // POST: api/staff
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Staff staff)
        {
            // 🔥 Auto generate mã NV
            var count = await _context.Staffs.CountAsync();
            staff.StaffCode = "NV" + (count + 1).ToString("D3");

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            return Ok(staff);
        }

        // ================= UPDATE =================
        // PUT: api/staff/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Staff staff)
        {
            if (id != staff.Id)
                return BadRequest("Id không khớp");

            var existing = await _context.Staffs.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.FullName = staff.FullName;
            existing.Phone = staff.Phone;
            existing.Email = staff.Email;
            existing.HireDate = staff.HireDate;
            existing.Status = staff.Status;
            existing.DepartmentId = staff.DepartmentId;
            existing.RoleId = staff.RoleId;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // ================= DELETE =================
        // DELETE: api/staff/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
                return NotFound();

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }
    }
}
