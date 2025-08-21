
using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Services.Server;
using NADesktop.Services.Helpers;
using NADesktop.Services.Helper;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace NADesktop.Pages.AuthorizePages
{
    public partial class EnterLogin : Page
    {
        private readonly SocialNetworkEnum SocialNetwork;
        public EnterLogin(SocialNetworkEnum socialNetwork)
        {
            InitializeComponent();
            SocialNetwork = socialNetwork;
            WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
        }

        private async void EnterLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBTC.BannerTextBox.Text;
            ResponseModel addAttrRM;

            WindowHelper.StartLoadingGIF(true);
            if (SocialNetwork == SocialNetworkEnum.Telegram)
                addAttrRM = await MainHttp.TelegramRequest.AdditionalAttributes(new(TypeMessage.Login, DangerLevel.Oke, login));
            else
                addAttrRM = await MainHttp.VKRequest.AdditionalAttributes(new(TypeMessage.Login, DangerLevel.Oke, login));

            WindowHelper.CheckStatusCode(addAttrRM.LoggingAnswer, SocialNetwork);
            WindowHelper.StartLoadingGIF(false);
        }
    }
}
