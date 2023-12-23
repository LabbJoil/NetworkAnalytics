
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkAnalytics.Models.Entities;

[Table("PartsSpeech")]
public class PartsSpeech
{
    [Key, Required]
    public int Id { get; set; }
    [Required]
    public int IdReport { get; set; }
    public int NOUN { get; set; }
    public int DET { get; set; }
    public int ADJ { get; set; }
    public int PART { get; set; }
    public int PRON { get; set; }
    public int ADP { get; set; }
    public int VERB { get; set; }
    public int NUM { get; set; }
    public int ADV { get; set; }
    public int INTJ { get; set; }
    public int SYM { get; set; }
    public int X { get; set; }
}
