
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetworkAnalytics.Models.Entities;
using NetworkAnalytics.Models.Enums;
using NetworkAnalytics.Models.SocialNetwork;
using NetworkAnalytics.Services.Clients;
using NetworkAnalytics.Services.Entities;
using NetworkAnalytics.Services.Helper;
using VkNet.Enums.StringEnums;

namespace NetworkAnalytics.Services.DataAccess;

public class TelegramHelper(ContextDB contextDB)
{
    private readonly ContextDB ContextDatabase = contextDB;

    private async Task<LogginHelper> TelegramAuthorizeUser(UserManager userSession)
    {
        if (userSession.TelegramClient != null)
        {
            //userSession.TelegramClient.TelegramTH.NextRequest(new(TypeMessage.StopThread, DangerLevel.Wanted, ""));
            userSession.TelegramClient.StopSession();
        }
        TelegramUser telegramUser = await ContextDatabase.TelegramUsers.FirstOrDefaultAsync(tUser => tUser.IdUser == userSession.IdUser) ??
            new TelegramUser()
            {
                IdUser = userSession.IdUser
            };
        if (string.IsNullOrEmpty(telegramUser.NameSession))
        {
            while (true)
            {
                string randomString = Guid.NewGuid().ToString("N");
                TelegramUser? userSameSession = await ContextDatabase.TelegramUsers.FirstOrDefaultAsync(tUser => tUser.NameSession == randomString);
                if (userSameSession == null)
                {
                    telegramUser.NameSession = $"Session{randomString}";
                    break;
                }
            }
        }
        userSession.TelegramClient = new TelegramAPI(telegramUser.NameSession);
        LogginHelper startThreadLE = userSession.TelegramClient.TelegramTH.StartThread(new ParameterizedThreadStart((obj) => { userSession.TelegramClient.AuthorizeUser(telegramUser); }));
        if (startThreadLE.Type == TypeMessage.NoneNetworkAuthorize && telegramUser.Id != null)
        {
            ContextDatabase.TelegramUsers.Remove(telegramUser);
            await ContextDatabase.SaveChangesAsync();
        }
        return startThreadLE;
    }

    public async Task<LogginHelper> AuthorizationAttributes(UserManager userSession, LogginHelper attributeLH)
    {
        if (userSession.TelegramClient == null)
            return new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "No telegram client");
        attributeLH = userSession.TelegramClient.TelegramTH.NextRequest(attributeLH);
        if (attributeLH.Type == TypeMessage.NoneNetworkAuthorize)
        {
            TelegramUser? telegramUser = ContextDatabase.TelegramUsers.FirstOrDefault(user => user.IdUser == userSession.IdUser);
            if (telegramUser != null)
            {
                ContextDatabase.TelegramUsers.Remove(telegramUser);
                await ContextDatabase.SaveChangesAsync();
            }
        }
        else if (attributeLH.Type == TypeMessage.GoodNetworkAuthorize)
        {
            if (attributeLH.SomeObject is TelegramUser authorizeTU)
            {
                if (authorizeTU.Id == null)
                    ContextDatabase.TelegramUsers.Add(authorizeTU);
                else
                    ContextDatabase.TelegramUsers.Update(authorizeTU);
                await ContextDatabase.SaveChangesAsync();
            }
            else
                throw new Exception("You have logged in well, but the login has not been saved");
            attributeLH.SomeObject = null;
        }
        return attributeLH;
    }

    public async Task<LogginHelper> Signout(UserManager userSession)
    {
        userSession.TelegramClient?.StopSession();
        var telegramUser = ContextDatabase.TelegramUsers.FirstOrDefault(tu => tu.IdUser == userSession.IdUser);
        if (telegramUser != null)
            ContextDatabase.TelegramUsers.Remove(telegramUser);
        await ContextDatabase.SaveChangesAsync();
        return new(TypeMessage.NoneNetworkAuthorize, DangerLevel.Oke, "Unauthorize");
    }

    public async Task<LogginHelper> GetDialogs(UserManager userSession)
    {
        Func<UserManager, Task<LogginHelper>> getDialogs = async (userSession) =>
        {
            LogginHelper LH = await userSession.TelegramClient!.GetAllDialogs();
            return LH;
        };
        LogginHelper dialogsLoggingHelper = await CheckUserAndExecute(userSession, getDialogs);
        return dialogsLoggingHelper;
    }

    public async Task<LogginHelper> GetMessages(UserManager userSession, long idDialog, int messageOffsetId)
    {
        Func<UserManager, Task<LogginHelper>> getMessages = async (userSession) =>
        {
            LogginHelper LH = await userSession.TelegramClient!.GetHundredMessages(idDialog, messageOffsetId);
            return LH;
        };
        LogginHelper messagesLoggingHelper = await CheckUserAndExecute(userSession, getMessages);
        return messagesLoggingHelper;
    }

    private async Task<LogginHelper> CheckUserAndExecute(UserManager userSession, Func<UserManager, Task<LogginHelper>> execution)
    {
        if (userSession.TelegramClient == null)
            goto AgainAuth;
        LogginHelper dialogsLoggingHelper = await execution(userSession);
        if (dialogsLoggingHelper.Type != TypeMessage.NoneClient)
            return dialogsLoggingHelper;

        AgainAuth:
        LogginHelper authLogging = await TelegramAuthorizeUser(userSession);
        if (authLogging.Type == TypeMessage.GoodNetworkAuthorize)
            return await CheckUserAndExecute(userSession, execution);
        return authLogging;
    }
}
