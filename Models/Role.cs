using System.ComponentModel.DataAnnotations;

public class Role
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public ICollection<Staff>? Staffs { get; set; }
}
