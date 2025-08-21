
using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Services.Helpers;
using NADesktop.Services.Server;
using System.Windows;
using System.Windows.Controls;

namespace NADesktop.Pages.AuthorizePages;

public partial class EnterCode : Page
{
    private readonly SocialNetworkEnum SocialNetwork;
    public EnterCode(SocialNetworkEnum socialNetwork)
    {
        InitializeComponent();
        SocialNetwork = socialNetwork;
        WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
    }

    private async void EnterCode_Click(object sender, RoutedEventArgs e)
    {
        string code = CodeBTC.BannerTextBox.Text;
        WindowHelper.StartLoadingGIF(true);
        ResponseModel addAttrRM = SocialNetwork switch
        {
            SocialNetworkEnum.Telegram => await MainHttp.TelegramRequest.AdditionalAttributes(new(TypeMessage.Code, DangerLevel.Oke, code)),
            SocialNetworkEnum.VK => await MainHttp.VKRequest.AdditionalAttributes(new(TypeMessage.Code, DangerLevel.Oke, code)),
            _ => new() 
            {
                LoggingAnswer = new(TypeMessage.Problem, DangerLevel.Error, "Unclear type of social network")
            }
        };
        WindowHelper.StartLoadingGIF(false);

        if (addAttrRM.LoggingAnswer.Type == TypeMessage.NeedCode)
        {
            CodeBTC.BannerTextBox.Text = "";
            MessageBox.Show("Введите код повторно");
        }
        else
            WindowHelper.CheckStatusCode(addAttrRM.LoggingAnswer, SocialNetwork);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        WindowHelper.RunToBackPage();
    }
}
