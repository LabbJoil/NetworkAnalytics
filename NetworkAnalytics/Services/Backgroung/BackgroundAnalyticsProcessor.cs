
using NetworkAnalytics.Models.Entities;
using NetworkAnalytics.Models.SocialNetwork;
using NetworkAnalytics.Services.Analytics;
using NetworkAnalytics.Services.Entities;

namespace NetworkAnalytics.Services.Background;

public class BackgroundAnalyticsProcessor(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory ScopeFactory = scopeFactory;
    private static readonly Queue<KeyValuePair<int, DialogModel>> AnaliticDialogs = new();

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = ScopeFactory.CreateScope();
            ContextDB Database = scope.ServiceProvider.GetRequiredService<ContextDB>();

            while (AnaliticDialogs.Count > 0)
            {
                try
                {
                    var nextDialog = AnaliticDialogs.Dequeue();
                    AnalyticReport newAnalitycsReport = new DataAnalyzer().StartAnalytic(nextDialog.Value);
                    newAnalitycsReport.MainInfo.Dialog = nextDialog.Value.Title;
                    newAnalitycsReport.MainInfo.CreateDate = DateTime.UtcNow;
                    newAnalitycsReport.MainInfo.Login = Database.Users.FirstOrDefault(user => user.Id == nextDialog.Key)?.Login ?? "Unknown";
                    newAnalitycsReport.MainInfo.SocialNetwork = nextDialog.Value.SocialNetwork.ToString();
                    newAnalitycsReport.MainInfo.IdUser = nextDialog.Key;

                    var addedReport = Database.Reports.Add(newAnalitycsReport.MainInfo);
                    await Database.SaveChangesAsync(stoppingToken);

                    int reportId = addedReport.Entity.Id;
                    newAnalitycsReport.AverageThem.IdReport = reportId;
                    newAnalitycsReport.AverageTonality.IdReport = reportId;
                    newAnalitycsReport.AggregatePartsSpeech.IdReport = reportId;

                    foreach (var nextWord in newAnalitycsReport.AggregateCommonWords)
                    {
                        nextWord.IdReport = reportId;
                        Database.CommonWords.Add(nextWord);
                    }
                    Database.Thems.Add(newAnalitycsReport.AverageThem);
                    Database.Tonalitys.Add(newAnalitycsReport.AverageTonality);
                    Database.PartsSpeechs.Add(newAnalitycsReport.AggregatePartsSpeech);
                    await Database.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            await Task.Delay(100, stoppingToken);
        }
    }

    public static void AddDialog(int idUser, DialogModel analyticDialog)
        => AnaliticDialogs.Enqueue(new KeyValuePair<int, DialogModel>(idUser, analyticDialog));
}
