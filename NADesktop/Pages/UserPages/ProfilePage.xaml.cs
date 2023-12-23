using NADesktop.Models.Enums;
using NADesktop.Services.Server;
using NADesktop.Services.Helpers;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using NADesktop.Models.Responses;
using NADesktop.Models.Domain;
using NADesktop.Services.Helper;

namespace NADesktop.Pages.UserPages;

public partial class ProfilePage : Page
{
    public ProfilePage(User user)
    {
        InitializeComponent();
        WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
        EnterUser(user);
    }

    public ProfilePage()
    {
        InitializeComponent();
        WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
        Loaded += ProfilePage_Loaded;
    }

    private async void ProfilePage_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= ProfilePage_Loaded;
        WindowHelper.StartLoadingGIF(true);
        var userState = await MainHttp.UserRequest.GetUserInfo();
        WindowHelper.StartLoadingGIF(false);
        if (WindowHelper.CheckStatusCode(userState.Item1))
            EnterUser(userState.Item2);
    }

    private void ChangePasswordB_Click(object sender, RoutedEventArgs e)
    {
        WindowHelper.PageFrame.Navigate(new ChangePasswordPage());
    }

    private async void ChangeUserB_Click(object sender, RoutedEventArgs e)
    {
        if (!UserInputValidator.ValidateEmail(EMail.BannerTextBox.Text) || !UserInputValidator.ValidateLogin(LoginBTC.BannerTextBox.Text))
            return;
        User updateUser = new()
        {
            Login = LoginBTC.BannerTextBox.Text,
            Name = NameBTC.BannerTextBox.Text,
            SecondName = SecondNameBTC.BannerTextBox.Text,
            Email = EMail.BannerTextBox.Text
        };
        WindowHelper.StartLoadingGIF(true);
        var userState = await MainHttp.UserRequest.UpdateUserInfo(updateUser);
        WindowHelper.StartLoadingGIF(false);
        if (WindowHelper.CheckStatusCode(userState.Item1))
            EnterUser(userState.Item2);
    }

    private async void DeleteUserB_Click(object sender, RoutedEventArgs e)
    {
        WindowHelper.StartLoadingGIF(true);
        LoggingHelper userLH = await MainHttp.UserRequest.DeleteUser();
        WindowHelper.StartLoadingGIF(false);
        if (WindowHelper.CheckStatusCode(userLH))
        {
            MessageBox.Show("Аккаунт успешно удалён");
            WindowHelper.PageFrame.Navigate(new AuthorizationPage());
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void EnterUser(User? user)
    {
        if (user == null)
        {
            MessageBox.Show("Ошибка получения пользователя");
            return;
        }
        LoginBTC.BannerTextBox.Text = user.Login;
        NameBTC.BannerTextBox.Text = user.Name;
        SecondNameBTC.BannerTextBox.Text = user.SecondName;
        EMail.BannerTextBox.Text = user.Email;
    }
}
