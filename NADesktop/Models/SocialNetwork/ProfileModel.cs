using System.Text.Json.Serialization;

namespace NADesktop.Models.SocialNetwork;

public class ProfileModel
{
    public SocialNetworkUser? SNUser { get; set; }
    public List<ChatModel>? Dialogs { get; set; }
}
