using aspp.Models;
using System.ComponentModel.DataAnnotations;

public class Staff
{
    public int Id { get; set; }

    [Required]
    public string StaffCode { get; set; } = "";

    [Required]
    public string FullName { get; set; } = "";

    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";

    public DateTime HireDate { get; set; }

    public int Status { get; set; } // 1: active

    // FK
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }
}
