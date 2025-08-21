using NPOI.SS.UserModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NADesktop.Personal_Elements;

public partial class BannerPasswordControl : UserControl
{
    private bool IsHidenTextBox = true;
    private string OriginalText = string.Empty;
    public new double Height { get; set; } = 40;
    public new double FontSize { get; set; } = 14;

    public string BannerText { get; set; } = string.Empty;
    public string OriginalTextProperty { get => OriginalText; set { } }

    public BannerPasswordControl()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += BannerTextboxControl_Loaded;
        DataObject.AddPastingHandler(BannerTextBox, OnPaste);
    }

    private void BannerTextboxControl_Loaded(object sender, RoutedEventArgs e)
    {
        BannerTextBox.FontSize = FontSize + Height / 4.5;
        BannerTextBlock.FontSize = FontSize + Height / 5;
        HideTextButton.RoundButton.FontSize = Height/ 2.5;

        HideTextButton.RoundButton.Width = Height;
        HideTextButton.RoundButton.Height = Height;

        ColumnDefinitionCollection thirdGridColumn = BannerTextBoxGrid.ColumnDefinitions;
        thirdGridColumn[1].Width = new GridLength(Height / 10);
        thirdGridColumn[2].Width = new GridLength(Height);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    private void HideTextButton_Click(object sender, RoutedEventArgs e)
    {
        RoundControl nowRownControl = (RoundControl)sender;
        int cursorPosition = BannerTextBox.CaretIndex;
        if (IsHidenTextBox)
        {
            nowRownControl.RoundButton.Content = "ಠ_ ಠ";
            BannerTextBox.Text = OriginalText;
        }
        else
        {
            nowRownControl.RoundButton.Content = "(~.~)";
            BannerTextBox.Text = new string('●', BannerTextBox.Text.Length);
        }
        IsHidenTextBox = ! IsHidenTextBox;
        BannerTextBox.Focus();
        BannerTextBox.CaretIndex = cursorPosition;
    }

    private void BannerTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox nowTextBox = (TextBox)sender;
        if (string.IsNullOrEmpty(nowTextBox.Text)) BannerTextBlock.Visibility = Visibility.Visible;
        else BannerTextBlock.Visibility = Visibility.Hidden;
    }

    private void BannerTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        TextBox nowTextBox = (TextBox)sender;
        if (CheckEnterSymbol(e, nowTextBox)) return;
    }

    private void BannerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        TextBox nowTextBox = (TextBox)sender;
        if (CheckEnterSymbol(e, nowTextBox)) return;
        e.Handled = true;
    }        

    private void CustomTextBox_MouseEvent(object sender, MouseEventArgs e)
    {
        TextBox nowTextBox = (TextBox)sender;
        if (nowTextBox.IsMouseOver == true) BannerBorder.Background = new SolidColorBrush(Colors.LightGray);
        else BannerBorder.Background = new SolidColorBrush(Colors.White);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    private bool CheckEnterSymbol(InputEventArgs keyInput, TextBox changeableTextBox)
    {
        string? keyText = null;
        int cursorPosition = changeableTextBox.CaretIndex;
        int countDelete = changeableTextBox.SelectionLength > 1 ? changeableTextBox.SelectionLength : 1;

        if (keyInput is KeyEventArgs keyHot)
        {
            if (keyHot.Key == Key.Delete)
            {
                if (cursorPosition == OriginalText.Length) return true;
                OriginalText = OriginalText.Remove(cursorPosition, countDelete);
                return true;
            }
            else if (keyHot.Key == Key.Back && (cursorPosition > 0 || countDelete > 1))
            {
                if (countDelete == 1) cursorPosition--;
                OriginalText = OriginalText.Remove(cursorPosition, countDelete);
                return true;
            }
            else if (keyHot.Key == Key.Space) keyText = " ";
        }
        else if (keyInput is TextCompositionEventArgs keyTextComposition) keyText = keyTextComposition.Text;

        if (!string.IsNullOrEmpty(keyText) && Regex.IsMatch(keyText, @"([a-zA-Z0-9\[\]\\\-!""#$%&'()*+,./:;<=>?@^_`{|}~ ]){1}"))
        {
            OriginalText = OriginalText.Insert(cursorPosition, keyText);
            if (IsHidenTextBox)
            {
                keyInput.Handled = true;
                changeableTextBox.Text += "●";
                changeableTextBox.CaretIndex = cursorPosition + 1;
            }
            return true;
        }
        else return false;
    }
    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            string pastedText = (string)e.DataObject.GetData(DataFormats.Text);
            int cursorPosition = BannerTextBox.CaretIndex;
            OriginalText = OriginalText.Insert(cursorPosition, pastedText);
            if (IsHidenTextBox)
            {
                DataObject newDataObject = new ();
                newDataObject.SetData(DataFormats.Text, new string('●', pastedText.Length));
                e.DataObject = newDataObject;
            }
        }
    }

    public void ResertPassword()
    {
        OriginalText = "";
        BannerTextBox.Text = "";
    }
}
