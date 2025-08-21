
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkAnalytics.Models.Entities;

[Table("Tonality")]
public class Tonality
{
    [Key, Required]
    public int Id { get; set; }
    [Required]
    public int IdReport { get; set; }
    [Required]
    public float Positive { get; set; }
    [Required]
    public float Median { get; set; }
    [Required]
    public float Negative { get; set; }
}
