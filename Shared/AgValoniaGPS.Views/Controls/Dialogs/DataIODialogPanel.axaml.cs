using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AgValoniaGPS.ViewModels;

namespace AgValoniaGPS.Views.Controls.Dialogs;

public partial class DataIODialogPanel : UserControl
{
    private MainViewModel? ViewModel => DataContext as MainViewModel;
    private TextBox? _activeTextBox;
    private string _activeField = "";

    public DataIODialogPanel()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // Subscribe to keyboard text changes
        if (ViewModel != null)
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        if (ViewModel != null)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.DataIOKeyboardText) && ViewModel != null && _activeTextBox != null)
        {
            // Update the active text box from keyboard input
            switch (_activeField)
            {
                case "CasterAddress":
                    ViewModel.NtripCasterAddress = ViewModel.DataIOKeyboardText;
                    break;
                case "CasterPort":
                    if (int.TryParse(ViewModel.DataIOKeyboardText, out int port))
                        ViewModel.NtripCasterPort = port;
                    break;
                case "MountPoint":
                    ViewModel.NtripMountPoint = ViewModel.DataIOKeyboardText;
                    break;
                case "Username":
                    ViewModel.NtripUsername = ViewModel.DataIOKeyboardText;
                    break;
                case "Password":
                    ViewModel.NtripPassword = ViewModel.DataIOKeyboardText;
                    break;
            }
        }
    }

    private void ShowKeyboardFor(TextBox textBox, string fieldName, string currentValue)
    {
        _activeTextBox = textBox;
        _activeField = fieldName;

        if (ViewModel != null)
        {
            ViewModel.DataIOKeyboardText = currentValue;
            ViewModel.IsDataIOKeyboardVisible = true;
        }
    }

    private void CasterAddress_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is TextBox tb && ViewModel != null)
        {
            ShowKeyboardFor(tb, "CasterAddress", ViewModel.NtripCasterAddress);
        }
    }

    private void CasterPort_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is TextBox tb && ViewModel != null)
        {
            ShowKeyboardFor(tb, "CasterPort", ViewModel.NtripCasterPort.ToString());
        }
    }

    private void MountPoint_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is TextBox tb && ViewModel != null)
        {
            ShowKeyboardFor(tb, "MountPoint", ViewModel.NtripMountPoint);
        }
    }

    private void Username_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is TextBox tb && ViewModel != null)
        {
            ShowKeyboardFor(tb, "Username", ViewModel.NtripUsername);
        }
    }

    private void Password_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is TextBox tb && ViewModel != null)
        {
            ShowKeyboardFor(tb, "Password", ViewModel.NtripPassword);
        }
    }

    private async void BtnNtripConnect_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            await ViewModel.ConnectToNtripAsync();
        }
    }

    private async void BtnNtripDisconnect_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            await ViewModel.DisconnectFromNtripAsync();
        }
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.SaveNtripSettings();
        }
    }

    private void Backdrop_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        CloseDialog();
        e.Handled = true;
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        CloseDialog();
    }

    private void CloseDialog()
    {
        if (ViewModel != null)
        {
            ViewModel.IsDataIOKeyboardVisible = false;
            ViewModel.IsDataIODialogVisible = false;
        }
    }
}
