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
        // 1. GET ALL + SEARCH + FILTER
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GetInvoices(string? searchString, string? statusFilter)
        {
            var query = _context.Invoices
                .Include(i => i.Student)
                .Include(i => i.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s =>
                    s.Student!.FullName!.Contains(searchString) ||
                    s.Student.StudentCode!.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(x => x.Status == statusFilter);
            }

            var result = await query
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToListAsync();

            return Ok(result);
        }

        // ==========================================================
        // 2. GET BY ID
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
        // 3. CREATE
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice)
        {
            // tự động tính tiền
            invoice.TotalAmount = invoice.RoomFee + invoice.UtilityFee;
            invoice.Status = "Chưa thanh toán";

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        // ==========================================================
        // 4. UPDATE
        // ==========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, Invoice invoice)
        {
            if (id != invoice.Id)
                return BadRequest();

            var existing = await _context.Invoices.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.RoomFee = invoice.RoomFee;
            existing.UtilityFee = invoice.UtilityFee;
            existing.TotalAmount = invoice.RoomFee + invoice.UtilityFee;
            existing.Status = invoice.Status;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        // ==========================================================
        // 5. CONFIRM PAYMENT
        // ==========================================================
        [HttpPost("confirm-payment/{id}")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return NotFound();

            invoice.Status = "Đã thanh toán";

            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        // ==========================================================
        // 6. DELETE
        // ==========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return NotFound();

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}
