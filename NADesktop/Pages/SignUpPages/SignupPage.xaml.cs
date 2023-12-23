
using NADesktop.Models.Domain;
using NADesktop.Models.Responses;
using NADesktop.Services.Helpers;
using NADesktop.Services.Server;
using System.Windows;
using System.Windows.Controls;

namespace NADesktop.Pages.UserPages;

public partial class SignupPage : Page
{
    public SignupPage()
    {
        InitializeComponent();
    }

    private async void CreateAccount_Click(object sender, RoutedEventArgs e)
    {
        if (!UserInputValidator.ValidateEmail(EmailBTC.BannerTextBox.Text) || !UserInputValidator.ValidateLogin(LoginBTC.BannerTextBox.Text) ||
            !UserInputValidator.ValidatePassword(PasswordBPC.OriginalTextProperty))
            return;
        WindowHelper.StartLoadingGIF(true);
        var userState = await MainHttp.UserRequest.SignUpUser(new User()
        {
            Login = LoginBTC.BannerTextBox.Text,
            Password = PasswordBPC.OriginalTextProperty,
            Name = NameBTC.BannerTextBox.Text,
            SecondName = SecondNameBTC.BannerTextBox.Text,
            Email = EmailBTC.BannerTextBox.Text
        });

        WindowHelper.StartLoadingGIF(false);
        if(WindowHelper.CheckStatusCode(userState.Item1))
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
