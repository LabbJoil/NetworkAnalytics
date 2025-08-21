using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NADesktop.Personal_Elements;

public partial class BannerTextboxControl : UserControl
{
    public string BannerText { get; set; } = string.Empty;
    public double NewHeight { get; set; } = 40;
    public double NewFontSize { get; set; } = 14;

    public BannerTextboxControl()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += BannerTextboxControl_Loaded;
    }

    private void BannerTextboxControl_Loaded(object sender, RoutedEventArgs e)
    {
        BannerTextBox.FontSize = NewFontSize + NewHeight / 5;
        BannerTextBlock.FontSize = NewFontSize + NewHeight / 5;
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    private void BannerTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox nowTextBox = (TextBox)sender;
        if (string.IsNullOrEmpty(nowTextBox.Text)) BannerTextBlock.Visibility = Visibility.Visible;
        else BannerTextBlock.Visibility = Visibility.Hidden;
    }


    private void CustomTextBox_MouseEvent(object sender, MouseEventArgs e)
    {
        TextBox nowTextBox = (TextBox)sender;
        if (nowTextBox.IsMouseOver == true) BannerBorder.Background = new SolidColorBrush(Colors.LightGray);
        else BannerBorder.Background = new SolidColorBrush(Colors.White);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

}
