
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkAnalytics.Models.Entities;

[Table("CommonWord")]
public class CommonWord
{
    [Key, Required]
    public int Id { get; set; }
    [Required]
    public int IdReport { get; set; }
    [Required]
    public string? Word { get; set; }
    [Required]
    public int NumberRepetitions { get; set; }
}
