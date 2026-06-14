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

        [HttpGet]
        public async Task<IActionResult> GetAll(string? keyword, int? departmentId, int? status)
        {
            var query = _context.Staffs
                .Include(s => s.Department)
                .Include(s => s.Role)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(s => s.FullName.Contains(keyword) ||
                                       s.Email.Contains(keyword) ||
                                       s.StaffCode.Contains(keyword));
            }

            if (departmentId.HasValue) query = query.Where(s => s.DepartmentId == departmentId);
            if (status.HasValue) query = query.Where(s => s.Status == status);

            var result = await query.OrderByDescending(x => x.Id).Select(s => new {
                s.Id,
                s.StaffCode,
                s.FullName,
                s.Phone,
                s.Email,
                s.HireDate,
                s.Status,
                s.DepartmentId,
                DepartmentName = s.Department != null ? s.Department.Name : "N/A",
                s.RoleId,
                RoleName = s.Role != null ? s.Role.Name : "N/A"
            }).ToListAsync();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Staff staff)
        {
            // ❗ Bỏ validate StaffCode (phòng trường hợp hệ thống vẫn check)
            ModelState.Remove("StaffCode");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var last = await _context.Staffs
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();

                int nextId = (last?.Id ?? 0) + 1;

                // ✅ Auto generate mã
                staff.StaffCode = $"NV{nextId:D3}";

                // ❗ tránh EF tạo mới quan hệ
                staff.Department = null;
                staff.Role = null;

                _context.Staffs.Add(staff);
                await _context.SaveChangesAsync();

                return Ok(staff);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Staff staff)
        {
            if (id != staff.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _context.Staffs.FindAsync(id);
            if (existing == null) return NotFound();

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null) return NotFound();

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Xóa thành công" });
        }
    }
}