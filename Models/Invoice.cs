using aspp.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    // --- Khóa ngoại: Phiếu thu này của Sinh viên nào? ---
    [Required]
    public int StudentId { get; set; }

    [ForeignKey("StudentId")]
    public Student? Student { get; set; }

    // --- Khóa ngoại: Thu tiền của Phòng nào? ---
    [Required]
    public int RoomId { get; set; }

    [ForeignKey("RoomId")]
    public Room? Room { get; set; }

    // --- MÔ TẢ KHOẢN THU (Quan trọng để khớp với Frontend) ---
    // Trường này sẽ lưu: "Tiền phòng trọn kỳ (06 tháng)" hoặc "Phí phạt hỏng khóa cửa..."
    [StringLength(500)]
    public string? Description { get; set; }

    // --- Thông tin thời gian ---
    [Required]
    public int Month { get; set; }

    [Required]
    public int Year { get; set; }

    // --- Chi tiết các khoản phí ---
    [Column(TypeName = "decimal(18,2)")]
    public decimal RoomFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UtilityFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } = 0;

    // --- Thời hạn và Trạng thái ---
    public DateTime DueDate { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Chưa thanh toán";
}