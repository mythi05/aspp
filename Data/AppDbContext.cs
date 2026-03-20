using aspp.Models;
using DormitoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace aspp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }
    // Thêm dòng này cho bảng Sinh viên
    public DbSet<Student> Students { get; set; }
    // Thêm bảng Giao dịch nhận/trả phòng
    public DbSet<RoomTransaction> RoomTransactions { get; set; }
    // Thêm bảng Quản lý phí (Phiếu thu)
    public DbSet<Invoice> Invoices { get; set; }
}