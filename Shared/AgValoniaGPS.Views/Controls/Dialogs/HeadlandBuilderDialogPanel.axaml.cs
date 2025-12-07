using Avalonia.Controls;
using Avalonia.Input;

namespace AgValoniaGPS.Views.Controls.Dialogs;

public partial class HeadlandBuilderDialogPanel : UserControl
{
    public HeadlandBuilderDialogPanel()
    {
        InitializeComponent();
    }

    private void Backdrop_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Clicking the backdrop closes the dialog
        if (DataContext is AgValoniaGPS.ViewModels.MainViewModel vm)
        {
            vm.CloseHeadlandBuilderCommand?.Execute(null);
        }
    }
}
