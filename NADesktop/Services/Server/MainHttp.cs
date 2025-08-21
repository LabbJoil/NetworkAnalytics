
using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Services.Helper;
using NADesktop.Services.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NADesktop.Services.Server;

internal class MainHttp
{
    protected readonly static HttpClient HttpClient = 
        new(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = delegate { return true; }
        })
        {
            Timeout = TimeSpan.FromSeconds(200)
        };
    public static UserHttp UserRequest { get; private set; }
    public static TelegramHttp TelegramRequest { get; private set; }
    public static VKHttp VKRequest { get; private set; }

    private HttpResponseMessage? Response;
    public static string? IP { get; private set; }
    public static string? Port { get; private set; }
    private static string? Token { get; set; }

    static MainHttp()
    {
        IP = ConfigurationHelper.ConfigApp.Ip;
        Port = ConfigurationHelper.ConfigApp.Port;
        SetToken(ConfigurationHelper.ConfigApp.Token!);
        UserRequest = new UserHttp();
        TelegramRequest = new TelegramHttp();
        VKRequest = new VKHttp();
    }

    public static async Task<LoggingHelper> ChangeParams(string ip, string port)
    {
        LoggingHelper configLH = await ConfigurationHelper.SaveConfig(ip, port);
        if (configLH.DangerLevel == DangerLevel.Oke)
        {
            IP = ip;
            Port = port;
        }
        return configLH;
    }

    public static async Task<LoggingHelper> SaveParams()
    {
        LoggingHelper configLH = await ConfigurationHelper.SaveConfig(IP, Port, Token);
        return configLH;
    }

    private static void SetToken(string token)
    {
        Token = token;
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static string GetRequestMessage(HttpResponseMessage? response, string? exceptionMessage)
    {
        string message = exceptionMessage ?? "Сообщения нет";
        if (response == null)
            return message;
        else
        {
            byte[] buffer = new byte[2048];
            int bytesRead = response.Content.ReadAsStream().Read(buffer);
            message = bytesRead == 0 ? "Сообщения нет" : Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
        return message;
    }

    protected async Task<ResponseModel> GetRequest(string requestURI)
    {
        Response = null;
        ResponseModel? jsonBody = null;
        try
        {
            Response = await HttpClient.GetAsync(requestURI);
            string responseBody = await Response.Content.ReadAsStringAsync();
            jsonBody = JsonSerializer.Deserialize<ResponseModel>(responseBody)
                ?? throw new Exception("Не удалось преобразовать полученный от сервера ответ");
            SetToken(jsonBody.Token ?? throw new Exception("Не удалось получить токен авторизации"));
            jsonBody.LoggingAnswer.StatusCode = Response.StatusCode;
            return jsonBody;
        }
        catch (Exception ex)
        {
            return new()
            {
                LoggingAnswer = jsonBody?.LoggingAnswer ?? new()
                {
                    Message = GetRequestMessage(Response, ex.Message),
                    Description = ex.ToString()
                },
                StatusCode = Response?.StatusCode,
            };
        }
    }

    protected async Task<ResponseModel> DeleteRequest(string requestURI)
    {
        Response = null;
        ResponseModel? jsonBody = null;
        try
        {
            Response = await HttpClient.DeleteAsync(requestURI);
            string responseBody = await Response.Content.ReadAsStringAsync();
            jsonBody = JsonSerializer.Deserialize<ResponseModel>(responseBody)
                ?? throw new Exception("Не удалось преобразовать полученный от сервера ответ");
            SetToken(jsonBody.Token ?? throw new Exception("Не удалось получить токен авторизации"));
            jsonBody.LoggingAnswer.StatusCode = Response.StatusCode;
            return jsonBody;
           
        }
        catch (Exception ex)
        {
            return new()
            {
                LoggingAnswer = jsonBody?.LoggingAnswer ?? new()
                {
                    Message = GetRequestMessage(Response, ex.Message),
                    Description = ex.ToString()
                },
                StatusCode = Response?.StatusCode,
            };
        }
    }

    protected async Task<ResponseModel> PutRequest(string requestURI, string jsonRequestBody)
    {
        Response = null;
        ResponseModel? jsonBody = null;
        try
        {
            Response = await HttpClient.PutAsync(requestURI, new StringContent(jsonRequestBody, Encoding.UTF8, "application/json"));
            string responseBody = await Response.Content.ReadAsStringAsync();
            jsonBody = JsonSerializer.Deserialize<ResponseModel>(responseBody)
                ?? throw new Exception("Не удалось преобразовать полученный от сервера ответ");
            SetToken(jsonBody.Token ?? throw new Exception("Не удалось получить токен авторизации"));
            jsonBody.LoggingAnswer.StatusCode = Response.StatusCode;
            return jsonBody;
        }
        catch (Exception ex)
        {
            return new()
            {
                LoggingAnswer = jsonBody?.LoggingAnswer ?? new()
                {
                    Message = GetRequestMessage(Response, ex.Message),
                    Description = ex.ToString()
                },
                StatusCode = Response?.StatusCode,
            };
        }
    }

    protected async Task<ResponseModel> PostRequest(string requestURI, string jsonRequestBody)
    {
        Response = null;
        ResponseModel? jsonBody = null;
        try
        {
            Response = await HttpClient.PostAsync(requestURI, new StringContent(jsonRequestBody, Encoding.UTF8, "application/json"));
            string responseBody = await Response.Content.ReadAsStringAsync();
            jsonBody = JsonSerializer.Deserialize<ResponseModel>(responseBody)
                ?? throw new Exception("Не удалось преобразовать полученный от сервера ответ");
            SetToken(jsonBody.Token ?? throw new Exception("Не удалось получить токен авторизации"));
            jsonBody.LoggingAnswer.StatusCode = Response.StatusCode;
            return jsonBody;
        }
        catch (Exception ex)
        {
            return new()
            {
                LoggingAnswer = jsonBody?.LoggingAnswer ?? new()
                {
                    Message = GetRequestMessage(Response, ex.Message),
                    Description = ex.ToString()
                },
                StatusCode = Response?.StatusCode,
            };
        }
    }
}
