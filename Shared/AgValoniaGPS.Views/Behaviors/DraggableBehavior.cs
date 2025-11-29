using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace AgValoniaGPS.Views.Behaviors;

/// <summary>
/// Behavior that makes an element draggable within a Canvas parent.
/// Attach to any element to enable drag-to-reposition functionality.
/// The element must be a child of a Canvas for positioning to work.
/// </summary>
/// <example>
/// <![CDATA[
/// <Border>
///     <Interaction.Behaviors>
///         <behaviors:DraggableBehavior ConstrainToParent="True" />
///     </Interaction.Behaviors>
/// </Border>
/// ]]>
/// </example>
public class DraggableBehavior : Behavior<Control>
{
    private bool _isDragging;
    private Point _dragStartPoint;
    private double _originalLeft;
    private double _originalTop;

    /// <summary>
    /// When true, constrains the element to stay within the parent bounds.
    /// </summary>
    public static readonly StyledProperty<bool> ConstrainToParentProperty =
        AvaloniaProperty.Register<DraggableBehavior, bool>(nameof(ConstrainToParent), defaultValue: true);

    public bool ConstrainToParent
    {
        get => GetValue(ConstrainToParentProperty);
        set => SetValue(ConstrainToParentProperty, value);
    }

    /// <summary>
    /// Optional handle element name. If set, only this child element initiates dragging.
    /// If not set, the entire element is draggable.
    /// </summary>
    public static readonly StyledProperty<string?> DragHandleNameProperty =
        AvaloniaProperty.Register<DraggableBehavior, string?>(nameof(DragHandleName));

    public string? DragHandleName
    {
        get => GetValue(DragHandleNameProperty);
        set => SetValue(DragHandleNameProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
        {
            AssociatedObject.PointerPressed += OnPointerPressed;
            AssociatedObject.PointerMoved += OnPointerMoved;
            AssociatedObject.PointerReleased += OnPointerReleased;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject != null)
        {
            AssociatedObject.PointerPressed -= OnPointerPressed;
            AssociatedObject.PointerMoved -= OnPointerMoved;
            AssociatedObject.PointerReleased -= OnPointerReleased;
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject == null) return;

        // If a drag handle is specified, only start drag if the event originated from it
        if (!string.IsNullOrEmpty(DragHandleName))
        {
            var source = e.Source as Control;
            bool isFromHandle = false;

            while (source != null)
            {
                if (source.Name == DragHandleName)
                {
                    isFromHandle = true;
                    break;
                }
                source = source.Parent as Control;
            }

            if (!isFromHandle) return;
        }

        // Get the parent for coordinate reference
        var parent = AssociatedObject.Parent as Visual;
        if (parent == null) return;

        _isDragging = true;
        _dragStartPoint = e.GetPosition(parent);
        _originalLeft = Canvas.GetLeft(AssociatedObject);
        _originalTop = Canvas.GetTop(AssociatedObject);

        // Handle NaN values (element not yet positioned)
        if (double.IsNaN(_originalLeft)) _originalLeft = 0;
        if (double.IsNaN(_originalTop)) _originalTop = 0;

        e.Pointer.Capture(AssociatedObject);
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || AssociatedObject == null) return;

        var parent = AssociatedObject.Parent as Visual;
        if (parent == null) return;

        var currentPoint = e.GetPosition(parent);
        var deltaX = currentPoint.X - _dragStartPoint.X;
        var deltaY = currentPoint.Y - _dragStartPoint.Y;

        var newLeft = _originalLeft + deltaX;
        var newTop = _originalTop + deltaY;

        // Constrain to parent bounds if enabled
        if (ConstrainToParent && parent is Control parentControl)
        {
            var maxLeft = parentControl.Bounds.Width - AssociatedObject.Bounds.Width;
            var maxTop = parentControl.Bounds.Height - AssociatedObject.Bounds.Height;

            newLeft = Math.Clamp(newLeft, 0, Math.Max(0, maxLeft));
            newTop = Math.Clamp(newTop, 0, Math.Max(0, maxTop));
        }

        Canvas.SetLeft(AssociatedObject, newLeft);
        Canvas.SetTop(AssociatedObject, newTop);

        e.Handled = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
            e.Handled = true;
        }
    }
}
