
using NADesktop.Models.Responses;
using NADesktop.Models.SocialNetwork;
using NADesktop.Services.Helper;
using System.Text.Json;

namespace NADesktop.Services.Server;

internal class TelegramHttp : MainHttp
{
    public async Task<ResponseModel> AdditionalAttributes(LoggingHelper addAttrLogging)
    {
        string uri = $"https://{IP}:{Port}/telegramAPI/addattr";
        string jsonRequestBody = JsonSerializer.Serialize(addAttrLogging);
        ResponseModel responseModel = await PostRequest(uri, jsonRequestBody);
        return responseModel;
    }

    public async Task<(LoggingHelper, ProfileModel?)> GetAllDialogs()
    {
        string uri = $"https://{IP}:{Port}/telegramAPI/allchats";
        ResponseModel responseModel = await GetRequest(uri);
        ProfileModel? profileModel = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<ProfileModel>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, profileModel);
    }

    public async Task<(LoggingHelper, DialogModel?)> GetHundredMessages(long idDialog, long lastMessageId)
    {
        string uri = $"https://{IP}:{Port}/telegramAPI/hundredmessages/{idDialog}/{lastMessageId}";
        ResponseModel responseModel = await GetRequest(uri);
        DialogModel? dialogModel = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<DialogModel>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, dialogModel);
    }

    public async Task<LoggingHelper> SignOut()
    {
        string uri = $"https://{IP}:{Port}/telegramAPI/signout";
        ResponseModel responseModel = await DeleteRequest(uri);
        return responseModel.LoggingAnswer;
    }
}
