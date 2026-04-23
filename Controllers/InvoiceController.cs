using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspp.Data;
using aspp.Models;

namespace aspp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoiceApiController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. LẤY DANH SÁCH + TÌM KIẾM + LỌC TRẠNG THÁI
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GetInvoices(string? searchString, string? statusFilter)
        {
            var query = _context.Invoices
                .Include(i => i.Student)
                .Include(i => i.Room)
                .AsQueryable();

            // Tìm kiếm theo tên SV hoặc MSSV
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s =>
                    (s.Student != null && s.Student.FullName != null && s.Student.FullName.Contains(searchString)) ||
                    (s.Student != null && s.Student.StudentCode != null && s.Student.StudentCode.Contains(searchString)));
            }

            // Lọc theo trạng thái (Chưa thanh toán / Đã thanh toán)
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
            {
                query = query.Where(x => x.Status == statusFilter);
            }

            var result = await query
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            return Ok(result);
        }

        // ==========================================================
        // 2. CHI TIẾT HÓA ĐƠN
        // ==========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Student)
                .Include(i => i.Room)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        // ==========================================================
        // 3. TẠO HÓA ĐƠN LẺ (Phí phạt, đền bù, điện nước phát sinh)
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice)
        {
            if (invoice == null) return BadRequest();

            // SỬA Ở ĐÂY: Ưu tiên lấy TotalAmount từ Frontend gửi về (trường amount trong JSON)
            // Nếu Frontend chỉ gửi totalAmount, ta gán ngược lại cho RoomFee để logic đồng nhất
            if (invoice.TotalAmount > 0)
            {
                invoice.RoomFee = invoice.TotalAmount;
            }
            else
            {
                invoice.TotalAmount = invoice.RoomFee + invoice.UtilityFee;
            }

            // Thiết lập mặc định cho hóa đơn mới
            if (string.IsNullOrEmpty(invoice.Status)) invoice.Status = "Chưa thanh toán";

            // ... các logic ngày tháng giữ nguyên ...

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        // ==========================================================
        // 4. CẬP NHẬT HÓA ĐƠN
        // ==========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] Invoice invoice)
        {
            if (id != invoice.Id) return BadRequest();

            var existing = await _context.Invoices.FindAsync(id);
            if (existing == null) return NotFound();

            // SỬA Ở ĐÂY: Cập nhật linh hoạt hơn
            existing.RoomFee = invoice.RoomFee;
            existing.UtilityFee = invoice.UtilityFee;

            // Nếu invoice gửi về có TotalAmount (từ field amount của FE) thì dùng nó, 
            // không thì mới cộng dồn Fee
            existing.TotalAmount = invoice.TotalAmount > 0 ? invoice.TotalAmount : (invoice.RoomFee + invoice.UtilityFee);

            existing.Description = invoice.Description; // Đừng quên cập nhật mô tả (Lý do thu)
            existing.Month = invoice.Month;
            existing.Year = invoice.Year;
            existing.Status = invoice.Status;
            existing.DueDate = invoice.DueDate;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }
        // ==========================================================
        // 5. XÁC NHẬN THANH TOÁN (Nút "Thu tiền" trên UI)
        // ==========================================================
        [HttpPost("confirm-payment/{id}")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            // Chuyển trạng thái
            invoice.Status = "Đã thanh toán";

            // Lưu log hoặc thực hiện nghiệp vụ khác (ví dụ: gửi mail) tại đây nếu cần

            await _context.SaveChangesAsync();
            return Ok(new { message = "Thanh toán thành công", data = invoice });
        }

        // ==========================================================
        // 6. XÓA HÓA ĐƠN
        // ==========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            // Lưu ý: Trong thực tế thường không nên xóa hóa đơn đã thanh toán để giữ lịch sử kế toán
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted" });
        }
    }
}