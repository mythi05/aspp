using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models // QUAN TRỌNG: Đổi TênProjectCuaBan thành tên project của bạn
{
    // Enum trạng thái dịch vụ
    public enum TrangThaiDichVu
    {
        [Display(Name = "Đang sử dụng")]
        DangSuDung = 1,

        [Display(Name = "Hết hạn")]
        HetHan = 2,

        [Display(Name = "Đã hủy")]
        DaHuy = 3
    }

    [Table("DangKyDichVu")]
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập MSSV")]
        [StringLength(20)]
        [Display(Name = "MSSV")]
        public string? MSSV { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sinh viên")]
        [StringLength(100)]
        [Display(Name = "Sinh viên")]
        public string? SinhVien { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phòng")]
        [StringLength(20)]
        [Display(Name = "Phòng")]
        public string? Phong { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn dịch vụ")]
        [StringLength(100)]
        [Display(Name = "Dịch vụ")]
        public string? DichVu { get; set; } // Ví dụ: Internet, Giặt ủi...

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Giá")]
        public decimal Gia { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public TrangThaiDichVu TrangThai { get; set; }
    }
}