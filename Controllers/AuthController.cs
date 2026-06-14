using aspp.Data;
using aspp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // ================= LOGIN =================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var staff = await _context.Staffs
            .Include(s => s.Role)
            .FirstOrDefaultAsync(s => s.Username == request.Username);

        if (staff == null)
            return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });

        // 🔥 So sánh password bằng BCrypt
        bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, staff.Password);

        if (!isValid)
            return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });

        if (staff.Status == 0)
            return BadRequest(new { message = "Tài khoản bị khóa" });

        return Ok(new
        {
            id = staff.Id,
            fullName = staff.FullName,
            role = staff.Role?.Name,
            staffCode = staff.StaffCode
        });
    }

    // ================= CREATE ADMIN =================
    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin()
    {
        var exists = await _context.Staffs.AnyAsync(x => x.Username == "admin");
        if (exists)
            return BadRequest(new { message = "Admin đã tồn tại" });

        var admin = new Staff
        {
            StaffCode = "ADMIN001",
            FullName = "Administrator",
            DepartmentId = 1,
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"), // 🔥 hash password
            Status = 1,
            RoleId = 1 // 👉 nhớ role admin trong DB
        };

        _context.Staffs.Add(admin);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Tạo admin thành công" });
    }

    // ================= REGISTER STAFF =================
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var exists = await _context.Staffs.AnyAsync(x => x.Username == request.Username);
        if (exists)
            return BadRequest(new { message = "Username đã tồn tại" });

        var last = await _context.Staffs.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        int nextId = (last?.Id ?? 0) + 1;

        var staff = new Staff
        {
            StaffCode = $"NV{nextId:D3}",
            FullName = request.FullName,
            Username = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password), // 🔥 hash
            Status = 1,
            RoleId = request.RoleId
        };

        _context.Staffs.Add(staff);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Tạo tài khoản thành công" });
    }
}

// ================= DTO =================

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterRequest
{
    public string FullName { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int RoleId { get; set; }
}