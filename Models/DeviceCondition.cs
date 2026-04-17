using System.ComponentModel.DataAnnotations;

namespace aspp.Models
{
    public class DeviceCondition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
