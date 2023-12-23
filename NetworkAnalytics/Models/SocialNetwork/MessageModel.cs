
namespace NetworkAnalytics.Models.SocialNetwork;

public class MessageModel
{
    public string? Text { get; set; }
    public DateTime? Date { get; set; }
    public MessageModel(string? text, DateTime? date)
    {
        Text = text;
        Date = date;
    }
}
