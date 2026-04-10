using System.ComponentModel.DataAnnotations;

namespace aspp.Models;


public class Room
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string RoomName { get; set; } = string.Empty; // VD: A101

    [Required]
    [StringLength(50)]
    public string Building { get; set; } = string.Empty; // VD: Tòa A

    [Required]
    [StringLength(50)]
    public string Floor { get; set; } = string.Empty; // VD: Tầng 1

    [Required]
    [StringLength(50)]
    public string RoomType { get; set; } = string.Empty; // VD: Phòng 4 người

    public int CurrentOccupancy { get; set; } = 0; // Số người đang ở (VD: 2)

    public int MaxCapacity { get; set; } // Sức chứa tối đa (VD: 4)

    public int DangO { get; set; } = 0;
    public decimal Price { get; set; } // Giá phòng (VD: 750000)

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Còn trống"; // Còn trống, Đã đầy, Bảo trì
}