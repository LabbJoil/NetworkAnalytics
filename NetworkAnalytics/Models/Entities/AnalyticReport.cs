namespace NetworkAnalytics.Models.Entities;

public class AnalyticReport
{
    public Report MainInfo { get; set; } = new();
    public Them AverageThem { get; set; } = new();
    public Tonality AverageTonality { get; set; } = new();
    public PartsSpeech AggregatePartsSpeech { get; set; } = new();
    public List<CommonWord> AggregateCommonWords { get; set; } = [];
}
