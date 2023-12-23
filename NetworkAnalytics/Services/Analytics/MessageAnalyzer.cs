using Catalyst;
using Catalyst.Models;
using NetworkAnalytics.Services.Analytics.ML;
using Mosaik.Core;
using NetworkAnalytics.Models.ML;
using NetworkAnalytics.Models.Entities;

namespace NetworkAnalytics.Services.Analytics;

public class MessageAnalyzer(string textMessage)
{
    private static readonly object AnalyticMessageLock = new();
    private readonly string AnalyzeText = textMessage;
    private readonly AnalyticMessage ReportMessage = new();

    static MessageAnalyzer()
    {
        English.Register();
        Russian.Register();
    }

    public async Task<AnalyticMessage?> StartAnalyticMessage()
    {
        Language detectedLanguage = await NLPAnalytic();
        if (detectedLanguage == Language.Unknown)
            return null;
        MLAnalytic(detectedLanguage);
        return ReportMessage;
    }

    private void MLAnalytic(Language language)
    {
        ModelInput modelInput = new() { Col1 = AnalyzeText };
        ModelOutput modelOutputThem;
        ModelOutput modelOutputTonality;

        lock(AnalyticMessageLock)
        {
            if (language == Language.Russian)
            {
                modelOutputThem = ThemML_Rus.Predict(modelInput);
                modelOutputTonality = TonalityML_Rus.Predict(modelInput);
            }
            else if (language == Language.English)
            {
                modelOutputThem = ThemML_Eng.Predict(modelInput);
                modelOutputTonality = TonalityML_Eng.Predict(modelInput);
            }
            else return;
        }
        if (modelOutputThem.Score.Length == 4)
            ReportMessage.ThemMessage = new()
            {
                Technic = modelOutputThem.Score[0],
                Politics = modelOutputThem.Score[1],
                Art = modelOutputThem.Score[2],
                Sport = modelOutputThem.Score[3]
            };
        if (modelOutputTonality.Score.Length == 3)
            ReportMessage.TonalityMessage = new()
            {
                Positive = modelOutputTonality.Score[1],
                Median = modelOutputTonality.Score[0],
                Negative = modelOutputTonality.Score[2]
            };
    }

    private async Task<Language> NLPAnalytic()
    {
        string text = AnalyzeText;
        LanguageDetector languageDetector = await LanguageDetector.FromStoreAsync(Language.Any, Mosaik.Core.Version.Latest, "");

        ReportMessage.PartsSpeechMessage = new();
        ReportMessage.CommonWordsMessage = [];

        Storage.Current = new DiskStorage("catalyst-models");
        var doc = new Document(text);
        for (int attemptIdentifyLanguage = 0; attemptIdentifyLanguage < 5; attemptIdentifyLanguage++)
        {
            languageDetector.Detect(doc);
            if (doc.Language == Language.Russian || doc.Language == Language.English)
                break;
            else if(attemptIdentifyLanguage == 4)
                return Language.Unknown;
        }
        
        
        var nlp = Pipeline.For(doc.Language);
        nlp.ProcessSingle(doc);

        foreach (var tokensList in doc.TokensData)
        {
            foreach (var token in tokensList)
            {
                if (token.Tag != PartOfSpeech.PUNCT)
                {
                    string word = doc.Value.Substring(token.Bounds[0], token.Bounds[1] - token.Bounds[0] + 1);
                    int countWords = 1;
                    if (ReportMessage.CommonWordsMessage.TryGetValue(word, out int value))
                        countWords += value;
                    ReportMessage.CommonWordsMessage[word] = countWords;
                }
                
                switch (token.Tag)
                {
                    case PartOfSpeech.NOUN:
                    case PartOfSpeech.PROPN:
                        ReportMessage.PartsSpeechMessage.NOUN++;
                        break;
                    case PartOfSpeech.DET:
                        ReportMessage.PartsSpeechMessage.DET++;
                        break;
                    case PartOfSpeech.ADJ:
                        ReportMessage.PartsSpeechMessage.ADJ++;
                        break;
                    case PartOfSpeech.ADP:
                        ReportMessage.PartsSpeechMessage.ADP++;
                        break;
                    case PartOfSpeech.ADV:
                        ReportMessage.PartsSpeechMessage.ADV++;
                        break;
                    case PartOfSpeech.VERB:
                    case PartOfSpeech.AUX:
                        ReportMessage.PartsSpeechMessage.VERB++;
                        break;
                    case PartOfSpeech.CCONJ:
                    case PartOfSpeech.PART:
                    case PartOfSpeech.SCONJ:
                        ReportMessage.PartsSpeechMessage.PART++;
                        break;
                    case PartOfSpeech.NUM:
                        ReportMessage.PartsSpeechMessage.NUM++;
                        break;
                    case PartOfSpeech.PRON:
                        ReportMessage.PartsSpeechMessage.PRON++;
                        break;
                    case PartOfSpeech.SYM:
                        ReportMessage.PartsSpeechMessage.SYM++;
                        break;
                    case PartOfSpeech.X:
                        ReportMessage.PartsSpeechMessage.X++;
                        break;
                    case PartOfSpeech.INTJ:
                        ReportMessage.PartsSpeechMessage.INTJ++;
                        break;
                }
            }
        }
        return doc.Language;
    }
}
