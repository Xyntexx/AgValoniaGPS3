using Avalonia.Controls;
using Avalonia.Input;

namespace AgValoniaGPS.Views.Controls.Dialogs;

public partial class IsoXmlImportDialogPanel : UserControl
{
    public IsoXmlImportDialogPanel()
    {
        InitializeComponent();
    }

    private void Backdrop_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is AgValoniaGPS.ViewModels.MainViewModel vm)
        {
            vm.CancelIsoXmlImportDialogCommand?.Execute(null);
        }
        e.Handled = true;
    }
}
