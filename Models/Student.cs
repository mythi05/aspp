using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models;

public class Student
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string StudentCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string Major { get; set; } = string.Empty;

    // 🔥 FIX: thống nhất active / inactive
    [StringLength(50)]
    public string Status { get; set; } = "active";

    // FK
    public int? RoomId { get; set; }

    [ForeignKey("RoomId")]
    public Room? Room { get; set; }
}