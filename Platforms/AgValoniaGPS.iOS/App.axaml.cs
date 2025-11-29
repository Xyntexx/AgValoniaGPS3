using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AgValoniaGPS.iOS.Views;
using AgValoniaGPS.iOS.DependencyInjection;
using AgValoniaGPS.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AgValoniaGPS.iOS;

public partial class App : Application
{
    private IServiceProvider? _services;

    public static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Build DI container
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddAgValoniaServices();
        _services = serviceCollection.BuildServiceProvider();
        Services = _services;

        // Load settings
        var settingsService = Services.GetRequiredService<ISettingsService>();
        settingsService.Load();

        if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
            DisableAvaloniaDataAnnotationValidation();

            singleViewPlatform.MainView = new MainView();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
