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

        // ================= GET ALL =================
        // GET: api/thietbi
        [HttpGet]
        public async Task<IActionResult> GetAll(string? keyword, string? phong, TinhTrangThietBi? tinhTrang)
        {
            var query = _context.ThietBis.AsQueryable();

            // 🔍 search theo tên
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.TenThietBi.Contains(keyword));
            }

            // 📍 filter phòng
            if (!string.IsNullOrEmpty(phong))
            {
                query = query.Where(x => x.Phong == phong);
            }

            // 📊 filter tình trạng
            if (tinhTrang.HasValue)
            {
                query = query.Where(x => x.TinhTrang == tinhTrang);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

        // ================= GET BY ID =================
        // GET: api/thietbi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _context.ThietBis.FindAsync(id);

            if (data == null)
                return NotFound("Không tìm thấy thiết bị");

            return Ok(data);
        }

        // ================= CREATE =================
        // POST: api/thietbi
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThietBi model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.ThietBis.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // ================= UPDATE =================
        // PUT: api/thietbi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ThietBi model)
        {
            if (id != model.Id)
                return BadRequest("Id không khớp");

            var existing = await _context.ThietBis.FindAsync(id);
            if (existing == null)
                return NotFound("Không tìm thấy thiết bị");

            // update field
            existing.TenThietBi = model.TenThietBi;
            existing.Phong = model.Phong;
            existing.Loai = model.Loai;
            existing.SoLuong = model.SoLuong;
            existing.TinhTrang = model.TinhTrang;
            existing.NgayMua = model.NgayMua;
            existing.GiaTri = model.GiaTri;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // ================= DELETE =================
        // DELETE: api/thietbi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.ThietBis.FindAsync(id);

            if (data == null)
                return NotFound("Không tìm thấy thiết bị");

            _context.ThietBis.Remove(data);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }
    }
}
