using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace aspp.Models
{
    public class Staff
    {
        [Key]
        public int Id { get; set; }

        // ❗ Không cho client đụng vào
        [BindNever]
        public string? StaffCode { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại chỉ được nhập số (10-11 số)")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = "";

        [Required]
        public DateTime HireDate { get; set; }

        public int Status { get; set; } = 1;

        [Required]
        public int DepartmentId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public virtual Department? Department { get; set; }

        [Required]
        public int RoleId { get; set; }

        public virtual Role? Role { get; set; }
    }
}