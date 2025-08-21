using NetworkAnalytics.Services.Helper;

namespace NetworkAnalytics.Models.Response
{
    public class ResponseModel
    {
        public string? RequestObject { get; set; }
        public LogginHelper? LoggingAnswer { get; set; }
        public string? Token { get; set; }
        public ResponseModel(string? requestObject, LogginHelper loggingAnswer, string? token)
        {
            RequestObject = requestObject;
            LoggingAnswer = loggingAnswer;
            Token = token;
        }
    }
}
