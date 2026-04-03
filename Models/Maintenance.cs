using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models // NHỚ ĐỔI TÊN PROJECT
{
    // Enum Mức độ
    public enum MucDoBaoTri
    {
        [Display(Name = "Thấp")]
        Thap = 1,

        [Display(Name = "Trung bình")]
        TrungBinh = 2,

        [Display(Name = "Cao")]
        Cao = 3,

        [Display(Name = "Khẩn cấp")]
        KhanCap = 4
    }

    // Enum Trạng thái
    public enum TrangThaiBaoTri
    {
        [Display(Name = "Chờ xử lý")]
        ChoXuLy = 1,

        [Display(Name = "Đang xử lý")]
        DangXuLy = 2,

        [Display(Name = "Hoàn thành")]
        HoanThanh = 3
    }

    [Table("BaoTriSuaChua")]
    public class Maintenance
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phòng")]
        [StringLength(20)]
        [Display(Name = "Phòng")]
        public string? Phong { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập người báo cáo")]
        [StringLength(100)]
        [Display(Name = "Người báo cáo")]
        public string? NguoiBaoCao { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập loại")]
        [StringLength(50)]
        [Display(Name = "Loại")]
        public string? Loai { get; set; } // Ví dụ: Điện, Nước, Nội thất...

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required]
        [Display(Name = "Mức độ")]
        public MucDoBaoTri MucDo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày báo cáo")]
        public DateTime NgayBaoCao { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public TrangThaiBaoTri TrangThai { get; set; }
    }
}