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
    // (Lưu lại phòng ở thời điểm thu tiền, đề phòng sau này SV chuyển phòng)
    [Required]
    public int RoomId { get; set; }
    [ForeignKey("RoomId")]
    public Room? Room { get; set; }

    // --- Thông tin thời gian ---
    [Required]
    public int Month { get; set; } // Tháng (VD: 3)

    [Required]
    public int Year { get; set; } // Năm (VD: 2026)

    // --- Chi tiết các khoản phí ---
    // Dùng kiểu decimal cho tiền tệ. Khai báo (18,2) để tránh lỗi warning của Entity Framework
    [Column(TypeName = "decimal(18,2)")]
    public decimal RoomFee { get; set; } = 0; // Phí phòng (VD: 750000)

    [Column(TypeName = "decimal(18,2)")]
    public decimal UtilityFee { get; set; } = 0; // Điện/Nước/Khác (VD: 250000)

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } = 0; // Tổng cộng (Sẽ bằng RoomFee + UtilityFee)

    // --- Thời hạn và Trạng thái ---
    public DateTime DueDate { get; set; } // Hạn thanh toán

    [StringLength(50)]
    public string Status { get; set; } = "Chưa thanh toán"; // Trạng thái: "Chưa thanh toán", "Đã thanh toán", "Quá hạn"
}