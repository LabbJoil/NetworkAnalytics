
using NetworkAnalytics.Models.Entities;
using NetworkAnalytics.Models.SocialNetwork;

namespace NetworkAnalytics.Services.Analytics;

public class DataAnalyzer
{
    private readonly object AnalyticDialogLock = new();
    private readonly Queue<AnalyticMessage> AnalyticDialogQueue = new();
    private int CountAnalizeMessages = 0;
    private int CountAnalizeResults = 0;
    private int ProcessThread = 0;

    public AnalyticReport StartAnalytic(DialogModel dialogModel)
    {
        foreach (MessageModel message in dialogModel.Messages)
        {
            while (ProcessThread >= 5)
            {
                Thread.Sleep(100);
                continue;
            }
            if (message.Text != null)
            {
                new Thread(new ParameterizedThreadStart((obj) => { NextAnalizeMessage(message.Text); })).Start();
                ProcessThread++;
            }
        }
        
        while (dialogModel.Messages.Count != CountAnalizeMessages)
        {
            Thread.Sleep(100);
            continue;
        }
        AnalyticReport anlalyticReport = new()
        {
            MainInfo = new()
            {
                CountMessages = CountAnalizeResults
            }
        };

        Dictionary<string, int> allCommonWords = [];
        while (AnalyticDialogQueue.Count > 0)
        {
            AnalyticMessage report = AnalyticDialogQueue.Dequeue();
            string[] noneMergeProp = ["Id", "IdReport"];
            MergerReports(anlalyticReport.AggregatePartsSpeech, report.PartsSpeechMessage, noneMergeProp, 0);
            MergerReports(anlalyticReport.AverageThem, report.ThemMessage, noneMergeProp, 0f);
            MergerReports(anlalyticReport.AverageTonality, report.TonalityMessage, noneMergeProp, 0f);
            foreach (var entry in report.CommonWordsMessage)
            {
                if (allCommonWords.TryGetValue(entry.Key, out int value))
                    allCommonWords[entry.Key] = value + entry.Value;
                else
                    allCommonWords[entry.Key] = entry.Value;
            }
        }
        var topWords = allCommonWords.OrderByDescending(kv => kv.Value).Take(5);
        foreach (var word in topWords)
            anlalyticReport.AggregateCommonWords.Add(new CommonWord()
            {
                Word = word.Key,
                NumberRepetitions = word.Value,
            });

        if (CountAnalizeResults > 0)
        {
            anlalyticReport.AverageThem.Politics /= CountAnalizeResults;
            anlalyticReport.AverageThem.Art /= CountAnalizeResults;
            anlalyticReport.AverageThem.Sport /= CountAnalizeResults;
            anlalyticReport.AverageThem.Technic /= CountAnalizeResults;
            anlalyticReport.AverageTonality.Positive /= CountAnalizeResults;
            anlalyticReport.AverageTonality.Negative /= CountAnalizeResults;
            anlalyticReport.AverageTonality.Median /= CountAnalizeResults;
        }
        return anlalyticReport;
    }

    private async void NextAnalizeMessage(string message)
    {
        MessageAnalyzer messageAnalyzer = new(message);
        AnalyticMessage? reportMessage = await messageAnalyzer.StartAnalyticMessage();
        lock (AnalyticDialogLock)
        {
            if (reportMessage != null)
            {
                AnalyticDialogQueue.Enqueue(reportMessage);
                CountAnalizeResults++;
            }
            CountAnalizeMessages++;
            ProcessThread--;
        }
    }

    private static void MergerReports<T, TSource>(T firstMergeObject, T secondMergeObject, string[] propNotMerge, TSource defaultValue)
    {
        var allProperties = typeof(T).GetProperties()
            .Where(prop => !propNotMerge.Contains(prop.Name));

        foreach (var sourceProperty in allProperties)
        {
            if (sourceProperty.GetValue(firstMergeObject) is TSource firstObjectPropValue && sourceProperty.GetValue(secondMergeObject) is TSource secondObjectPropValue)
            {
                var newValue = AddValues(firstObjectPropValue, secondObjectPropValue, defaultValue);
                sourceProperty.SetValue(firstMergeObject, newValue);
            }
        }
    }

    private static TSource AddValues<TSource>(TSource value1, TSource value2, TSource defaultValue)
    {
        if (EqualityComparer<TSource>.Default.Equals(defaultValue, default))
        {
            dynamic dynamicValue1 = value1!;
            dynamic dynamicValue2 = value2!;
            return dynamicValue1 + dynamicValue2;
        }
        else
            return value1;
    }
}
