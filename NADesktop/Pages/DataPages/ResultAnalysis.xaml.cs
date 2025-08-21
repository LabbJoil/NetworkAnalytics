
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using Microsoft.Win32;
using NADesktop.Models.Domain;
using NADesktop.Services.DocumentGenerators;
using NADesktop.Services.Helpers;
using NADesktop.Services.Server;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NADesktop.Pages.Data;

public partial class ResultAnalysis : Page
{
    private readonly int ReportId;
    private AnalyticReport? Report;

    public ResultAnalysis(int reportId)
    {
        ReportId = reportId;
        InitializeComponent();
    }

    private async void ResultAnalysis_Loaded(object sender, RoutedEventArgs e)
    {
        WindowHelper.StartLoadingGIF(true);
        var analyticReport = await MainHttp.UserRequest.GetReportById(ReportId);
        WindowHelper.StartLoadingGIF(false);
        if (!WindowHelper.CheckStatusCode(analyticReport.Item1) || analyticReport.Item2 == null)
            return;
        Report = analyticReport.Item2;
        AddParams();
    }

    private void CreateFile_Click(object sender, RoutedEventArgs e)
    {
        string directory = DirectoryBTC.BannerTextBox.Text;
        string fileName = FileNameBTC.BannerTextBox.Text;
        string filePath = $"{directory}\\{fileName}";
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            MessageBox.Show("Такой директории не существует");
            return;
        }
        if (string.IsNullOrEmpty(fileName))
        {
            MessageBox.Show("Введите название файла");
            return;
        }
        try
        {
            var chartsName = new string[]
            {
                CreatePNGChart(ThemChart, 90, 220, 10, 0),
                CreatePNGChart(TonalityChart, 90, 600, 10, 0),
                CreatePNGChart(PartsSpeechChart, 0, 980, 70, 0),
            };

            if ((bool)CheckTB.IsChecked!)
            {
                DocxGenerator docs = new(filePath);
                docs.CreateWordDocument(chartsName, Report!.MainInfo , Report!.AggregateCommonWords);
            }
            else
            {
                PdfGenerator pdf = new(filePath);
                pdf.CreatePDF(chartsName, Report!.MainInfo, Report!.AggregateCommonWords);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
     }

    public void AddParams()
    {
        ThemChart.Series =
        [
            new PieSeries
            {
                Title = "Politics",
                Values = new ChartValues<float> { Report!.AverageThem?.Politics ?? 0f },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "Art",
                Values = new ChartValues<float> { Report!.AverageThem?.Art ?? 0f },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "Sport",
                Values = new ChartValues<float> { Report!.AverageThem?.Sport ?? 0f },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "Technic",
                Values = new ChartValues<float> { Report!.AverageThem?.Technic ?? 0f },
                DataLabels = true
            }
        ];

        TonalityChart.Series =
        [
            new PieSeries
            {
                Title = "Positive",
                Values = new ChartValues<float> { Report!.AverageTonality?.Positive ?? 0f },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "Negative",
                Values = new ChartValues<float> { Report!.AverageTonality?.Negative ?? 0f },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "Median",
                Values = new ChartValues<float> { Report!.AverageTonality?.Median ?? 0f },
                DataLabels = true
            }
        ];

        PartsSpeechChart.Series =
        [
            new ColumnSeries
            {
                Values = new ChartValues<int>
                {
                    Report!.AggregatePartsSpeech?.NOUN ?? 0,
                    Report!.AggregatePartsSpeech?.DET ?? 0,
                    Report!.AggregatePartsSpeech?.ADJ ?? 0,
                    Report!.AggregatePartsSpeech?.PART ?? 0,
                    Report!.AggregatePartsSpeech?.PRON ?? 0,
                    Report!.AggregatePartsSpeech?.ADP ?? 0,
                    Report!.AggregatePartsSpeech?.VERB ?? 0,
                    Report!.AggregatePartsSpeech?.NUM ?? 0,
                    Report!.AggregatePartsSpeech?.ADV ?? 0,
                    Report!.AggregatePartsSpeech?.INTJ ?? 0,
                    Report!.AggregatePartsSpeech?.SYM ?? 0,
                    Report!.AggregatePartsSpeech?.X ?? 0,
                }
            }
        ];

        SocialNetworkTB.Text = Report.MainInfo.SocialNetwork;
        DialogTB.Text = Report.MainInfo.Dialog;
        CountMessagesTB.Text = Report.MainInfo.CountMessages.ToString();
        LoginTB.Text = Report.MainInfo.Login;
        DateTB.Text = Report.MainInfo.CreateDate.ToString();

        List <(TextBlock, TextBlock)> tableWord = [
            ( FirstWordTB, FirstValueTB ),
            ( SecondWordTB, SecondValueTB ),
            ( ThirdWordTB, ThirdValueTB ),
            ( FourthWordTB, FourthValueTB ),
            ( FifthWordTB, FifthValueTB ),
        ];
        for (int indexWord = 0; indexWord < Report?.AggregateCommonWords.Count; indexWord++)
        {
            tableWord[indexWord].Item1.Text = Report?.AggregateCommonWords[indexWord].Word;
            tableWord[indexWord].Item2.Text = Report?.AggregateCommonWords[indexWord].NumberRepetitions.ToString();
        }
    }

    private static string CreatePNGChart(Chart chart, int x, int y, int addWidth, int addHeight)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        RenderTargetBitmap renderTarget = new(
          (int)chart.ActualWidth + addWidth + x,
          (int)chart.ActualHeight + addHeight + y,
          96, 96, PixelFormats.Pbgra32);

        renderTarget.Render(chart);

        int width = (int)chart.RenderSize.Width + addWidth;
        int height = (int)chart.RenderSize.Height + addHeight;

        CroppedBitmap croppedBitmap = new(
            renderTarget,
            new Int32Rect(x, y, width, height)
        );

        PngBitmapEncoder pngEncoder = new();
        pngEncoder.Frames.Add(BitmapFrame.Create(croppedBitmap));

        using Stream stream = File.Create($"{chart.Name}.png");
        pngEncoder.Save(stream);
        stream.Close();
        return $"{currentDirectory}\\{chart.Name}";
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        WindowHelper.RunToBackPage();
    }

    private void OpenDirectory_Click(object sender, RoutedEventArgs e)
    {
        OpenFolderDialog saveFileDialog = new ();
        if (saveFileDialog.ShowDialog() == true)
            DirectoryBTC.BannerTextBox.Text = saveFileDialog.FolderName;
    }
}
