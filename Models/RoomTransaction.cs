using aspp.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models;
public class RoomTransaction
{
    [Key]
    public int Id { get; set; }

    // --- Khóa ngoại liên kết với Sinh viên ---
    [Required]
    public int StudentId { get; set; }
    [ForeignKey("StudentId")]
    public Student? Student { get; set; }

    // --- Khóa ngoại liên kết với Phòng ---
    [Required]
    public int RoomId { get; set; }
    [ForeignKey("RoomId")]
    public Room? Room { get; set; }

    // --- Thông tin giao dịch ---
    [Required]
    [StringLength(20)]
    public string TransactionType { get; set; } = string.Empty; // "Check-in" hoặc "Check-out"

    public DateTime TransactionDate { get; set; } = DateTime.Now; // Ngày giờ thực hiện

    [StringLength(255)]
    public string? Note { get; set; } // Ghi chú

    [StringLength(100)]
    public string HandledBy { get; set; } = "Admin"; // Người xử lý (Tạm thời để chuỗi, sau này có bảng User thì nối khóa ngoại)
}