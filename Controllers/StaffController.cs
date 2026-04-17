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
        [HttpGet]
        public async Task<IActionResult> GetAll(string? keyword, int? departmentId, int? status)
        {
            var query = _context.Staffs
                .Include(s => s.Department)
                .Include(s => s.Role)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s =>
                    s.FullName.Contains(keyword) ||
                    s.Email.Contains(keyword) ||
                    s.Phone.Contains(keyword));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(s => s.DepartmentId == departmentId);
            }

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status);
            }

            var result = await query
                .Select(s => new
                {
                    s.Id,
                    s.StaffCode,
                    s.FullName,
                    s.Phone,
                    s.Email,
                    s.HireDate,
                    s.Status,
                    s.DepartmentId,
                    DepartmentName = s.Department!.Name,
                    s.RoleId,
                    RoleName = s.Role!.Name
                })
                .ToListAsync();

            return Ok(result);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _context.Staffs
                .Include(x => x.Department)
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (s == null) return NotFound();

            return Ok(new
            {
                s.Id,
                s.StaffCode,
                s.FullName,
                s.Phone,
                s.Email,
                s.HireDate,
                s.Status,
                s.DepartmentId,
                DepartmentName = s.Department!.Name,
                s.RoleId,
                RoleName = s.Role!.Name
            });
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Staff staff)
        {
            var count = await _context.Staffs.CountAsync();
            staff.StaffCode = "NV" + (count + 1).ToString("D3");

            _context.Staffs.Add(staff);
            await _context.SaveChangesAsync();

            return Ok(staff);
        }

        // ================= UPDATE =================
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