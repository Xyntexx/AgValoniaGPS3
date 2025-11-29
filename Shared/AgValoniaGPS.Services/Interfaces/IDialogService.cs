using System.Threading.Tasks;
using AgValoniaGPS.Models;

namespace AgValoniaGPS.Services.Interfaces;

/// <summary>
/// Service interface for showing dialogs.
/// Platform-specific implementations create and display the actual dialog windows.
/// ViewModel calls these methods without knowledge of UI framework.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows a simple message box with OK button.
    /// </summary>
    Task ShowMessageAsync(string title, string message);

    /// <summary>
    /// Shows a confirmation dialog with Yes/No buttons.
    /// </summary>
    /// <returns>True if user clicked Yes, false otherwise.</returns>
    Task<bool> ShowConfirmationAsync(string title, string message);

    /// <summary>
    /// Shows the Data I/O configuration dialog.
    /// </summary>
    Task ShowDataIODialogAsync();

    /// <summary>
    /// Shows the simulator coordinates entry dialog.
    /// </summary>
    /// <param name="currentLatitude">Current latitude to pre-fill.</param>
    /// <param name="currentLongitude">Current longitude to pre-fill.</param>
    /// <returns>New coordinates if confirmed, null if cancelled.</returns>
    Task<(double Latitude, double Longitude)?> ShowSimCoordsDialogAsync(double currentLatitude, double currentLongitude);

    /// <summary>
    /// Shows the field selection dialog.
    /// </summary>
    /// <param name="fieldsDirectory">Root directory containing fields.</param>
    /// <returns>Selected field info if confirmed, null if cancelled.</returns>
    Task<DialogFieldSelectionResult?> ShowFieldSelectionDialogAsync(string fieldsDirectory);

    /// <summary>
    /// Shows the new field creation dialog.
    /// </summary>
    /// <param name="currentPosition">Current GPS position for default origin.</param>
    /// <returns>New field info if confirmed, null if cancelled.</returns>
    Task<DialogNewFieldResult?> ShowNewFieldDialogAsync(Position currentPosition);

    /// <summary>
    /// Shows the "from existing field" dialog.
    /// </summary>
    /// <param name="fieldsDirectory">Root directory containing fields.</param>
    /// <returns>Selected field and options if confirmed, null if cancelled.</returns>
    Task<DialogFromExistingFieldResult?> ShowFromExistingFieldDialogAsync(string fieldsDirectory);

    /// <summary>
    /// Shows the ISO-XML import dialog.
    /// </summary>
    /// <param name="fieldsDirectory">Root directory for fields.</param>
    /// <returns>Import result if confirmed, null if cancelled.</returns>
    Task<DialogIsoXmlImportResult?> ShowIsoXmlImportDialogAsync(string fieldsDirectory);

    /// <summary>
    /// Shows the KML import dialog.
    /// </summary>
    /// <param name="fieldsDirectory">Root directory for fields.</param>
    /// <returns>Import result if confirmed, null if cancelled.</returns>
    Task<DialogKmlImportResult?> ShowKmlImportDialogAsync(string fieldsDirectory);

    /// <summary>
    /// Shows the AgShare download dialog.
    /// </summary>
    /// <param name="apiKey">API key for AgShare.</param>
    /// <param name="fieldsDirectory">Root directory for fields.</param>
    /// <returns>Download result if confirmed, null if cancelled.</returns>
    Task<DialogAgShareDownloadResult?> ShowAgShareDownloadDialogAsync(string apiKey, string fieldsDirectory);

    /// <summary>
    /// Shows the AgShare upload dialog.
    /// </summary>
    /// <param name="apiKey">API key for AgShare.</param>
    /// <param name="fieldName">Name of field to upload.</param>
    /// <param name="fieldDirectory">Directory of field to upload.</param>
    /// <returns>True if upload succeeded, false otherwise.</returns>
    Task<bool> ShowAgShareUploadDialogAsync(string apiKey, string fieldName, string fieldDirectory);

    /// <summary>
    /// Shows the AgShare settings dialog.
    /// </summary>
    Task ShowAgShareSettingsDialogAsync();

    /// <summary>
    /// Shows the Mapsui boundary drawing dialog.
    /// </summary>
    /// <param name="centerLatitude">Center latitude for map.</param>
    /// <param name="centerLongitude">Center longitude for map.</param>
    /// <returns>Boundary points if confirmed, null if cancelled.</returns>
    Task<DialogMapBoundaryResult?> ShowMapBoundaryDialogAsync(double centerLatitude, double centerLongitude);

    /// <summary>
    /// Shows a numeric input dialog.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="prompt">Input prompt.</param>
    /// <param name="currentValue">Current value to pre-fill.</param>
    /// <param name="min">Minimum allowed value.</param>
    /// <param name="max">Maximum allowed value.</param>
    /// <returns>Entered value if confirmed, null if cancelled.</returns>
    Task<double?> ShowNumericInputDialogAsync(string title, string prompt, double currentValue, double min, double max);
}

#region Dialog Result Types

/// <summary>
/// Result from field selection dialog.
/// </summary>
public class DialogFieldSelectionResult
{
    public required string FieldName { get; init; }
    public required string DirectoryPath { get; init; }
    public Boundary? Boundary { get; init; }
}

/// <summary>
/// Result from new field dialog.
/// </summary>
public class DialogNewFieldResult
{
    public required string FieldName { get; init; }
    public required Position Origin { get; init; }
}

/// <summary>
/// Result from "from existing field" dialog.
/// </summary>
public class DialogFromExistingFieldResult
{
    public required string SourceFieldPath { get; init; }
    public required string NewFieldName { get; init; }
    public bool CopyFlags { get; init; }
    public bool CopyMapping { get; init; }
    public bool CopyHeadland { get; init; }
    public bool CopyLines { get; init; }
}

/// <summary>
/// Result from ISO-XML import dialog.
/// </summary>
public class DialogIsoXmlImportResult
{
    public required string FieldName { get; init; }
    public required string FieldDirectory { get; init; }
    public Boundary? ImportedBoundary { get; init; }
}

/// <summary>
/// Result from KML import dialog.
/// </summary>
public class DialogKmlImportResult
{
    public required string FieldName { get; init; }
    public required string FieldDirectory { get; init; }
    public Boundary? ImportedBoundary { get; init; }
}

/// <summary>
/// Result from AgShare download dialog.
/// </summary>
public class DialogAgShareDownloadResult
{
    public required string FieldName { get; init; }
    public required string FieldDirectory { get; init; }
}

/// <summary>
/// Result from map boundary drawing dialog.
/// </summary>
public class DialogMapBoundaryResult
{
    public required System.Collections.Generic.List<(double Latitude, double Longitude)> Points { get; init; }
}

#endregion
