using aspp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DormitoryAPI.Models;

public class Student
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string StudentCode { get; set; } = string.Empty; // MSSV (VD: SV001)

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty; // Họ và tên[EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; } // Email (VD: nguyenvana@email.com)

    [StringLength(10)]
    public string Gender { get; set; } = string.Empty; // Giới tính (Nam/Nữ)

    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty; // Số điện thoại

    [StringLength(100)]
    public string Major { get; set; } = string.Empty; // Ngành học (VD: Công nghệ thông tin)

    [StringLength(50)]
    public string Status { get; set; } = "Đang ở"; // Trạng thái (VD: Đang ở, Đã rời đi)

    // --- THIẾT LẬP KHÓA NGOẠI (RELATIONSHIP) ---
    // Liên kết sinh viên này với bảng Room (Phòng)
    public int? RoomId { get; set; }

    [ForeignKey("RoomId")]
    public Room? Room { get; set; }
}