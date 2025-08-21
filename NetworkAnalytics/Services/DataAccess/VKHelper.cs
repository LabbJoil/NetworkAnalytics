
using Microsoft.EntityFrameworkCore;
using NetworkAnalytics.Models.Entities;
using NetworkAnalytics.Models.Enums;
using NetworkAnalytics.Models.SocialNetwork;
using NetworkAnalytics.Services.Clients;
using NetworkAnalytics.Services.Entities;
using NetworkAnalytics.Services.Helper;
using VkNet.Enums.StringEnums;
using WTelegram;

namespace NetworkAnalytics.Services.DataAccess;

public class VKHelper(ContextDB contextDB)
{
    private readonly ContextDB ContextDatabase = contextDB;

    private async Task<LogginHelper> VKAuthorizeUser(UserManager userSession)
    {
        if (userSession.VKClient != null)
        {
            //userSession.VKClient.VKTH.NextRequest(new(TypeMessage.StopThread, DangerLevel.Wanted, ""));
            userSession.VKClient.StopSession();
        }
        VKUser vkUser = await ContextDatabase.VKUsers.FirstOrDefaultAsync(tUser => tUser.IdUser == userSession.IdUser) ??
            new VKUser()
            {
                IdUser = userSession.IdUser
            };
        userSession.VKClient = new VKAPI();
        LogginHelper startThreadLH = userSession.VKClient.VKTH.StartThread(new ParameterizedThreadStart((obj) => { userSession.VKClient.AuthorizeUser(vkUser); }));
        if (startThreadLH.Type == TypeMessage.NoneNetworkAuthorize && vkUser.Id != null)
        {
            ContextDatabase.VKUsers.Remove(vkUser);
            await ContextDatabase.SaveChangesAsync();
        }
        return startThreadLH;
    }

    public async Task<LogginHelper> AuthorizationAttributes(UserManager userSession, LogginHelper logginAttributeEvent)
    {
        if (userSession.VKClient == null)
            return new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "No vk client");
        logginAttributeEvent = userSession.VKClient.VKTH.NextRequest(logginAttributeEvent);
        if (logginAttributeEvent.Type == TypeMessage.NoneNetworkAuthorize)
        {
            VKUser? vkUser = ContextDatabase.VKUsers.FirstOrDefault(user => user.IdUser == userSession.IdUser);
            if (vkUser != null)
            {
                ContextDatabase.VKUsers.Remove(vkUser);
                await ContextDatabase.SaveChangesAsync();
            }
        }
        else if (logginAttributeEvent.Type == TypeMessage.GoodNetworkAuthorize)
        {
            if (logginAttributeEvent.SomeObject is VKUser authorizeTU)
            {
                if (authorizeTU.Id == null)
                    ContextDatabase.VKUsers.Add(authorizeTU);
                else
                    ContextDatabase.VKUsers.Update(authorizeTU);
                await ContextDatabase.SaveChangesAsync();
            }
            else
                throw new Exception("You have logged in well, but the login has not been saved");
            logginAttributeEvent.SomeObject = null;
        }
        return logginAttributeEvent;
    }

    public async Task<LogginHelper> Signout(UserManager userSession)
    {
        userSession.VKClient?.StopSession();
        var vkUser = ContextDatabase.VKUsers.FirstOrDefault(vku => vku.IdUser == userSession.IdUser);
        if (vkUser != null)
            ContextDatabase.VKUsers.Remove(vkUser);
        await ContextDatabase.SaveChangesAsync();
        return new(TypeMessage.NoneNetworkAuthorize, DangerLevel.Oke, "Unauthorize");
    }

    public async Task<LogginHelper> GetDialogs(UserManager userSession)
    {
        Func<UserManager, LogginHelper> getDialogs = (userSession) =>
        {
            var dialogsLH = userSession.VKClient!.VKTH.StartThread(new ParameterizedThreadStart(async (obj) => { await userSession.VKClient.GetAllDialogs(); }));
            return dialogsLH;
        };
        LogginHelper dialogsLoggingHelper = await CheckUserAndExecute(userSession, getDialogs);

        return dialogsLoggingHelper;
    }

    public async Task<LogginHelper> GetMessages(UserManager userSession, long idDialog, int messageOffsetId)
    {
        Func<UserManager, LogginHelper> getMessages = (userSession) =>
        {
            var messagesLH = userSession.VKClient!.VKTH.StartThread(new ParameterizedThreadStart(async (obj) => { await userSession.VKClient!.GetHundredMessages(idDialog, messageOffsetId); }));
            return messagesLH;
        };
        LogginHelper messagesLH = await CheckUserAndExecute(userSession, getMessages);
        return messagesLH;
    }

    private async Task<LogginHelper> CheckUserAndExecute(UserManager userSession, Func<UserManager, LogginHelper> execution)
    {
        if (userSession.VKClient == null)
            goto AgainAuth;
        LogginHelper dialogsLH = execution(userSession);
        if (dialogsLH.Type != TypeMessage.NoneClient)
            return dialogsLH;

        AgainAuth:
        LogginHelper authLH = await VKAuthorizeUser(userSession);
        if (authLH.Type == TypeMessage.GoodNetworkAuthorize)
            return await CheckUserAndExecute(userSession, execution);
        return authLH;
    }
}
