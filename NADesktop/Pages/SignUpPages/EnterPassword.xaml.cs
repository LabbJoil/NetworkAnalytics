
using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Services.Server;
using NADesktop.Services.Helpers;
using NADesktop.Services.Helper;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace NADesktop.Pages.AuthorizePages;

public partial class EnterPassword : Page
{
    private readonly SocialNetworkEnum SocialNetwork;
    public EnterPassword(SocialNetworkEnum socialNetwork)
    {
        InitializeComponent();
        SocialNetwork = socialNetwork;
        WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
    }

    private async void EnterPassword_Click(object sender, RoutedEventArgs e)
    {
        string password = PasswordBPC.OriginalTextProperty;
        ResponseModel addAttrRM;
        WindowHelper.StartLoadingGIF(true);

        if (SocialNetwork == SocialNetworkEnum.Telegram)
            addAttrRM = await MainHttp.TelegramRequest.AdditionalAttributes(new(TypeMessage.Password, DangerLevel.Oke, password));
        else
            addAttrRM = await MainHttp.VKRequest.AdditionalAttributes(new(TypeMessage.Password, DangerLevel.Oke, password));
        WindowHelper.StartLoadingGIF(false);

        if (addAttrRM.LoggingAnswer.Type == TypeMessage.NeedPassword)
        {
            PasswordBPC.ResertPassword();
            MessageBox.Show("Введите пароль повторно");
        }
        else
            WindowHelper.CheckStatusCode(addAttrRM.LoggingAnswer, SocialNetwork);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        WindowHelper.RunToBackPage();
    }
}
