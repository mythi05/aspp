using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models
{
    public enum ViolationLevel
    {
        Light = 1,
        Medium = 2,
        Severe = 3,
        Critical = 4
    }

    public enum ViolationStatus
    {
        Pending = 1,
        Processed = 2
    }

    [Table("Violations")]
    public class Violation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Room { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ViolationType { get; set; } = string.Empty;

        [Required]
        public DateTime ViolationDate { get; set; }

        [Required]
        public ViolationLevel Level { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal Fine { get; set; }

        [Required]
        public ViolationStatus Status { get; set; }
    }
}
