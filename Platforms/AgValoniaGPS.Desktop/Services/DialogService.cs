using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using AgValoniaGPS.Services;
using AgValoniaGPS.Services.Interfaces;
using AgValoniaGPS.Models;
using AgValoniaGPS.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AgValoniaGPS.Desktop.Services;

/// <summary>
/// Desktop implementation of IDialogService.
/// Shows Avalonia dialogs for user interaction.
/// </summary>
public class DialogService : IDialogService
{
    private readonly IServiceProvider _serviceProvider;
    private Window? _parentWindow;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Set the parent window for dialogs. Must be called after window is created.
    /// </summary>
    public void SetParentWindow(Window window)
    {
        _parentWindow = window;
    }

    private Window GetParentWindow()
    {
        if (_parentWindow == null)
            throw new InvalidOperationException("Parent window not set. Call SetParentWindow first.");
        return _parentWindow;
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30))
        };

        var stack = new StackPanel
        {
            Margin = new Avalonia.Thickness(24),
            Spacing = 16
        };

        stack.Children.Add(new TextBlock
        {
            Text = message,
            FontSize = 16,
            Foreground = Brushes.White,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        });

        var okButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = HorizontalAlignment.Center,
            Padding = new Avalonia.Thickness(24, 8),
            Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
            Foreground = Brushes.White,
            BorderThickness = new Avalonia.Thickness(0)
        };
        okButton.Click += (s, args) => messageBox.Close();
        stack.Children.Add(okButton);

        messageBox.Content = stack;
        await messageBox.ShowDialog(GetParentWindow());
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        var result = false;
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30))
        };

        var stack = new StackPanel
        {
            Margin = new Avalonia.Thickness(24),
            Spacing = 16
        };

        stack.Children.Add(new TextBlock
        {
            Text = message,
            FontSize = 16,
            Foreground = Brushes.White,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        });

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 16
        };

        var yesButton = new Button
        {
            Content = "Yes",
            Padding = new Avalonia.Thickness(24, 8),
            Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
            Foreground = Brushes.White,
            BorderThickness = new Avalonia.Thickness(0)
        };
        yesButton.Click += (s, args) => { result = true; dialog.Close(); };

        var noButton = new Button
        {
            Content = "No",
            Padding = new Avalonia.Thickness(24, 8),
            Background = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
            Foreground = Brushes.White,
            BorderThickness = new Avalonia.Thickness(0)
        };
        noButton.Click += (s, args) => { result = false; dialog.Close(); };

        buttonPanel.Children.Add(yesButton);
        buttonPanel.Children.Add(noButton);
        stack.Children.Add(buttonPanel);

        dialog.Content = stack;
        await dialog.ShowDialog(GetParentWindow());
        return result;
    }

    public async Task ShowDataIODialogAsync()
    {
        var dialog = new DataIODialog();
        // DataIODialog uses its own ViewModel or the main ViewModel
        await dialog.ShowDialog(GetParentWindow());
    }

    public async Task<(double Latitude, double Longitude)?> ShowSimCoordsDialogAsync(double currentLatitude, double currentLongitude)
    {
        var dialog = new SimCoordsDialog(currentLatitude, currentLongitude);
        await dialog.ShowDialog(GetParentWindow());

        if (dialog.DialogResult)
        {
            return (dialog.Latitude, dialog.Longitude);
        }
        return null;
    }

    public async Task<DialogFieldSelectionResult?> ShowFieldSelectionDialogAsync(string fieldsDirectory)
    {
        var fieldService = _serviceProvider.GetRequiredService<IFieldService>();
        var dialog = new FieldSelectionDialog(fieldService, fieldsDirectory);
        var result = await dialog.ShowDialog<bool>(GetParentWindow());

        if (result && dialog.SelectedField != null)
        {
            return new DialogFieldSelectionResult
            {
                FieldName = System.IO.Path.GetFileName(dialog.SelectedField.DirectoryPath),
                DirectoryPath = dialog.SelectedField.DirectoryPath,
                Boundary = dialog.SelectedField.Boundary
            };
        }
        return null;
    }

    public async Task<DialogNewFieldResult?> ShowNewFieldDialogAsync(Position currentPosition)
    {
        var dialog = new NewFieldDialog(currentPosition);
        var result = await dialog.ShowDialog<(bool Success, string FieldName, Position Origin)>(GetParentWindow());

        if (result.Success && !string.IsNullOrWhiteSpace(result.FieldName))
        {
            return new DialogNewFieldResult
            {
                FieldName = result.FieldName,
                Origin = result.Origin
            };
        }
        return null;
    }

    public async Task<DialogFromExistingFieldResult?> ShowFromExistingFieldDialogAsync(string fieldsDirectory)
    {
        var fieldService = _serviceProvider.GetRequiredService<IFieldService>();
        var dialog = new FromExistingFieldDialog(fieldService, fieldsDirectory, "");
        var result = await dialog.ShowDialog<bool?>(GetParentWindow());

        if (result == true && dialog.Result != null)
        {
            return new DialogFromExistingFieldResult
            {
                SourceFieldPath = dialog.Result.SourceFieldPath,
                NewFieldName = dialog.Result.NewFieldName,
                CopyFlags = dialog.Result.CopyOptions.IncludeFlags,
                CopyMapping = dialog.Result.CopyOptions.IncludeMapping,
                CopyHeadland = dialog.Result.CopyOptions.IncludeHeadland,
                CopyLines = dialog.Result.CopyOptions.IncludeLines
            };
        }
        return null;
    }

    public async Task<DialogIsoXmlImportResult?> ShowIsoXmlImportDialogAsync(string fieldsDirectory)
    {
        var dialog = new IsoXmlImportDialog(fieldsDirectory);
        var result = await dialog.ShowDialog<bool?>(GetParentWindow());

        if (result == true && dialog.Result != null)
        {
            return new DialogIsoXmlImportResult
            {
                FieldName = dialog.Result.NewFieldName,
                FieldDirectory = dialog.Result.NewFieldPath,
                ImportedBoundary = null // TODO: Convert IsoXmlBoundary to Boundary if needed
            };
        }
        return null;
    }

    public async Task<DialogKmlImportResult?> ShowKmlImportDialogAsync(string fieldsDirectory)
    {
        var dialog = new KmlImportDialog(fieldsDirectory);
        var result = await dialog.ShowDialog<bool?>(GetParentWindow());

        if (result == true && dialog.Result != null)
        {
            return new DialogKmlImportResult
            {
                FieldName = dialog.Result.NewFieldName,
                FieldDirectory = dialog.Result.NewFieldPath,
                ImportedBoundary = null // TODO: Convert boundary points to Boundary if needed
            };
        }
        return null;
    }

    public async Task<DialogAgShareDownloadResult?> ShowAgShareDownloadDialogAsync(string apiKey, string fieldsDirectory)
    {
        var serverUrl = "https://agshare.agopengps.com";
        var dialog = new AgShareDownloadDialog(fieldsDirectory, serverUrl, apiKey);
        var result = await dialog.ShowDialog<bool?>(GetParentWindow());

        if (result == true && dialog.Result != null)
        {
            return new DialogAgShareDownloadResult
            {
                FieldName = dialog.Result.FieldName,
                FieldDirectory = dialog.Result.FieldPath
            };
        }
        return null;
    }

    public async Task<bool> ShowAgShareUploadDialogAsync(string apiKey, string fieldName, string fieldDirectory)
    {
        var dialog = new AgShareUploadDialog(apiKey, fieldName, fieldDirectory);
        var result = await dialog.ShowDialog<bool>(GetParentWindow());
        return result;
    }

    public async Task ShowAgShareSettingsDialogAsync()
    {
        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        var dialog = new AgShareSettingsDialog();
        await dialog.ShowDialog(GetParentWindow());
    }

    public async Task<DialogMapBoundaryResult?> ShowMapBoundaryDialogAsync(double centerLatitude, double centerLongitude)
    {
        var dialog = new MapsuiBoundaryDialog(centerLatitude, centerLongitude);
        var result = await dialog.ShowDialog<bool?>(GetParentWindow());

        if (result == true && dialog.Result?.BoundaryPoints != null && dialog.Result.BoundaryPoints.Count > 0)
        {
            return new DialogMapBoundaryResult
            {
                Points = dialog.Result.BoundaryPoints
            };
        }
        return null;
    }

    public async Task<double?> ShowNumericInputDialogAsync(string title, string prompt, double currentValue, double min, double max)
    {
        double? result = null;
        var dialog = new Window
        {
            Title = title,
            Width = 350,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30))
        };

        var stack = new StackPanel
        {
            Margin = new Avalonia.Thickness(24),
            Spacing = 12
        };

        stack.Children.Add(new TextBlock
        {
            Text = prompt,
            FontSize = 14,
            Foreground = Brushes.White
        });

        var numericInput = new NumericUpDown
        {
            Value = (decimal)currentValue,
            Minimum = (decimal)min,
            Maximum = (decimal)max,
            Increment = 1,
            Width = 200
        };
        stack.Children.Add(numericInput);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 16,
            Margin = new Avalonia.Thickness(0, 12, 0, 0)
        };

        var okButton = new Button
        {
            Content = "OK",
            Padding = new Avalonia.Thickness(24, 8),
            Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)),
            Foreground = Brushes.White,
            BorderThickness = new Avalonia.Thickness(0)
        };
        okButton.Click += (s, args) =>
        {
            result = (double?)numericInput.Value;
            dialog.Close();
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            Padding = new Avalonia.Thickness(24, 8),
            Background = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
            Foreground = Brushes.White,
            BorderThickness = new Avalonia.Thickness(0)
        };
        cancelButton.Click += (s, args) => dialog.Close();

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);
        stack.Children.Add(buttonPanel);

        dialog.Content = stack;
        await dialog.ShowDialog(GetParentWindow());
        return result;
    }
}
