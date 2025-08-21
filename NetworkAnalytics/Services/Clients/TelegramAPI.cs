
using TL;
using WTelegram;
using NetworkAnalytics.Models.Enums;
using NetworkAnalytics.Services.Helper;
using NetworkAnalytics.Models.SocialNetwork;
using NetworkAnalytics.Models.Entities;

namespace NetworkAnalytics.Services.Clients;

public class TelegramAPI
{
    static private readonly string ApiId;
    static private readonly string ApiHash;
    static private readonly string SessionPath;
    private readonly Stream SessionStream;
    public readonly ThreadHelper TelegramTH = new();
    private Client? TGClient { get; set; }

    static TelegramAPI()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();
        ApiId = configuration["AppSettings:TelegramSettings:ApiId"] ?? throw new Exception("There is no ApiId in the file");
        ApiHash = configuration["AppSettings:TelegramSettings:ApiHash"] ?? throw new Exception("There is no ApiHash in the file");
        SessionPath = $"{Directory.GetCurrentDirectory()}\\" +
            $"{configuration["AppSettings:TelegramSettings:Session"] ?? throw new Exception("There is no SessionPath in the file")}";
        if (!Directory.Exists(SessionPath))
            Directory.CreateDirectory(SessionPath);
    }

    public TelegramAPI(string nameSession)
    {
        SessionStream = new FileStream($"{SessionPath}\\{nameSession}.session", FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    public async void AuthorizeUser(TelegramUser telegramUser)
    {
        LogginHelper authTgLH;
        string? 
            phoneNumber = telegramUser.Login, 
            password = telegramUser.Password;
        try
        {
            Client client = new((what) =>
            {
                switch (what)
                {
                    case "api_id": return ApiId;
                    case "api_hash": return ApiHash;

                    case "phone_number":
                        if (string.IsNullOrEmpty(phoneNumber))
                        {
                            authTgLH = TelegramTH.NextResponse(new(TypeMessage.NeedLogin, DangerLevel.Wanted, "Need phone number"));
                            if (authTgLH.Type == TypeMessage.Login)
                            {
                                telegramUser.Login = authTgLH.Message;
                                return authTgLH.Message;
                            }
                            else
                                throw new Exception("Wrong type of message");
                        }
                        else
                            return telegramUser.Login;

                    case "verification_code":
                        authTgLH = TelegramTH.NextResponse(new(TypeMessage.NeedCode, DangerLevel.Wanted, "Need code"));
                        if (authTgLH.Type == TypeMessage.Code)
                            return authTgLH.Message;
                        else
                            throw new Exception("Wrong type of message");

                    case "password":
                        if (string.IsNullOrEmpty(password))
                        {
                            authTgLH = TelegramTH.NextResponse(new(TypeMessage.NeedPassword, DangerLevel.Wanted, "Need password"));
                            if (authTgLH.Type == TypeMessage.Password)
                            {
                                telegramUser.Password = authTgLH.Message;
                                return authTgLH.Message;
                            }
                            else
                                throw new Exception("Wrong type of message");
                        }
                        else
                        {
                            password = null;
                            return telegramUser.Password;
                        }
                    default: return null;
                }
            }, SessionStream);
            var myself = await client.LoginUserIfNeeded();
            if (myself == null)
                throw new Exception("Something went wrong");
            else
            {
                TGClient = client;
                await CreateTelegramUser(telegramUser);
                authTgLH = new(TypeMessage.GoodNetworkAuthorize, DangerLevel.Oke, "You good authorize")
                {
                    SomeObject = telegramUser
                };
                TelegramTH.NextResponse(authTgLH);
            }
        }
        catch (Exception ex)
        {
            SessionStream.Close();
            authTgLH = new(TypeMessage.NoneNetworkAuthorize, DangerLevel.Error, ex.Message);
            TelegramTH.NextResponse(authTgLH);
        }
    }

    public async Task<LogginHelper> GetAllDialogs()
    {
        if (TGClient == null)
            return new (TypeMessage.NoneClient, DangerLevel.Error, "There is no telegram client");
        var allDialogs = await TGClient.Messages_GetAllDialogs();
        TelegramUser newTUser = new();
        await CreateTelegramUser(newTUser);
        List<ChatModel> listDialogs = new();

        foreach (var dialog in allDialogs.dialogs)
        {
            switch (allDialogs.UserOrChat(dialog))
            {
                case TL.User user when user.IsActive:
                    listDialogs.Add(new ChatModel(user.ID, user.MainUsername ?? user.first_name ?? user.last_name ?? user.ID.ToString()));
                    break;

                case Chat chat when chat.IsActive:
                    listDialogs.Add(new ChatModel(chat.ID, chat.Title));
                    break;

                case Channel channel when channel.IsActive:
                    listDialogs.Add(new ChatModel(channel.ID, channel.Title));
                    break;

                default:
                    continue;
            }
        }
        return new(TypeMessage.Ordinary, DangerLevel.Oke, "All dialogs", new ProfileModel(newTUser, listDialogs));
    }

    public async Task<LogginHelper> GetHundredMessages(long idDialog, int offsetId)
    {
        if (TGClient == null)
            return new(TypeMessage.NoneClient, DangerLevel.Error, "There is no telegram client");
        var dialogs = await TGClient.Messages_GetAllDialogs();
        var dialogBase = dialogs.dialogs.FirstOrDefault(dialog => dialog.Peer.ID == idDialog);
        DialogModel dialog = new();
        InputPeer peer;

        switch (dialogs.UserOrChat(dialogBase))
        {
            case TL.User user when user.IsActive:
                peer = user;
                dialog.Title = user.MainUsername ?? user.first_name ?? user.last_name ?? user.ID.ToString();
                break;

            case Chat chat when chat.IsActive:
                peer = chat;
                dialog.Title = chat.Title;
                break;
            case Channel channel when channel.IsActive:
                peer = channel;
                dialog.Title = channel.Title;
                break;
            default:
                throw new Exception($"There is no chat with id({idDialog})");
        }

        var dialogHistory = await TGClient.Messages_GetHistory(peer, offset_id: offsetId);
        foreach (var baseMessage in dialogHistory.Messages)
        {
            if (baseMessage is Message message)
            {
                string text;
                if (string.IsNullOrEmpty(message.message))
                    text = $"Only {{{message.media}}} ({message.id})";
                else
                    text = message.message;
                dialog.Messages.Add(new(text, message.Date));
            }
        }
        dialog.Offset = dialogHistory.Messages.Length > 0 ? dialogHistory.Messages.Last().ID : offsetId;
        dialog.SocialNetwork = SocialNetworkEnum.Telegram;
        return new(TypeMessage.Ordinary, DangerLevel.Oke, "Last 100 text messages", dialog);
    }


    private async Task CreateTelegramUser(TelegramUser tUser)
    {
        var myself = TGClient!.User;
        using MemoryStream fileStream = new();
        await TGClient!.DownloadProfilePhotoAsync(myself, fileStream);
        fileStream.Close();
        tUser.Icon = fileStream.GetBuffer();
        tUser.NickName = myself.username;
        tUser.Login = myself.phone;
    }

    public void StopSession()
    {
        TelegramTH.StopThread();
        TGClient = null;
        SessionStream.Close();
    }
}
