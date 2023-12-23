namespace NetworkAnalytics.Models.Entities;

public class AnalyticMessage
{
    public Them ThemMessage { get; set; } = new();
    public Tonality TonalityMessage { get; set; } = new();
    public PartsSpeech PartsSpeechMessage { get; set; } = new();
    public Dictionary<string, int> CommonWordsMessage = [];
}
