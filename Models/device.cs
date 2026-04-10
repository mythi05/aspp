using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string DeviceName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Room { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        // ===== FOREIGN KEY =====
        public int ConditionId { get; set; }

        // navigation (nullable để tránh crash EF tracking)
        public DeviceCondition? Condition { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }
    }
}
