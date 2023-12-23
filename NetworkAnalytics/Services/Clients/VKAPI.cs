using NetworkAnalytics.Models.Entities;
using NetworkAnalytics.Models.Enums;
using NetworkAnalytics.Models.SocialNetwork;
using NetworkAnalytics.Services.Helper;
using TL;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Utils.AntiCaptcha;

namespace NetworkAnalytics.Services.Clients;

public class VKAPI
{
    private class CaptchaSolver(ThreadHelper threadHelper) : ICaptchaSolver
    {
        public readonly ThreadHelper CaptchaSolverTH = threadHelper;

        public void CaptchaIsFalse()
        {
            try
            {
                Console.WriteLine("Captcha faild");
                CaptchaSolverTH.NextResponse(new(TypeMessage.NoneNetworkAuthorize, DangerLevel.Error, "Bad captcha"));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string Solve(string url)
        {
            try
            {
                Console.WriteLine("It is necessary to solve the captcha");
                LogginHelper captchaLH = new(TypeMessage.NeedCaptcha, DangerLevel.Wanted, "Captcha")
                {
                    Description = url
                };
                captchaLH = CaptchaSolverTH.NextResponse(captchaLH);
                if (captchaLH.Type != TypeMessage.Captcha)
                    throw new Exception("Something went wrong");
                else
                    return captchaLH.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }
    }

    static private readonly ulong ApiId;
    static private readonly string SecretKey;
    public readonly ThreadHelper VKTH = new();
    private VkApi? VKClient { get; set; }

    static VKAPI()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();
        ApiId = Convert.ToUInt64(configuration["AppSettings:VKSettings:ApplicationId"] ?? throw new Exception("There is no ApiId in the file"));
        SecretKey = configuration["AppSettings:VKSettings:SecretKey"] ?? throw new Exception("There is no ApiHash in the file");
    }

    public async void AuthorizeUser(VKUser vkUser)
    {
        try
        {
            var client = new VkApi(null, new CaptchaSolver(VKTH));
            int numberAttemptsCode = 0;
            ApiAuthParams vkAuthParams = new()
            {
                AccessToken = vkUser.VkToken,
                ClientSecret = SecretKey,
                ApplicationId = ApiId,
                Login = EnterParam(vkUser.Login, TypeMessage.NeedLogin, TypeMessage.Login),
                Password = EnterParam(vkUser.Password, TypeMessage.NeedPassword, TypeMessage.Password),
                Settings = Settings.All,
                TwoFactorAuthorization = () =>
                {
                    numberAttemptsCode++;
                    if (numberAttemptsCode > 3)
                        throw new Exception("The number of requests is exceeded");
                    return EnterParam(null, TypeMessage.NeedCode, TypeMessage.Code);
                }
            };
            await client.AuthorizeAsync(vkAuthParams);

            if (!client.IsAuthorized)
                throw new Exception("You couldn't log in");
            vkUser.VkToken = client.Token;
            vkUser.Login = vkAuthParams.Login;
            vkUser.Password = vkAuthParams.Password;
            VKClient = client;
            await CreateVKUser(vkUser);
            VKTH.NextResponse(new(TypeMessage.GoodNetworkAuthorize, DangerLevel.Oke, "You good authorize")
            {
                SomeObject = vkUser
            });
        }
        catch (VkNet.Exception.UserAuthorizationFailException exVKAuth)  when (exVKAuth.Message.Contains("access_token has expired"))
        {
            vkUser.VkToken = null;
            AuthorizeUser(vkUser);
        }
        catch (Exception ex)
        {
            VKTH.NextResponse(new(TypeMessage.NoneNetworkAuthorize, DangerLevel.Error, ex.Message));
        }

        string EnterParam(string? param, TypeMessage requestType, TypeMessage expectedType)
        {
            if (!string.IsNullOrEmpty(param))
                return param;
            LogginHelper authVkLH = new(requestType, DangerLevel.Wanted, $"{requestType}");
            authVkLH = VKTH.NextResponse(authVkLH);
            if (authVkLH.Type != expectedType)
                throw new Exception("Wrong type of message");
            else
                return authVkLH.Message;
        }
    }

    public async Task GetAllDialogs()
    {
        try
        {
            if (VKClient == null)
            {
                VKTH.NextResponse(new(TypeMessage.NoneClient, DangerLevel.Error, "There is no vk client"));
                return;
            }
            var allGroups = await VKClient.Groups.GetAsync(new GroupsGetParams() { Extended = true });
            VKUser newVKUser = new();
            await CreateVKUser(newVKUser);
            List<ChatModel> listDialogs = [];
            foreach (var group in allGroups)
                listDialogs.Add(new ChatModel(group.Id, group.Name));
            VKTH.NextResponse(new(TypeMessage.Ordinary, DangerLevel.Oke, "All dialogs", new ProfileModel(newVKUser, listDialogs)));
        }
        catch (Exception ex)
        {
            VKTH.NextResponse(new(TypeMessage.Problem, DangerLevel.Error, ex.Message));
        }
    }

    public async Task GetHundredMessages(long idDialog, int offsetId)
    {
        try
        {
            if (VKClient == null)
            {
                VKTH.NextResponse(new(TypeMessage.NoneClient, DangerLevel.Error, "There is no vk client"));
                return;
            }
            DialogModel dialog = new()
            {
                Title = VKClient.Groups.GetById([], idDialog.ToString(), GroupsFields.All).FirstOrDefault()?.Name ?? "Unknown Name"
            };
            var posts = await VKClient.Wall.GetAsync(new WallGetParams
            {
                OwnerId = -idDialog,
                Count = 100,
                Offset = (ulong)offsetId
            });

            foreach (var post in posts.WallPosts)
            {
                string text;
                if (string.IsNullOrEmpty(post.Text))
                    text = $"Only {{{post.Attachment.Type.Name}}} ({post.Id})";
                else
                    text = post.Text;
                dialog.Messages.Add(new(text, post.Date));
            }
            dialog.Offset = offsetId + posts.WallPosts.Count;
            dialog.SocialNetwork = SocialNetworkEnum.VK;
            VKTH.NextResponse(new(TypeMessage.Ordinary, DangerLevel.Oke, "Last 100 text messages", dialog));
        }
        catch (Exception ex)
        {
            VKTH.NextResponse(new(TypeMessage.Problem, DangerLevel.Error, ex.Message));
        }
    }

    private async Task CreateVKUser(VKUser vkUser)
    {
        var myself = VKClient!.Users.Get(Array.Empty<long>(), ProfileFields.PhotoMax | ProfileFields.Nickname | ProfileFields.ScreenName).FirstOrDefault();
        if (myself == null)
            return;
        using (var response = new HttpClient().GetAsync(myself.PhotoMax))
        {
            var content = response.Result.Content;
            byte[] imageBytes = await content.ReadAsByteArrayAsync();
            vkUser.Icon = imageBytes;
        }
        vkUser.NickName = string.IsNullOrEmpty(myself.Nickname) ? myself.ScreenName : myself.Nickname;
    }

    public void StopSession()
    {
        VKTH.StopThread();
        VKClient = null;
    }
}
