using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models;

// Sử dụng Primary Constructor nếu lớp này có tham số khởi tạo, 
// tuy nhiên với Model thường ta giữ nguyên để EF Core làm việc dễ dàng hơn.
public class Contract
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string ContractCode { get; set; } = string.Empty;

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyFee { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Deposit { get; set; }
    public DateTime? CheckInDate { get; set; }
    public int Status { get; set; }

    public string? InventoryStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}