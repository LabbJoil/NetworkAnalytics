
using NADesktop.Services.Helper;
using System.Net;
using System.Text.Json.Serialization;

namespace NADesktop.Models.Responses;

public class ResponseModel
{
    [JsonPropertyName("requestObject")]
    public string? RequestObject { get; set; }
    [JsonPropertyName("loggingAnswer")]
    public LoggingHelper LoggingAnswer { get; set; } = new();
    [JsonPropertyName("token")]
    public string? Token { get; set; }
    public HttpStatusCode? StatusCode { get; set; }

    public ResponseModel() { }
    public ResponseModel(ResponseModel responseModel)
    {
        LoggingAnswer = responseModel.LoggingAnswer;
        Token = responseModel.Token;
        StatusCode = responseModel.StatusCode;
    }
}
