using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui.UI.Avalonia;
using NetTopologySuite.Geometries;
using NtsPoint = NetTopologySuite.Geometries.Point;

namespace AgValoniaGPS.Views.Controls.Dialogs;

public partial class BoundaryMapDialogPanel : UserControl
{
    private WritableLayer? _pointsLayer;
    private WritableLayer? _polygonLayer;
    private bool _isDrawingMode;
    private bool _mapInitialized;
    private readonly List<(double Lat, double Lon)> _boundaryPoints = new();

    public BoundaryMapDialogPanel()
    {
        InitializeComponent();

        // Initialize map when the control becomes visible
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(IsVisible) && IsVisible && !_mapInitialized)
        {
            SetupMap();
            _mapInitialized = true;
        }
    }

    private void SetupMap()
    {
        var map = new Mapsui.Map();

        // Add Esri World Imagery (satellite) as base layer - free, no API key required
        var esriSatelliteUrl = "https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}";
        var esriTileSource = new HttpTileSource(
            new GlobalSphericalMercator(),
            esriSatelliteUrl,
            name: "Esri World Imagery");
        map.Layers.Add(new TileLayer(esriTileSource) { Name = "Satellite" });

        // Create layer for polygon (drawn below points)
        _polygonLayer = new WritableLayer
        {
            Name = "Polygon",
            Style = new VectorStyle
            {
                Fill = new Mapsui.Styles.Brush(new Mapsui.Styles.Color(52, 152, 219, 50)), // Semi-transparent blue
                Line = new Mapsui.Styles.Pen(new Mapsui.Styles.Color(255, 255, 255, 255), 3) // White outline
            }
        };
        map.Layers.Add(_polygonLayer);

        // Create layer for boundary points
        _pointsLayer = new WritableLayer
        {
            Name = "Points",
            Style = new SymbolStyle
            {
                Fill = new Mapsui.Styles.Brush(new Mapsui.Styles.Color(231, 76, 60, 255)), // Red
                Outline = new Mapsui.Styles.Pen(new Mapsui.Styles.Color(192, 57, 43, 255), 2),
                SymbolScale = 0.5
            }
        };
        map.Layers.Add(_pointsLayer);

        // Get initial position from ViewModel
        double lat = 39.8283; // Default to US center
        double lon = -98.5795;

        if (DataContext is AgValoniaGPS.ViewModels.MainViewModel vm)
        {
            if (Math.Abs(vm.BoundaryMapCenterLatitude) > 0.01 || Math.Abs(vm.BoundaryMapCenterLongitude) > 0.01)
            {
                lat = vm.BoundaryMapCenterLatitude;
                lon = vm.BoundaryMapCenterLongitude;
            }
        }

        // Convert to SphericalMercator
        var center = SphericalMercator.FromLonLat(lon, lat);
        map.Navigator.CenterOnAndZoomTo(new MPoint(center.x, center.y), map.Navigator.Resolutions[16]);

        MapControl.Map = map;

        // Disable all debug/performance overlays and widgets
        map.Widgets.Clear();

        // Handle map clicks via pointer events
        MapControl.PointerPressed += OnMapPointerPressed;

        // Handle pointer movement for coordinate display
        MapControl.PointerMoved += OnPointerMoved;
    }

    private void OnMapPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!_isDrawingMode)
            return;

        var point = e.GetCurrentPoint(MapControl);
        if (point.Properties.IsLeftButtonPressed)
        {
            var viewport = MapControl.Map.Navigator.Viewport;
            var worldPos = viewport.ScreenToWorldXY(point.Position.X, point.Position.Y);

            // Convert from SphericalMercator to WGS84
            var lonLat = SphericalMercator.ToLonLat(worldPos.worldX, worldPos.worldY);

            AddBoundaryPoint(lonLat.lat, lonLat.lon);

            e.Handled = true;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var position = e.GetPosition(MapControl);
        var viewport = MapControl.Map.Navigator.Viewport;
        var worldPos = viewport.ScreenToWorldXY(position.X, position.Y);

        // Convert from SphericalMercator to WGS84
        var lonLat = SphericalMercator.ToLonLat(worldPos.worldX, worldPos.worldY);

        if (DataContext is AgValoniaGPS.ViewModels.MainViewModel vm)
        {
            vm.BoundaryMapCoordinateText = $"Lat: {lonLat.lat:F6}, Lon: {lonLat.lon:F6}";
        }
    }

    private void AddBoundaryPoint(double lat, double lon)
    {
        _boundaryPoints.Add((lat, lon));

        // Add point marker
        var mercator = SphericalMercator.FromLonLat(lon, lat);
        var point = new GeometryFeature(new NtsPoint(mercator.x, mercator.y));
        _pointsLayer?.Add(point);

        UpdatePolygon();
        UpdateUI();

        MapControl.Refresh();
    }

    private void UpdatePolygon()
    {
        _polygonLayer?.Clear();

        if (_boundaryPoints.Count >= 3)
        {
            // Create polygon from points
            var coordinates = new List<Coordinate>();
            foreach (var (lat, lon) in _boundaryPoints)
            {
                var mercator = SphericalMercator.FromLonLat(lon, lat);
                coordinates.Add(new Coordinate(mercator.x, mercator.y));
            }
            // Close the polygon
            var first = _boundaryPoints[0];
            var firstMercator = SphericalMercator.FromLonLat(first.Lon, first.Lat);
            coordinates.Add(new Coordinate(firstMercator.x, firstMercator.y));

            var ring = new LinearRing(coordinates.ToArray());
            var polygon = new Polygon(ring);
            var feature = new GeometryFeature(polygon);
            _polygonLayer?.Add(feature);
        }
        else if (_boundaryPoints.Count >= 2)
        {
            // Draw line between points
            var coordinates = new List<Coordinate>();
            foreach (var (lat, lon) in _boundaryPoints)
            {
                var mercator = SphericalMercator.FromLonLat(lon, lat);
                coordinates.Add(new Coordinate(mercator.x, mercator.y));
            }
            var line = new LineString(coordinates.ToArray());
            var feature = new GeometryFeature(line);
            _polygonLayer?.Add(feature);
        }
    }

    private void UpdateUI()
    {
        var count = _boundaryPoints.Count;

        if (DataContext is AgValoniaGPS.ViewModels.MainViewModel vm)
        {
            vm.BoundaryMapPointCount = count;
            vm.BoundaryMapCanSave = count >= 3;
        }

        BtnUndo.IsEnabled = count > 0;
        BtnClear.IsEnabled = count > 0;
    }

    private void BtnDraw_Click(object? sender, RoutedEventArgs e)
    {
        _isDrawingMode = !_isDrawingMode;

        if (_isDrawingMode)
        {
            BtnDraw.Classes.Add("active");
            BtnDrawText.Text = "Stop";
            MapControl.Cursor = new Cursor(StandardCursorType.Cross);
        }
        else
        {
            BtnDraw.Classes.Remove("active");
            BtnDrawText.Text = "Draw";
            MapControl.Cursor = Cursor.Default;
        }
    }

    private void BtnUndo_Click(object? sender, RoutedEventArgs e)
    {
        if (_boundaryPoints.Count == 0)
            return;

        _boundaryPoints.RemoveAt(_boundaryPoints.Count - 1);

        // Rebuild points layer
        _pointsLayer?.Clear();
        foreach (var (lat, lon) in _boundaryPoints)
        {
            var mercator = SphericalMercator.FromLonLat(lon, lat);
            var point = new GeometryFeature(new NtsPoint(mercator.x, mercator.y));
            _pointsLayer?.Add(point);
        }

        UpdatePolygon();
        UpdateUI();
        MapControl.Refresh();
    }

    private void BtnClear_Click(object? sender, RoutedEventArgs e)
    {
        _boundaryPoints.Clear();
        _pointsLayer?.Clear();
        _polygonLayer?.Clear();
        UpdateUI();
        MapControl.Refresh();
    }

    private async void BtnSave_Click(object? sender, RoutedEventArgs e)
    {
        if (_boundaryPoints.Count < 3)
            return;

        if (DataContext is not AgValoniaGPS.ViewModels.MainViewModel vm)
            return;

        var includeBackground = vm.BoundaryMapIncludeBackground;
        string? backgroundPath = null;
        double nwLat = 0, nwLon = 0, seLat = 0, seLon = 0;

        // Capture background if requested
        if (includeBackground)
        {
            BtnSave.IsEnabled = false;

            try
            {
                var result = await CaptureBackgroundImageAsync();
                if (result != null)
                {
                    backgroundPath = result.Value.Path;
                    nwLat = result.Value.NwLat;
                    nwLon = result.Value.NwLon;
                    seLat = result.Value.SeLat;
                    seLon = result.Value.SeLon;
                }
            }
            finally
            {
                BtnSave.IsEnabled = true;
            }
        }

        // Copy boundary points to ViewModel
        vm.BoundaryMapResultPoints.Clear();
        foreach (var (lat, lon) in _boundaryPoints)
        {
            vm.BoundaryMapResultPoints.Add((lat, lon));
        }

        vm.BoundaryMapResultBackgroundPath = backgroundPath;
        vm.BoundaryMapResultNwLat = nwLat;
        vm.BoundaryMapResultNwLon = nwLon;
        vm.BoundaryMapResultSeLat = seLat;
        vm.BoundaryMapResultSeLon = seLon;

        // Execute confirm command
        vm.ConfirmBoundaryMapDialogCommand?.Execute(null);

        // Reset state for next use
        ResetState();
    }

    private async Task<(string Path, double NwLat, double NwLon, double SeLat, double SeLon)?> CaptureBackgroundImageAsync()
    {
        try
        {
            // Get current viewport bounds
            var viewport = MapControl.Map.Navigator.Viewport;

            // Get the extent from viewport
            var worldMin = viewport.ScreenToWorldXY(0, viewport.Height);
            var worldMax = viewport.ScreenToWorldXY(viewport.Width, 0);

            // Convert extent corners to WGS84
            var nw = SphericalMercator.ToLonLat(worldMin.worldX, worldMax.worldY);
            var se = SphericalMercator.ToLonLat(worldMax.worldX, worldMin.worldY);

            // Export the map to a bitmap
            var tempDir = Path.Combine(Path.GetTempPath(), "AgValoniaGPS_Mapsui");
            Directory.CreateDirectory(tempDir);
            var savedBackgroundPath = Path.Combine(tempDir, "BackPic.png");

            // Hide drawing layers before capture
            if (_pointsLayer != null) _pointsLayer.Enabled = false;
            if (_polygonLayer != null) _polygonLayer.Enabled = false;
            MapControl.Refresh();

            // Small delay to ensure the map redraws without the layers
            await Task.Delay(100);

            // Get the size of the MapControl
            var bounds = MapControl.Bounds;
            var pixelSize = new PixelSize((int)bounds.Width, (int)bounds.Height);

            if (pixelSize.Width > 0 && pixelSize.Height > 0)
            {
                // Create a RenderTargetBitmap to capture the control
                var renderTarget = new RenderTargetBitmap(pixelSize);
                renderTarget.Render(MapControl);

                // Save the bitmap to a file
                renderTarget.Save(savedBackgroundPath);

                Console.WriteLine($"Background image saved to: {savedBackgroundPath}");
            }

            // Re-enable drawing layers
            if (_pointsLayer != null) _pointsLayer.Enabled = true;
            if (_polygonLayer != null) _polygonLayer.Enabled = true;
            MapControl.Refresh();

            // Create geo-reference file content
            var geoPath = Path.Combine(tempDir, "BackPic.txt");
            var geoContent = $"$BackPic\ntrue\n{nw.lat.ToString(CultureInfo.InvariantCulture)}\n{nw.lon.ToString(CultureInfo.InvariantCulture)}\n{se.lat.ToString(CultureInfo.InvariantCulture)}\n{se.lon.ToString(CultureInfo.InvariantCulture)}";
            File.WriteAllText(geoPath, geoContent);

            return (savedBackgroundPath, nw.lat, nw.lon, se.lat, se.lon);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error capturing background: {ex.Message}");

            // Re-enable drawing layers on error
            if (_pointsLayer != null) _pointsLayer.Enabled = true;
            if (_polygonLayer != null) _polygonLayer.Enabled = true;
            MapControl.Refresh();

            return null;
        }
    }

    private void ResetState()
    {
        _boundaryPoints.Clear();
        _pointsLayer?.Clear();
        _polygonLayer?.Clear();
        _isDrawingMode = false;
        BtnDraw.Classes.Remove("active");
        BtnDrawText.Text = "Draw";
        MapControl.Cursor = Cursor.Default;
        BtnUndo.IsEnabled = false;
        BtnClear.IsEnabled = false;
    }

    private void Backdrop_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is AgValoniaGPS.ViewModels.MainViewModel vm)
        {
            vm.CancelBoundaryMapDialogCommand?.Execute(null);
            ResetState();
        }
        e.Handled = true;
    }
}
