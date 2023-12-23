using NADesktop.Models.Domain;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System.IO;

namespace NADesktop.Services.DocumentGenerators;

internal class DocxGenerator(string pathDocument)
{
    private readonly XWPFDocument Document = new();
    private readonly string PathDocument = pathDocument + ".docx";

    public void CreateWordDocument(string[] chartsName, Report report, List<CommonWord>? commonWords)
    {
        DefaultParams();
        CreateFirstPage(report, chartsName[0]);
        CreateSecondPage(chartsName[1], chartsName[2]);
        CreateThirdPage(commonWords);
        using FileStream sw = File.Create(PathDocument);
        Document.Write(sw);
    }

    private void CreateFirstPage(Report report, string themChart)
    {
        string login = (report?.Login?.Length > 20 ? $"{report.Login[^20]}..." : report?.Login) ?? "None";
        string dialog = (report?.Dialog?.Length > 20 ? $"{report.Dialog[^20]}..." : report?.Dialog) ?? "None";
        AddTextBlock("Аналитика с помощью сервиса", 16, 2700, 1000, true);
        AddTextBlock("Network Analysis", 16, 3600, 0, true);
        AddTextBlock($"Социальная сеть для анализа: {report?.SocialNetwork ?? "None"} \t\t\t\t\t Логин: {login}", 12, 20, 1000);
        AddTextBlock($"Выбранный чат для анализа: {dialog}", 12, 20, 0);
        AddTextBlock($"Кол-во выбранных сообщений: {report?.CountMessages} \t\t\t\t\t Дата: {report?.CreateDate}", 12, 20, 0);

        AddTextBlock("Диаграмма вероятности тематической принадлежности", 14, 1500, 2000, true);
        AddChartPNG(themChart, 1400, 700);
    }

    private void CreateSecondPage(string tonalityChart, string PartsSpeech)
    {
        AddTextBlock("График тональной вероятности", 14, 3300, 2300, true);
        AddChartPNG(tonalityChart, 1400, 700);
        AddTextBlock("Диаграмма частей речи по количеству", 14, 3000, 2000, true);
        AddChartPNG(PartsSpeech, 1400, 700);
    }

    private void CreateThirdPage(List<CommonWord>? commonWords)
    {
        AddTextBlock("Топ-5 используемых слов", 16, 3600, 0, true);
        int numRows = 5;
        int numCols = 2;

        XWPFTable table = Document.CreateTable(numRows, numCols);

        for (int row = 0; row < commonWords?.Count; row++)
        {
            XWPFTableCell cell = table.GetRow(row).GetCell(0);
            cell.SetText(commonWords?[row].Word ?? "");
            cell = table.GetRow(row).GetCell(1);
            cell.SetText(commonWords?[row].NumberRepetitions.ToString() ?? "");
        }

        // Добавляем созданную таблицу в параграф
        //paragraph.(table);
    }

    private void AddChartPNG(string nameChart, int xPosition, int yPosition)
    {
        XWPFParagraph paragraph = Document.CreateParagraph();
        paragraph.IndentationLeft = xPosition;
        paragraph.SpacingBefore = yPosition;

        XWPFRun runPNG = paragraph.CreateRun();
        using FileStream img = new($"{nameChart}.png", FileMode.Open, FileAccess.Read);
        runPNG.AddPicture(img, (int)NPOI.XWPF.UserModel.PictureType.PNG, nameChart, 450 * 10857, 252 * 12857);
    }

    private void AddTextBlock(string text, float fontSize, int xPosition, int yPosition, bool isBold = false)
    {
        XWPFParagraph paragraph = Document.CreateParagraph();
        paragraph.IndentationLeft = xPosition;
        paragraph.SpacingBefore = yPosition;
        paragraph.SpacingAfter = 0;


        XWPFRun run = paragraph.CreateRun();
        run.SetText(text);
        run.FontSize = fontSize;
        run.FontFamily = "Tahoma";
        run.IsBold = isBold;
    }

    private void DefaultParams()
    {
        CT_SectPr sectPr = Document.Document.body.sectPr ?? Document.Document.body.AddNewSectPr();
        CT_PageMar pageMar = sectPr.pgMar ?? sectPr.AddPageMar();
        pageMar.left = 500;
        pageMar.top = 500;
        pageMar.right = 500;
        pageMar.bottom = 500;
    }
}
