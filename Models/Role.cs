using System.ComponentModel.DataAnnotations;
using aspp.Models; // 🔥 THÊM DÒNG NÀY

namespace aspp.Models // 🔥 THÊM NAMESPACE
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public ICollection<Staff>? Staffs { get; set; }
    }
}