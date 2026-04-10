using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models
{
    // Priority Level
    public enum MaintenanceLevel
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    // Status
    public enum MaintenanceStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3
    }

    [Table("Maintenances")]
    public class Maintenance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Room { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Reporter { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // Electricity, Water, Furniture...

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public MaintenanceLevel Level { get; set; }

        [Required]
        public DateTime ReportDate { get; set; } = DateTime.Now;

        [Required]
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;
    }
}
