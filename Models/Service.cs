using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models
{
    // Service status enum
    public enum ServiceStatus
    {
        [Display(Name = "In Use")]
        Active = 1,

        [Display(Name = "Expired")]
        Expired = 2,

        [Display(Name = "Cancelled")]
        Cancelled = 3
    }

    [Table("ServiceRegistrations")]
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        [StringLength(20)]
        [Display(Name = "Student ID")]
        public string? StudentId { get; set; }

        [Required(ErrorMessage = "Student name is required")]
        [StringLength(100)]
        [Display(Name = "Student Name")]
        public string? StudentName { get; set; }

        [Required(ErrorMessage = "Room is required")]
        [StringLength(20)]
        [Display(Name = "Room")]
        public string? Room { get; set; }

        [Required(ErrorMessage = "Service name is required")]
        [StringLength(100)]
        [Display(Name = "Service Name")]
        public string? ServiceName { get; set; } // Internet, Laundry,...

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ServiceStatus Status { get; set; }
    }
}
