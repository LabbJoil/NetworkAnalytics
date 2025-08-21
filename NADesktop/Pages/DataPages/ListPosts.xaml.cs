using NADesktop.Models.Enums;
using NADesktop.Models.Responses;
using NADesktop.Services.Server;
using NADesktop.Services.Helpers;
using NADesktop.Windows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NADesktop.Models.SocialNetwork;
using System.Text.Json;
using NADesktop.Services.Helper;

namespace NADesktop.Pages.AuthorizePages;

public partial class ListPosts : Page
{
    private readonly SocialNetworkEnum SocialNetwork;
    private readonly ObservableCollection<MessageModel> ReceivedMessages = [];
    private readonly long IdDialog;
    private long Offset = 0;


    public ListPosts(SocialNetworkEnum socialNetwork, long idDialog)
    {
        SocialNetwork = socialNetwork;
        IdDialog = idDialog;
        InitializeComponent();
        MessagesDataGrid.ItemsSource = ReceivedMessages;
        NumberChoosePostsTB.Text = "0";
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox)
            if (MessagesDataGrid.SelectedItem is MessageModel message)
            {
                message.IsSelected = checkBox.IsChecked ?? false;
                IncrementChoosePosts(message.IsSelected);
            }
    }
    
    private async void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
        {
            if (await GetNextHundredMessages() is List<MessageModel> nextMessages && nextMessages.Count > 0)
            {
                foreach (MessageModel message in nextMessages)
                    ReceivedMessages.Add(message);
                MessagesDataGrid.ScrollIntoView(ReceivedMessages[^nextMessages.Count]);
                MessagesDataGrid.UpdateLayout();
            }
        }
    }

    private async Task<List<MessageModel>?> GetNextHundredMessages()
    {
        WindowHelper.StartLoadingGIF(true);
        var dialogState = SocialNetwork switch
        {
            SocialNetworkEnum.Telegram => await MainHttp.TelegramRequest.GetHundredMessages(IdDialog, Offset),
            SocialNetworkEnum.VK => await MainHttp.VKRequest.GetHundredMessages(IdDialog, Offset),
            _ => (new(TypeMessage.Problem, DangerLevel.Error, "Unclear type of social network"), null)
        };
        WindowHelper.StartLoadingGIF(false);
        //if (dialogState.Item1.Type == TypeMessage.Captcha)
        //{
        //    CaptchaSolverWindow captchaSolverWindow = new(dialogState.Item1);
        //    if (captchaSolverWindow.ShowDialog() == true)
        //    {
        //        dialogState.Item1 = captchaSolverWindow.ResultRM.LoggingAnswer;
        //        dialogState.Item2 = !string.IsNullOrEmpty(captchaSolverWindow.ResultRM.RequestObject) ? JsonSerializer.Deserialize<DialogModel>(captchaSolverWindow.ResultRM.RequestObject) : null;
        //    }
        //}
        if (!WindowHelper.CheckStatusCode(dialogState.Item1, SocialNetwork))
            return null;

        if (dialogState.Item2 != null)
        {
            ChatNameTB.Text = dialogState.Item2.Title;
            Offset = dialogState.Item2.Offset;
            return dialogState.Item2.Messages;
        }
        return null;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
        => WindowHelper.RunToBackPage();

    private void SelectedLastHundredMessages_Click(object sender, RoutedEventArgs e)
    {
        int countMes = ReceivedMessages.Count > 100 ? 100 : ReceivedMessages.Count;
        for (int numMes = 0; numMes < countMes; numMes++)
        {
            ReceivedMessages[numMes].IsSelected = true;
            IncrementChoosePosts(true);
        }
    }

    private async void DoAnalysis_Click(object sender, RoutedEventArgs e)
    {
        DialogModel messagesAnalytic = new ()
        {
            Title = ChatNameTB.Text,
            SocialNetwork = SocialNetwork,
            Messages = ReceivedMessages.Where(mes => mes.IsSelected).ToList(),
        };
        LoggingHelper userLH = await MainHttp.UserRequest.SendDataAnalytic(messagesAnalytic);
        if (WindowHelper.CheckStatusCode(userLH, SocialNetwork))
            MessageBox.Show(userLH.Message);
    }

    private void IncrementChoosePosts(bool num)
    {
        int newValue;
        if (num) newValue = int.Parse(NumberChoosePostsTB.Text) + 1;
        else newValue = int.Parse(NumberChoosePostsTB.Text) - 1;
        NumberChoosePostsTB.Text = $"{newValue}";
    }
}
