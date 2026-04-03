using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models // NHỚ ĐỔI TÊN PROJECT CỦA BẠN
{
    // Enum Mức độ vi phạm
    public enum MucDoViPham
    {
        [Display(Name = "Nhẹ")]
        Nhe = 1,

        [Display(Name = "Trung bình")]
        TrungBinh = 2,

        [Display(Name = "Nặng")]
        Nang = 3,

        [Display(Name = "Nghiêm trọng")]
        NghiemTrong = 4
    }

    // Enum Trạng thái xử lý
    public enum TrangThaiViPham
    {
        [Display(Name = "Chưa xử lý")]
        ChuaXuLy = 1,

        [Display(Name = "Đã xử lý")]
        DaXuLy = 2
    }

    [Table("ViPham")]
    public class rule
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

        [Required(ErrorMessage = "Vui lòng nhập loại vi phạm")]
        [StringLength(100)]
        [Display(Name = "Loại vi phạm")]
        public string? LoaiViPham { get; set; } // Ví dụ: Gây ồn, Vệ sinh...

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày")]
        public DateTime NgayViPham { get; set; }

        [Required]
        [Display(Name = "Mức độ")]
        public MucDoViPham MucDo { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền phạt")]
        public decimal TienPhat { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public TrangThaiViPham TrangThai { get; set; }
    }
}