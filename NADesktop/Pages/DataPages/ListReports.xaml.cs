
using NADesktop.Models.Domain;
using NADesktop.Models.SocialNetwork;
using NADesktop.Pages.Data;
using NADesktop.Services.Helper;
using NADesktop.Services.Helpers;
using NADesktop.Services.Server;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NADesktop.Pages.AuthorizePages;

public partial class ListReports : Page
{
    private readonly ObservableCollection<Report> ReceivedReports = [];
    public ListReports()
    {
        InitializeComponent();
        WindowHelper.PageFrame.Navigated += WindowHelper.RemoveAllBackPages;
        ReportsDataGrid.ItemsSource = ReceivedReports;
    }

    private async void ListReports_Loaded(object sender, RoutedEventArgs e)
    {
        ReceivedReports.Clear();
        WindowHelper.StartLoadingGIF(true);
        var reports = await MainHttp.UserRequest.GetReports();
        WindowHelper.StartLoadingGIF(false);
        if (!WindowHelper.CheckStatusCode(reports.Item1) || reports.Item2 == null)
            return;
        foreach (var report in reports.Item2)
            ReceivedReports.Add(report);
    }

    private async void DeleteReport_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Report report)
        {
            WindowHelper.StartLoadingGIF(true);
            LoggingHelper deleteReportLH = await MainHttp.UserRequest.DeleteReports(report.Id);
            WindowHelper.StartLoadingGIF(false);
            if (!WindowHelper.CheckStatusCode(deleteReportLH))
                return;
            ReceivedReports.Remove(report);
        }
    }

    private void ViewReport_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Report report)
            WindowHelper.PageFrame.Navigate(new ResultAnalysis(report.Id));
    }
}
