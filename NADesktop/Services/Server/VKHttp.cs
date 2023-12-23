
using NADesktop.Models.Domain;
using NADesktop.Models.Responses;
using NADesktop.Models.SocialNetwork;
using NADesktop.Services.Helper;
using System.Text.Json;

namespace NADesktop.Services.Server;

internal class VKHttp : MainHttp
{
    public async Task<ResponseModel> AdditionalAttributes(LoggingHelper addAttrLogging)
    {
        string uri = $"https://{IP}:{Port}/vkAPI/addattr";
        string jsonRequestBody = JsonSerializer.Serialize(addAttrLogging);
        ResponseModel responseModel = await PostRequest(uri, jsonRequestBody);
        return responseModel;
    }

    public async Task<(LoggingHelper, ProfileModel?)> GetAllDialogs()
    {
        string uri = $"https://{IP}:{Port}/vkAPI/allchats";
        ResponseModel responseModel = await GetRequest(uri);
        ProfileModel? profileModel = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<ProfileModel>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, profileModel);
    }

    public async Task<(LoggingHelper, DialogModel?)> GetHundredMessages(long idDialog, long lastMessageId)
    {
        string uri = $"https://{IP}:{Port}/vkAPI/hundredmessages/{idDialog}/{lastMessageId}";
        ResponseModel responseModel = await GetRequest(uri);
        DialogModel? dialogModel = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<DialogModel>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, dialogModel);
    }

    public async Task<LoggingHelper> SignOut()
    {
        string uri = $"https://{IP}:{Port}/vkAPI/signout";
        ResponseModel responseModel = await DeleteRequest(uri);
        return responseModel.LoggingAnswer;
    }
}
