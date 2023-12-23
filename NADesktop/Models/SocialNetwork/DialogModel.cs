using NADesktop.Models.Enums;

namespace NADesktop.Models.SocialNetwork;

public class DialogModel
{
    public SocialNetworkEnum SocialNetwork { get; set; }
    public string? Title { get; set; }
    public long Offset { get; set; }
    public List<MessageModel>? Messages { get; set; }
    public DialogModel() { }
}
