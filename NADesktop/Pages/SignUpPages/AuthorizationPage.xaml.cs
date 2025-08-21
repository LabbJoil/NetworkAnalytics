using NADesktop.Services.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace NADesktop.Pages.UserPages
{
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
            WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            WindowHelper.PageFrame.Navigate(new LoginPage());
        }

        private void SingUp_Click(object sender, RoutedEventArgs e)
        {
            WindowHelper.PageFrame.Navigate(new SignupPage());
        }
    }
}
