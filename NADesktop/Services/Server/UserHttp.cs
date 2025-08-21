
using NADesktop.Models.Enums;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Net;
using NADesktop.Models.Responses;
using NADesktop.Models.SocialNetwork;
using NADesktop.Models.Domain;
using NADesktop.Services.Helper;
using System.Collections.Generic;

namespace NADesktop.Services.Server;

internal class UserHttp : MainHttp
{
    public async Task<(LoggingHelper, User?)> GetUserInfo()
    {
        string uri = $"https://{IP}:{Port}/user/userinfo";
        ResponseModel responseModel = await GetRequest(uri);
        if (responseModel.LoggingAnswer.Type == TypeMessage.Problem)
            responseModel.LoggingAnswer.Type = TypeMessage.NoneAuthorize;
        User? user = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<User>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, user);
    }


    public async Task<(LoggingHelper, User?)> LogInUser(string login, string password)
    {
        string uri = $"https://{IP}:{Port}/user/login/{login}/{password}";
        ResponseModel responseModel = await GetRequest(uri);
        User? user = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<User>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, user);
    }


    public async Task<(LoggingHelper, User?)> SignUpUser(User user)
    {
        string uri = $"https://{IP}:{Port}/user/signup";
        string jsonRequestBody = JsonSerializer.Serialize(user);
        ResponseModel responseModel = await PostRequest(uri, jsonRequestBody);
        User? addUser = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<User>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, addUser);
    }

    public async Task<(LoggingHelper, User?)> UpdateUserInfo(User updateUser)
    {
        string uri = $"https://{IP}:{Port}/user/updateUser";
        string jsonRequestBody = JsonSerializer.Serialize(updateUser);
        ResponseModel responseModel = await PutRequest(uri, jsonRequestBody);
        User? newUser = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<User>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, newUser);
    }

    public async Task<LoggingHelper> UpdatePassword(UpdatePassword updatePassword)
    {
        string uri = $"https://{IP}:{Port}/user/updatePassword";
        string jsonRequestBody = JsonSerializer.Serialize(updatePassword);
        ResponseModel responseModel = await PutRequest(uri, jsonRequestBody);
        return responseModel.LoggingAnswer;
    }

    public async Task<LoggingHelper> DeleteUser()
    {
        string uri = $"https://{IP}:{Port}/user/deleteUser";
        ResponseModel responseModel = await DeleteRequest(uri);
        return responseModel.LoggingAnswer;
    }

    public async Task<LoggingHelper> SendDataAnalytic(DialogModel dialogModel)
    {
        string uri = $"https://{IP}:{Port}/user/doAnalytics";
        string jsonRequestBody = JsonSerializer.Serialize(dialogModel);
        ResponseModel responseModel = await PostRequest(uri, jsonRequestBody);
        return responseModel.LoggingAnswer;
    }

    public async Task<(LoggingHelper, List<Report>?)> GetReports()
    {
        string uri = $"https://{IP}:{Port}/user/reports";
        ResponseModel responseModel = await GetRequest(uri);
        List <Report>? reports = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<List<Report>>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, reports);
    }

    public async Task<(LoggingHelper, AnalyticReport?)> GetReportById(int reportId)
    {
        string uri = $"https://{IP}:{Port}/user/reportId/{reportId}";
        ResponseModel responseModel = await GetRequest(uri);
        AnalyticReport? reports = !string.IsNullOrEmpty(responseModel.RequestObject) ? JsonSerializer.Deserialize<AnalyticReport?>(responseModel.RequestObject) : null;
        return (responseModel.LoggingAnswer, reports);
    }

    public async Task<LoggingHelper> DeleteReports(int idReport)
    {
        string uri = $"https://{IP}:{Port}/user/deleteReport/{idReport}";
        ResponseModel responseModel = await DeleteRequest(uri);
        return responseModel.LoggingAnswer;
    }
}
