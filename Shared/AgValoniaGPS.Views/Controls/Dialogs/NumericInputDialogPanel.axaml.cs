using Avalonia.Controls;
using Avalonia.Input;

namespace AgValoniaGPS.Views.Controls.Dialogs;

public partial class NumericInputDialogPanel : UserControl
{
    public NumericInputDialogPanel()
    {
        InitializeComponent();
    }

    private void Backdrop_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Clicking the backdrop cancels the dialog
        if (DataContext is AgValoniaGPS.ViewModels.MainViewModel vm)
        {
            vm.CancelNumericInputDialogCommand?.Execute(null);
        }
    }
}
