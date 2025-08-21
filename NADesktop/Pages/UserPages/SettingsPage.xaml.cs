using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Personal_Elements;
using NADesktop.Services.Server;
using NADesktop.Services.Helpers;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NADesktop.Services.Helper;

namespace NADesktop.Pages.UserPages
{
    public partial class SettingsPage : Page
    {
        [GeneratedRegex("([0-9.]){1}")]
        private static partial Regex IPRegex();

        [GeneratedRegex("([0-9]){1}")]
        private static partial Regex PortRegex();

        public SettingsPage()
        {
            InitializeComponent();
            IPTextBox.BannerTextBox.Text = MainHttp.IP;
            PortTextBox.BannerTextBox.Text = MainHttp.Port;
            WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is BannerTextboxControl tbx)
            {
                Regex nowRegex;
                int neccessaryNumbers;
                if (tbx.Name == "IPTextBox")
                {
                    neccessaryNumbers = 15;
                    nowRegex = IPRegex();
                }
                else
                {
                    nowRegex = PortRegex();
                    neccessaryNumbers = 4;
                }
                if (tbx.BannerTextBox.Text.Length < neccessaryNumbers && CheckEnterSymbol(e, nowRegex)) return;
            }
            e.Handled = true;
        }

        private static bool CheckEnterSymbol(InputEventArgs keyInput, Regex? regex)
        {
            if (keyInput is KeyEventArgs keyHot)
            {
                if (keyHot.Key == Key.Delete)
                    return true;
                else if (keyHot.Key == Key.Back)
                    return true;
            }
            else if (regex != null && keyInput is TextCompositionEventArgs keyTextComposition)
                if (!string.IsNullOrEmpty(keyTextComposition.Text) && regex.IsMatch(keyTextComposition.Text))
                    return true;
            return false;
        }

        private async void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (IPTextBox.BannerTextBox.Text.Count(c => c == '.') < 3 || PortTextBox.BannerTextBox.Text == "")
            {
                MessageBox.Show("Неправильно введены данные");
                return;
            }
            LoggingHelper paramLH = await MainHttp.ChangeParams(IPTextBox.BannerTextBox.Text, PortTextBox.BannerTextBox.Text);
            if (paramLH.DangerLevel != DangerLevel.Oke)
            {
                MessageBox.Show($"Статус: {paramLH.DangerLevel}\nОшибка: {paramLH.Message}");
                return;
            }
            MessageBox.Show("Изменения сохраннены");
        }
    }
}
