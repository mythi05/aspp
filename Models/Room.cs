using System.ComponentModel.DataAnnotations;

namespace aspp.Models;

public class Room
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string RoomName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Building { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Floor { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string RoomType { get; set; } = string.Empty;

    // ❌ XOÁ CurrentOccupancy + DangO

    public int MaxCapacity { get; set; }

    public decimal Price { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Còn trống";
}