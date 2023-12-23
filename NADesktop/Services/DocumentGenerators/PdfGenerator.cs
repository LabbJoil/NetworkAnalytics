
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using NADesktop.Models.Domain;

namespace NADesktop.Services.DocumentGenerators;

class PdfGenerator
{
    private readonly PdfDocument PDFMainPoint;
    private readonly Document RootDocument;

    public PdfGenerator(string pathDocument)
    {
        PdfWriter writer = new(pathDocument + ".pdf", new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
        PDFMainPoint = new(writer);
        RootDocument = new Document(PDFMainPoint, PageSize.A4);
    }

    public void CreatePDF(string[] chartsName, Report mainInfo, List<CommonWord>? commonWords)
    {
        CreateFirstPage(mainInfo, chartsName[0]);
        CreateSecondPage(chartsName[1], chartsName[2]);
        CreateThirdPage(commonWords);

        PDFMainPoint.Close();
    }

    private void CreateFirstPage(Report mainInfo, string themChart)
    {
        var page = PDFMainPoint.AddNewPage(PageSize.A4);
        string login = (mainInfo?.Login?.Length > 20 ? $"{mainInfo.Login[^20]}..." : mainInfo?.Login) ?? "None";
        string dialog = (mainInfo?.Dialog?.Length > 20 ? $"{mainInfo.Dialog[^20]}..." : mainInfo?.Dialog) ?? "None";

        AddTextBlock("Аналитика с помощью сервиса\nNetwork Analysis", -200, 700, 16, true, TextAlignment.CENTER);
        AddTextBlock($"Социальная сеть для анализа: {mainInfo?.SocialNetwork ?? "None"}", 15, 660, 12, false, TextAlignment.LEFT);
        AddTextBlock($"Логин: {login}", -620, 660, 12, false, TextAlignment.RIGHT);
        AddTextBlock($"Выбранный чат для анализа: {dialog}", 15, 630, 12, false, TextAlignment.LEFT);
        AddTextBlock($"Кол-во выбранных сообщений: {mainInfo?.CountMessages}", 15, 600, 12, false, TextAlignment.LEFT);
        AddTextBlock($"Дата: {mainInfo?.CreateDate}", -620, 600, 12, false, TextAlignment.RIGHT);
        AddTextBlock("Диаграмма вероятности тематической принадлежности", -200, 450, 14, true, TextAlignment.CENTER);
        AddChartPNG(themChart, 100, 100);
    }

    private void CreateSecondPage(string tonalityChart, string PartsSpeech)
    {
        RootDocument.Add(new AreaBreak());
        AddTextBlock("График тональной вероятности", -200, 800, 14, true, TextAlignment.CENTER);
        AddChartPNG(tonalityChart, 100, 450);
        AddTextBlock("Диаграмма частей речи по количеству", -200, 400, 14, true, TextAlignment.CENTER);
        AddChartPNG(PartsSpeech, 20, 50);
    }

    private void CreateThirdPage(List<CommonWord>? commonWords)
    {
        RootDocument.Add(new AreaBreak());
        AddTextBlock("Топ-5 используемых слов", -200, 700, 14, true, TextAlignment.CENTER);

        Table table = new(2, false);
        table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
        table.SetVerticalAlignment(VerticalAlignment.MIDDLE);

        for (int row = 0; row < commonWords?.Count; row++)
        {
            Cell cellLeft = new Cell().Add(new Paragraph(commonWords[row].Word ?? ""));
            table.AddCell(cellLeft);
            Cell cellRight = new Cell().Add(new Paragraph(commonWords[row].NumberRepetitions.ToString() ?? ""));
            table.AddCell(cellRight);
        }

        table.SetFixedPosition(200, 550, 200);
        RootDocument.Add(table);
    }

    private void AddTextBlock(string text, float xPosition, float yPosition, float fontSize, bool isBold = false, TextAlignment textAlignment = TextAlignment.JUSTIFIED)
    {
        PdfFont font = PdfFontFactory.CreateFont("Tahoma.ttf", "CP1251");
        Paragraph paragraph = new Paragraph(text)
            .SetFont(font)
            .SetFontSize(fontSize).SetKeepTogether(true);
        if (isBold)
            paragraph.SetBold();
        paragraph.SetFixedPosition(xPosition, yPosition, 1000);
        paragraph.SetTextAlignment(textAlignment);
        RootDocument.Add(paragraph);
    }

    private void AddChartPNG(string nameChart, int x, int y)
    {
        PdfCanvas canvasImage = new(PDFMainPoint.GetLastPage());
        string tempImagePath = $"{nameChart}.png";
        var image = ImageDataFactory.Create(tempImagePath);
        canvasImage.AddImageAt(image, x, y, false);
    }

}
