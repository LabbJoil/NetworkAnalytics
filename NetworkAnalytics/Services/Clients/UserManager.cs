
using Microsoft.Extensions.Caching.Memory;
using NetworkAnalytics.Models.Entities;

namespace NetworkAnalytics.Services.Clients;

public class UserManager
{
    private static readonly IMemoryCache UserSessions = new MemoryCache(new MemoryCacheOptions());

    public readonly int IdUser;
    public TelegramAPI? TelegramClient { get; set; }
    public VKAPI? VKClient {  get; set; }

    private UserManager(int idUser)
    {
        IdUser = idUser;
    }

    public static void AddUser(User user)
    {
        //string key = UserConverter.HashUserSession(user.Id.ToString() + DateTime.Now.ToString());
        UserManager userManager = new(user.Id);
        UserSessions.Set(user.Id, userManager, TimeSpan.FromHours(2));
    }

    public static  UserManager? GetUser(int key)
    {
        if(UserSessions.Get<UserManager>(key) is UserManager userManager)
        {
            UserSessions.Set(key, userManager, TimeSpan.FromHours(2));
            return userManager;
        }
        return null;
    }
}
