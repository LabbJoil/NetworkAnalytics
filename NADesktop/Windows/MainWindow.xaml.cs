using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Pages.AuthorizePages;
using NADesktop.Pages.Data;
using NADesktop.Pages.UserPages;
using NADesktop.Personal_Elements;
using NADesktop.Services.Server;
using NADesktop.Services.Helpers;
using NADesktop.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using NADesktop.Services.Helper;

namespace NADesktop;

public partial class MainWindow : Window
{
    private bool IsOpenMenu = false;

    public MainWindow()
    {
        InitializeComponent();
        WindowHelper.LoadWindowsElements(PageFrame, OverlayRectangle, LoadingGIF, MenuSlider);
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        WindowHelper.PageFrame.Navigate(new ProfilePage());
    }

    private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        => await MainHttp.SaveParams();

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void InteractionMenu(object sender, RoutedEventArgs e)
       => InteractionSliderButton(MenuSlider, MenuButton);

    private void InteractionSliderButton(Slider slider, RoundControl button)
    {
        DoubleAnimation animationSlider;
        DoubleAnimation animationButton;
        int sliderOpenTo, buttonOpenTo;

        if (IsOpenMenu)
        {
            sliderOpenTo = 0;
            buttonOpenTo = 40;
        }
        else
        {
            sliderOpenTo = 200;
            buttonOpenTo = 0;
        }

        animationSlider = new DoubleAnimation
        {
            From = slider.ActualWidth,
            To = sliderOpenTo,
            Duration = TimeSpan.FromSeconds(0.3)
        };
        animationButton = new DoubleAnimation
        {
            From = button.ActualWidth,
            To = buttonOpenTo,
            Duration = TimeSpan.FromSeconds(0.3)
        };

        Storyboard storyboard = new ();
        storyboard.Children.Add(animationSlider);
        storyboard.Children.Add(animationButton);

        Storyboard.SetTarget(animationSlider, slider);
        Storyboard.SetTarget(animationButton, button);

        Storyboard.SetTargetProperty(animationSlider, new PropertyPath("Width"));
        Storyboard.SetTargetProperty(animationButton, new PropertyPath("Width"));
        storyboard.Begin();
        CloseSliderButton.Visibility = CloseSliderButton.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        CloseSliderButton.IsEnabled = !IsOpenMenu;
        IsOpenMenu = !IsOpenMenu;
    }

    private void OpenTelegram(object sender, RoutedEventArgs e)
    {
        PageFrame.Navigate(new ListChats(SocialNetworkEnum.Telegram));
        InteractionSliderButton(MenuSlider, MenuButton);
    }

    private void OpenVK(object sender, RoutedEventArgs e)
    {
        PageFrame.Navigate(new ListChats(SocialNetworkEnum.VK));
        InteractionSliderButton(MenuSlider, MenuButton);
    }

    private void OpenAuthorization(object sender, RoutedEventArgs e)
    {
        PageFrame.Navigate(new AuthorizationPage());
        InteractionSliderButton(MenuSlider, MenuButton);
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        PageFrame.Navigate(new SettingsPage());
        InteractionSliderButton(MenuSlider, MenuButton);
    }

    private void OpenEditProfile(object sender, RoutedEventArgs e)
    {
        PageFrame.Navigate(new ProfilePage());
        InteractionSliderButton(MenuSlider, MenuButton);
    }

    private void OpenReports(object sender, RoutedEventArgs e)
    {
        PageFrame.Navigate(new ListReports());
        InteractionSliderButton(MenuSlider, MenuButton);
    }
}
