namespace NADesktop.Models.SocialNetwork;

public class MessageModel
{
    public string? Text { get; set; }
    public string? ShortText
    {
        get
        {
            if (Text?.Length > 40)
                return Text?[..40] + "...";
            else return Text;
        }
    }
    public DateTime? Date { get; set; }
    public bool IsSelected { get; set; }
}
