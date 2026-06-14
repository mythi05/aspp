using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models;

public class Student
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Mã sinh viên không được để trống")]
    [StringLength(20)]
    // Chỉ cho phép nhập chữ số từ 0-9
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Mã sinh viên chỉ được phép nhập số")]
    [Display(Name = "Mã sinh viên")]
    public string StudentCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [StringLength(100)]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [StringLength(15)]
    // Kiểm tra định dạng số điện thoại Việt Nam (đầu 0, dài 10-11 số)
    [RegularExpression(@"^(0[3|5|7|8|9])([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng Việt Nam")]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Chuyên ngành")]
    public string Major { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    // Trạng thái: active (Đang ở), inactive (Đã rời đi), v.v.
    public string Status { get; set; } = "active";

    // Bổ sung thêm CCCD để quản lý lưu trú đúng chuẩn công an
    [StringLength(12)]
    [RegularExpression(@"^[0-9]{9,12}$", ErrorMessage = "CCCD/CMND phải từ 9 đến 12 số")]
    [Display(Name = "Số CCCD/CMND")]
    public string? IdCard { get; set; }

    // FK - Phòng
    public int? RoomId { get; set; }

    [ForeignKey("RoomId")]
    public Room? Room { get; set; }
}