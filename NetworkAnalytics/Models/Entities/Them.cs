
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkAnalytics.Models.Entities;

[Table("Them")]
public class Them
{
    [Key, Required]
    public int Id { get; set; }
    [Required]
    public int IdReport { get; set; }
    [Required]
    public float Sport { get; set; }
    [Required]
    public float Politics { get; set; }
    [Required]
    public float Art { get; set; }
    [Required]
    public float Technic { get; set; }
}
