using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspp.Models;
using aspp.Data;

namespace aspp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MaintenanceApiController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. GET ALL + SEARCH + FILTER
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(string? search, int? level)
        {
            var query = _context.Maintenances.AsQueryable();

            // search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.Room.Contains(search) ||
                    x.Reporter.Contains(search));
            }

            // filter level
            if (level.HasValue)
            {
                query = query.Where(x => (int)x.Level == level);
            }

            var data = await query
                .OrderByDescending(x => x.ReportDate)
                .ToListAsync();

            return Ok(data);
        }

        // ==========================================================
        // 2. DASHBOARD
        // ==========================================================
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var total = await _context.Maintenances.CountAsync();
            var pending = await _context.Maintenances.CountAsync(x => x.Status == MaintenanceStatus.Pending);
            var processing = await _context.Maintenances.CountAsync(x => x.Status == MaintenanceStatus.Processing);
            var completed = await _context.Maintenances.CountAsync(x => x.Status == MaintenanceStatus.Completed);

            return Ok(new
            {
                Total = total,
                Pending = pending,
                Processing = processing,
                Completed = completed
            });
        }

        // ==========================================================
        // 3. GET BY ID
        // ==========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Maintenances.FindAsync(id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // ==========================================================
        // 4. CREATE
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Maintenance model)
        {
            model.Status = MaintenanceStatus.Pending;
            model.ReportDate = DateTime.Now;

            _context.Maintenances.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // ==========================================================
        // 5. UPDATE
        // ==========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Maintenance model)
        {
            var item = await _context.Maintenances.FindAsync(id);
            if (item == null)
                return NotFound();

            item.Room = model.Room;
            item.Reporter = model.Reporter;
            item.Category = model.Category;
            item.Description = model.Description;
            item.Level = model.Level;

            await _context.SaveChangesAsync();

            return Ok(item);
        }

        // ==========================================================
        // 6. START PROCESS
        // ==========================================================
        [HttpPut("start/{id}")]
        public async Task<IActionResult> StartProcess(int id)
        {
            var item = await _context.Maintenances.FindAsync(id);
            if (item == null)
                return NotFound();

            item.Status = MaintenanceStatus.Processing;

            await _context.SaveChangesAsync();

            return Ok(item);
        }

        // ==========================================================
        // 7. COMPLETE
        // ==========================================================
        [HttpPut("complete/{id}")]
        public async Task<IActionResult> Complete(int id)
        {
            var item = await _context.Maintenances.FindAsync(id);
            if (item == null)
                return NotFound();

            item.Status = MaintenanceStatus.Completed;

            await _context.SaveChangesAsync();

            return Ok(item);
        }

        // ==========================================================
        // 8. DELETE
        // ==========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Maintenances.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.Maintenances.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Deleted");
        }
    }
}
