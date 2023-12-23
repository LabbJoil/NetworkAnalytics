
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkAnalytics.Models.Entities;

[Table("Report")]
public class Report
{
    [Key, Required]
    public int Id { get; set; }
    [Required]
    public int IdUser { get; set; }
    [Required]
    public string? SocialNetwork { get; set; }
    [Required]
    public string? Login { get; set; }
    [Required]
    public string? Dialog { get; set; }
    [Required]
    public int CountMessages { get; set; }
    [Required]
    public DateTime? CreateDate { get; set; }
}
