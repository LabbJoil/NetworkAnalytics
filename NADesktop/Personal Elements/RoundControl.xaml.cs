using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NADesktop.Personal_Elements;

public partial class RoundControl : UserControl
{
    public ImageSource? ImageLeaveButton { get; set; }
    public ImageSource? ImageEnterButton { get; set; }
    public event RoutedEventHandler? Click;

    public string? Text { get; set; }
    public new double Height { get; set; }
    public new double Width { get; set; }

    public RoundControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    public void ChangeFullSize(double newSize, double newSizeFont)
    {
        Height = newSize; Width = newSize;
        RoundButton.Height = newSize;
        RoundButton.Width = newSize;
        RoundButton.FontSize = newSizeFont;
        Height = newSize;
        Width = newSize;
    }

    private void RoundButton_MouseLeftButtonDown(object sender, RoutedEventArgs e) 
        => Click?.Invoke(this, e);
}

