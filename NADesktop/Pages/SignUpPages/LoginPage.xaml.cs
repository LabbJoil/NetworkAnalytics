using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Services.Server;
using NADesktop.Services.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace NADesktop.Pages.UserPages;

public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void LogIn_Click(object sender, RoutedEventArgs e)
    {
        WindowHelper.StartLoadingGIF(true);
        var userState = await MainHttp.UserRequest.LogInUser(LogInBTC.BannerTextBox.Text, PasswordBPC.OriginalTextProperty);
        WindowHelper.StartLoadingGIF(false);
        if (WindowHelper.CheckStatusCode(userState.Item1))
        {
            WindowHelper.BlockAuthorizeInterface(false);
            if (userState.Item2 == null)
                WindowHelper.PageFrame.Navigate(new ProfilePage());
            else
                WindowHelper.PageFrame.Navigate(new ProfilePage(userState.Item2));
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        WindowHelper.RunToBackPage();
    }
}
