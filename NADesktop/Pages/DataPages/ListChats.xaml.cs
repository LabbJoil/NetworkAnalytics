
using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Services.Server;
using NADesktop.Services.Helpers;
using NADesktop.Windows;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NADesktop.Models.SocialNetwork;
using NADesktop.Services.Helper;
using System.Text.Json;
using NADesktop.Pages.UserPages;

namespace NADesktop.Pages.AuthorizePages;

public partial class ListChats : Page
{
    private readonly SocialNetworkEnum SocialNetwork;
    public ListChats(SocialNetworkEnum socialNetwork)
    {
        SocialNetwork = socialNetwork;
        InitializeComponent();
        WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
    }

    private async void ListChatsPage_Loaded(object sender, RoutedEventArgs e)
    {
        WindowHelper.StartLoadingGIF(true);
        var profileState = SocialNetwork switch
        {
            SocialNetworkEnum.Telegram => await MainHttp.TelegramRequest.GetAllDialogs(),
            SocialNetworkEnum.VK => await MainHttp.VKRequest.GetAllDialogs(),
            _ => (new(TypeMessage.Problem, DangerLevel.Error, "Unclear type of social network"), null)
        };
        WindowHelper.StartLoadingGIF(false);
        //if (profileState.Item1.Type == TypeMessage.Captcha)
        //{
        //    CaptchaSolverWindow captchaSolverWindow = new(profileState.Item1);
        //    if (captchaSolverWindow.ShowDialog() == true)
        //    {
        //        profileState.Item1 = captchaSolverWindow.ResultRM.LoggingAnswer;
        //        profileState.Item2 = !string.IsNullOrEmpty(captchaSolverWindow.ResultRM.RequestObject) ? JsonSerializer.Deserialize<ProfileModel>(captchaSolverWindow.ResultRM.RequestObject) : null;
        //    }
        //}
            
        if (!WindowHelper.CheckStatusCode(profileState.Item1, SocialNetwork))
            return;

        var dialogs = profileState.Item2;
        if (dialogs != null)
        {
            UserIcon.ImageSource = Convert(dialogs.SNUser?.Icon);
            UserNickName.Text = dialogs.SNUser?.NickName;
            ChatsDataGrid.ItemsSource = dialogs.Dialogs;
        }
    }

    private void ChatsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ChatsDataGrid.SelectedItem is ChatModel selectedChat)
           WindowHelper.PageFrame.Navigate(new ListPosts(SocialNetwork, selectedChat.Id));
    }

    public static BitmapImage? Convert(byte[]? bytesImage)
    {
        if (bytesImage == null)
            return null;
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = new MemoryStream(bytesImage);
        bitmapImage.EndInit();
        return bitmapImage;
    }

    private async void Signout_Click(object sender, RoutedEventArgs e)
    {
        WindowHelper.StartLoadingGIF(true);
        LoggingHelper signoutLH = SocialNetwork switch
        {
            SocialNetworkEnum.Telegram => await MainHttp.TelegramRequest.SignOut(),
            SocialNetworkEnum.VK => await MainHttp.VKRequest.SignOut(),
            _ => new(TypeMessage.Problem, DangerLevel.Error, "Unclear type of social network")
        };
        WindowHelper.StartLoadingGIF(false);
        //if (signoutLH.Type == TypeMessage.Captcha)
        //{
        //    CaptchaSolverWindow captchaSolverWindow = new(signoutLH);
        //    if (captchaSolverWindow.ShowDialog() == true)
        //        signoutLH = captchaSolverWindow.ResultRM.LoggingAnswer;
        //}

        if (WindowHelper.CheckStatusCode(signoutLH, SocialNetwork))
        {
            MessageBox.Show("Вы успешно вышли из аккаунта");
            WindowHelper.PageFrame.Navigate(new ProfilePage());
        }
    }
}
