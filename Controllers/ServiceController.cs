using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspp.Models;
using aspp.Data;

namespace aspp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceApiController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. GET ALL + SEARCH + FILTER + AUTO EXPIRE
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GetServices(string? search, string? serviceName)
        {
            var today = DateTime.Now.Date;

            // Auto expire services
            var expiredList = await _context.Services
                .Where(s => s.Status == ServiceStatus.Active && s.EndDate < today)
                .ToListAsync();

            if (expiredList.Any())
            {
                foreach (var item in expiredList)
                {
                    item.Status = ServiceStatus.Expired;
                }

                await _context.SaveChangesAsync();
            }

            var query = _context.Services.AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.StudentId!.Contains(search) ||
                    s.StudentName!.Contains(search) ||
                    s.Room!.Contains(search));
            }

            // Filter by service name
            if (!string.IsNullOrEmpty(serviceName))
            {
                query = query.Where(s => s.ServiceName == serviceName);
            }

            var result = await query
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            return Ok(result);
        }

        // ==========================================================
        // 2. DASHBOARD
        // ==========================================================
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var total = await _context.Services.CountAsync();
            var active = await _context.Services.CountAsync(s => s.Status == ServiceStatus.Active);
            var expired = await _context.Services.CountAsync(s => s.Status == ServiceStatus.Expired);
            var cancelled = await _context.Services.CountAsync(s => s.Status == ServiceStatus.Cancelled);
            var revenue = await _context.Services.SumAsync(s => s.Price);

            return Ok(new
            {
                TotalServices = total,
                ActiveServices = active,
                ExpiredServices = expired,
                CancelledServices = cancelled,
                TotalRevenue = revenue
            });
        }

        // ==========================================================
        // 3. GET BY ID
        // ==========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            return Ok(service);
        }

        // ==========================================================
        // 4. CREATE
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] Service service)
        {
            service.Status = ServiceStatus.Active;

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return Ok(service);
        }

        // ==========================================================
        // 5. CANCEL SERVICE
        // ==========================================================
        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            service.Status = ServiceStatus.Cancelled;

            await _context.SaveChangesAsync();

            return Ok(service);
        }

        // ==========================================================
        // 6. DELETE
        // ==========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
