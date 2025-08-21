
namespace NADesktop.Models.Domain;

public class Report
{
    public int Id { get; set; }
    public int IdUser { get; set; }
    public string? SocialNetwork { get; set; }
    public string? Login { get; set; }
    public string? Dialog { get; set; }
    public string? ShortDialog {
        get
        {
            if (Dialog?.Length > 40)
                return Dialog?[..40] + "...";
            else return Dialog;
        }
    }
    public int CountMessages { get; set; }
    public DateTime? CreateDate { get; set; }
}
