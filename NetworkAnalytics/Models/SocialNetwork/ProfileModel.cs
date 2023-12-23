
using NetworkAnalytics.Interfaces;

namespace NetworkAnalytics.Models.SocialNetwork;

public class ProfileModel
{
    public ISocialNetworkUser? SNUser { get; set; }
    public List<ChatModel>? Dialogs { get; set; }
    public ProfileModel(ISocialNetworkUser snUser, List<ChatModel>? dialogs)
    {
        SNUser = snUser;
        Dialogs = dialogs;
    }
}
