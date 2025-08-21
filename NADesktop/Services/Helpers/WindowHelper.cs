
using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Pages.AuthorizePages;
using NADesktop.Pages.UserPages;
using NADesktop.Services.Helper;
using NADesktop.Windows;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace NADesktop.Services.Helpers;

internal static class WindowHelper
{
    private static readonly BitmapImage LoadingBitmapImage;
    public static Frame PageFrame = new();
    private static Rectangle OverlayRectangle = new();
    private static Image LoadingGIF = new();
    private static Slider MenuSlider = new();

    static WindowHelper()
    {
        LoadingBitmapImage = new();
        LoadingBitmapImage.BeginInit();
        LoadingBitmapImage.UriSource = new(@"/Assets/GIF/LoadingGIF.gif", UriKind.Relative);
        LoadingBitmapImage.EndInit();
    }

    public static void LoadWindowsElements(Frame frame, Rectangle rectangle, Image imageGIF, Slider slider)
    {
        PageFrame = frame;
        OverlayRectangle = rectangle;
        LoadingGIF = imageGIF;
        MenuSlider = slider;
    }

    public static void RemoveAllBackPages(object sender, NavigationEventArgs e)
    {
        PageFrame.Navigated -= RemoveAllBackPages;
        while (PageFrame.NavigationService.CanGoBack)
            PageFrame.NavigationService.RemoveBackEntry();
    }

    public static void RemoveBackPage(object sender, NavigationEventArgs e)
    {
        PageFrame.Navigated -= RemoveBackPage;
        if (PageFrame.NavigationService.CanGoBack)
            PageFrame.NavigationService.RemoveBackEntry();
    }

    public static void StartLoadingGIF(bool isOpen)
    {
        Visibility isVisibility;
        BitmapImage? loadingImage;
        if (isOpen)
        {
            isVisibility = Visibility.Visible;
            loadingImage = LoadingBitmapImage;
        }
        else
        {
            isVisibility = Visibility.Collapsed;
            loadingImage = null;
        }
        LoadingGIF.Visibility = isVisibility;
        OverlayRectangle.Visibility = isVisibility;
        OverlayRectangle.IsHitTestVisible = isOpen;
        ImageBehavior.SetAnimatedSource(LoadingGIF, loadingImage);
    }

    public static void BlockAuthorizeInterface(bool isBlock)
    {
        if (MenuSlider.Template.FindName("TelegramB", MenuSlider) is Button myTasksButton)
            myTasksButton.IsEnabled = !isBlock;
        if (MenuSlider.Template.FindName("VKB", MenuSlider) is Button AccessTasksButton)
            AccessTasksButton.IsEnabled = !isBlock;
        if (MenuSlider.Template.FindName("ProfileB", MenuSlider) is Button ProfileButton)
            ProfileButton.IsEnabled = !isBlock;
        if (MenuSlider.Template.FindName("ReportsB", MenuSlider) is Button ReportsButton)
            ReportsButton.IsEnabled = !isBlock;
    }

    public static bool CheckStatusCode(LoggingHelper logging, SocialNetworkEnum? socialNetwork = null)
    {
        if (socialNetwork != null)
            switch (logging.Type)
            {
                case TypeMessage.NeedLogin:
                    PageFrame.Navigate(new EnterLogin((SocialNetworkEnum)socialNetwork));
                    return false;
                case TypeMessage.NeedPassword:
                    PageFrame.Navigate(new EnterPassword((SocialNetworkEnum)socialNetwork));
                    return false;
                case TypeMessage.NeedCode:
                    PageFrame.Navigate(new EnterCode((SocialNetworkEnum)socialNetwork));
                    return false;
                case TypeMessage.GoodNetworkAuthorize:
                    PageFrame.Navigate(new ListChats((SocialNetworkEnum)socialNetwork));
                    return false;
                case TypeMessage.NeedCaptcha:
                    CaptchaSolverWindow captchaSolverWindow = new(logging);
                    if (captchaSolverWindow.ShowDialog() == true)
                        return CheckStatusCode(captchaSolverWindow.ResultRM.LoggingAnswer, socialNetwork);
                    return false;
            }
        if (logging.DangerLevel != DangerLevel.Oke)
        {
            MessageBox.Show($"Уровень опасности: {logging.DangerLevel}\nСтатус код: {logging.StatusCode}\nОшибка: {logging.Message}");
            if (logging.StatusCode == HttpStatusCode.Unauthorized || logging.Type == TypeMessage.NoneAuthorize)
            {
                PageFrame.Content = null;
                BlockAuthorizeInterface(true);
                PageFrame.Navigate(new AuthorizationPage());
            }
            return false;
        }
        return true;
    }

    public static void RunToBackPage()
    {
        PageFrame.Navigated += RemoveBackPage;
        PageFrame.NavigationService.GoBack();
    }
}