using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models
{
    // Enum định nghĩa các trạng thái của thiết bị
    public enum TinhTrangThietBi
    {
        [Display(Name = "Tốt")]
        Tot = 1,

        [Display(Name = "Hư hỏng")]
        HuHong = 2,

        [Display(Name = "Hỏng")]
        Hong = 3
    }

    [Table("ThietBi")] // Tên bảng trong Database
    public class ThietBi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thiết bị")]
        [StringLength(255)]
        [Display(Name = "Tên thiết bị")]
        public string TenThietBi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phòng")]
        [StringLength(50)]
        [Display(Name = "Phòng")]
        public string Phong { get; set; } // Ví dụ: A101 (Có thể làm Foreign Key liên kết bảng Phong nếu cần)

        [Required(ErrorMessage = "Vui lòng chọn loại thiết bị")]
        [StringLength(100)]
        [Display(Name = "Loại")]
        public string Loai { get; set; } // Ví dụ: Nội thất, Điện tử...

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Required]
        [Display(Name = "Tình trạng")]
        public TinhTrangThietBi TinhTrang { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày mua")]
        public DateTime NgayMua { get; set; }

        [Column(TypeName = "decimal(18,0)")] // Lưu tiền tệ VNĐ không cần số thập phân
        [Display(Name = "Giá trị")]
        public decimal GiaTri { get; set; }
    }
}