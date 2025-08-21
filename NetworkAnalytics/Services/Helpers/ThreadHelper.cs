
using NetworkAnalytics.Models.Enums;

namespace NetworkAnalytics.Services.Helper;

public class ThreadHelper
{
    private Thread? NetworkThread;
    private LogginHelper? LogginThread;
    private AutoResetEvent RequestEvent { get; set; } = new AutoResetEvent(false);
    private AutoResetEvent ResponseEvent { get; set; } = new AutoResetEvent(false);

    public LogginHelper StartThread(ParameterizedThreadStart parameterizedThreadStart)
    {
        LogginThread = new();
        NetworkThread = new (parameterizedThreadStart);
        NetworkThread.Start();
        if (!RequestEvent.WaitOne(TimeSpan.FromSeconds(180)))
            return new LogginHelper(TypeMessage.Authorize, DangerLevel.Error, "Something went wrong", "Request time is up");
        LogginHelper loggin = LogginThread.Clone();
        CheckTypeLogginHelper(LogginThread.Type);
        return loggin;
    }

    public LogginHelper NextRequest(LogginHelper nextRequestLH)
    {
        if (NetworkThread == null)
            return new LogginHelper(TypeMessage.Problem, DangerLevel.Error, "The stream has ceased to exist");
        LogginThread = nextRequestLH;
        ResponseEvent.Set();
        RequestEvent.Reset();
        if (!RequestEvent.WaitOne(TimeSpan.FromSeconds(180)))
            return new LogginHelper(TypeMessage.NoneAuthorize, DangerLevel.Error, "Something went wrong", "Request time is up");
        nextRequestLH = LogginThread.Clone();
        CheckTypeLogginHelper(LogginThread.Type);
        return nextRequestLH;
    }

    public LogginHelper NextResponse(LogginHelper nextResponseLH)
    {
        if (NetworkThread == null)
            return new LogginHelper(TypeMessage.Problem, DangerLevel.Error, "The stream has ceased to exist");
        LogginThread = nextResponseLH;
        RequestEvent.Set();
        ResponseEvent.Reset();
        if (!ResponseEvent.WaitOne(TimeSpan.FromSeconds(180)))
            return new LogginHelper(TypeMessage.Problem, DangerLevel.Error, "Request time is up");
        return LogginThread?.Clone() ?? new();
    }

    public void StopThread()
    {
        NetworkThread?.Join();
        NetworkThread = null;
        LogginThread = null;
        RequestEvent = new AutoResetEvent(false);
        ResponseEvent = new AutoResetEvent(false);
    }

    private void CheckTypeLogginHelper(TypeMessage nextLHType)
    {
        switch(nextLHType)
        {
            case TypeMessage.NoneNetworkAuthorize:
            case TypeMessage.GoodNetworkAuthorize:
            case TypeMessage.NoneClient:
            case TypeMessage.Problem:
            case TypeMessage.Ordinary:
                ResponseEvent.Set();
                StopThread();
                break;
        }
    }
}
