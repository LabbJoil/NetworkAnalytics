
using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Services.Helper;
using NADesktop.Services.Server;
using System.Windows;

namespace NADesktop.Windows;

public partial class CaptchaSolverWindow : Window
{
    public ResponseModel ResultRM { get; set; } = new();
    public CaptchaSolverWindow(LoggingHelper CaptchaLH)
    {
        InitializeComponent();
        LoadURI(CaptchaLH.Description);
    }

    private async void EnterCaptchaB_Click(object sender, RoutedEventArgs e)
    {
        string code = CodeCaptchaBTC.BannerTextBox.Text;
        LoadRectangle(true);
        ResultRM = await MainHttp.VKRequest.AdditionalAttributes(new LoggingHelper(TypeMessage.Captcha, DangerLevel.Oke, code));
        LoadRectangle(false);
        if (ResultRM.LoggingAnswer.Type == TypeMessage.Captcha)
        {
            CodeCaptchaBTC.BannerTextBox.Text = "";
            LoadURI(ResultRM.LoggingAnswer.Description);
            MessageBox.Show("Введите ещё раз каптчу");
            return;
        }
        DialogResult = true;
        Close();
    }

    private void LoadRectangle(bool isTurn)
    {
        if (isTurn)
            OverlayRectangle.Visibility = Visibility.Visible;
        else
            OverlayRectangle.Visibility = Visibility.Hidden;
        OverlayRectangle.IsEnabled = isTurn;
    }

    private void LoadURI(string? uri)
    {
        try
        {
            Web.Navigate(uri);
        }
        catch
        {
            MessageBox.Show("Не удалось открыть каптчу");
        }
    }
}
