using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Threading;
using Griddle.Core.Geometry;
using Griddle.Core.History;
using Griddle.Core.Models;
using Griddle.Core.Services;
using Griddle.Core.Tools;

namespace Griddle.App.Controls;

public sealed class DrawingCanvas : Control
{
    private readonly List<Stroke> _strokes = new();
    private readonly Stack<IHistoryAction> _undoStack = new();
    private readonly Stack<IHistoryAction> _redoStack = new();

    private readonly PenTool _pen;
    private readonly ActiveToolService _activeTool;
    private readonly SelectionService _selection;

    private bool _isToolInteractionActive;

    private Stroke? _activeStroke;
    private Stroke? _draggingStroke;
    private Stroke? _editingTextStroke;
    private bool _isEditingText;
    private readonly DispatcherTimer _caretTimer;
    private bool _isCaretVisible;
    private Guid _activeCalloutGroupId = Guid.NewGuid();
    private Point? _lastPointerPosition;
    private Point2D? _resizeAnchorPoint;
    private Point2D? _resizeBeforeStart;
    private Point2D? _resizeBeforeEnd;
    private Point2D? _arrowBeforeStart;
    private Point2D? _arrowBeforeEnd;

    private ResizeHandle _activeResizeHandle = ResizeHandle.None;
    private ArrowHandle _activeArrowHandle = ArrowHandle.None;

    private double _dragDeltaX;
    private double _dragDeltaY;

    public PenTool Pen => _pen;
    public ActiveToolService ActiveTool => _activeTool;
    public SelectionService Selection => _selection;
    public bool IsEditingText =>
        _isEditingText &&
        _editingTextStroke is not null;

    public DrawingCanvas()
        : this(
            new PenTool(new PenSettings()),
            null,
            null)
    {
    }

    public DrawingCanvas(
        PenTool pen,
        ActiveToolService? activeTool,
        SelectionService? selection)
    {
        _pen = pen;
        _activeTool = activeTool ?? new ActiveToolService(pen);
        _selection = selection ?? new SelectionService();
        _caretTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };

        _caretTimer.Tick += (_, _) =>
        {
            if (!IsEditingText)
            {
                return;
            }

            _isCaretVisible = !_isCaretVisible;
            InvalidateVisual();
        };
    }

    public void SetColor(StrokeColor color)
    {
        Pen.Settings.Color = color;
    }

    public void SetThickness(double thickness)
    {
        Pen.Settings.Thickness = thickness;
    }

    private void UpdateCursor(Point point)
    {
        var selectedStroke = _selection.SelectedStroke;

        if (selectedStroke is null)
        {
            Cursor = Cursor.Default;
            return;
        }

        switch (selectedStroke.Kind)
        {
            case StrokeKind.Rectangle:

                var resizeHandle = HitTestResizeHandle(
                    GetRectangleBounds(selectedStroke),
                    point);

                Cursor = resizeHandle switch
                {
                    ResizeHandle.TopLeft =>
                        new Cursor(StandardCursorType.TopLeftCorner),

                    ResizeHandle.BottomRight =>
                        new Cursor(StandardCursorType.BottomRightCorner),

                    ResizeHandle.TopRight =>
                        new Cursor(StandardCursorType.TopRightCorner),

                    ResizeHandle.BottomLeft =>
                        new Cursor(StandardCursorType.BottomLeftCorner),

                    _ =>
                        Cursor.Default
                };

                break;

            case StrokeKind.Arrow:
            case StrokeKind.Callout:

                var arrowHandle =
                    HitTestArrowHandle(
                        selectedStroke,
                        point);

                Cursor = arrowHandle switch
                {
                    ArrowHandle.Start =>
                        new Cursor(StandardCursorType.Cross),

                    ArrowHandle.End =>
                        new Cursor(StandardCursorType.Cross),

                    _ =>
                        Cursor.Default
                };

                break;

            default:

                Cursor = Cursor.Default;
                break;
        }
    }

    public void AppendText(string text)
    {
        if (!IsEditingText)
        {
            return;
        }

        _isCaretVisible = true;
        _caretTimer.Stop();
        _caretTimer.Start();

        _editingTextStroke!.Text += text;

        InvalidateVisual();
    }

    public void BackspaceText()
    {
        if (!IsEditingText ||
            string.IsNullOrEmpty(_editingTextStroke!.Text))
        {
            return;
        }

        _isCaretVisible = true;
        _caretTimer.Stop();
        _caretTimer.Start();

        _editingTextStroke.Text =
            _editingTextStroke.Text[..^1];

        InvalidateVisual();
    }

    public void CommitText()
    {
        if (!IsEditingText)
        {
            return;
        }

        var stroke = _editingTextStroke!;

        if (string.IsNullOrWhiteSpace(stroke.Text))
        {
            _strokes.Remove(stroke);
        }
        else
        {
            var index = _strokes.IndexOf(stroke);

            _undoStack.Push(
                new AddStrokeAction(
                    _strokes,
                    stroke,
                    index));

            _redoStack.Clear();
        }

        _editingTextStroke = null;
        _isEditingText = false;
        _isCaretVisible = false;
        _caretTimer.Stop();

        InvalidateVisual();
    }

    public void CancelText()
    {
        if (!IsEditingText)
        {
            return;
        }

        _strokes.Remove(
            _editingTextStroke!);

        _editingTextStroke = null;
        _isEditingText = false;

        InvalidateVisual();
    }

    public void BeginInteraction(
        Point point,
        int clickCount = 1)
    {
        if (_activeTool.Current is TextTool)
        {
            var textStroke =
                _activeTool.Current.Begin(
                    ToPoint2D(point));

            if (textStroke is not null)
            {
                _strokes.Add(textStroke);

                _undoStack.Push(
                    new AddStrokeAction(
                        _strokes,
                        textStroke,
                        _strokes.Count - 1));

                _redoStack.Clear();

                _editingTextStroke = textStroke;
                _isEditingText = true;

                StartCaret();

                _selection.Clear();
                ResetDragState();
            }

            _activeStroke = null;
            _isToolInteractionActive = false;

            InvalidateVisual();
            return;
        }

        if (_activeTool.Current is SelectionTool)
        {
            var hit = HitTest(point);

            if (clickCount == 2 &&
                hit is not null &&
                (hit.Kind == StrokeKind.Text ||
                 hit.Kind == StrokeKind.Callout))
            {
                _editingTextStroke = hit;
                _isEditingText = true;

                StartCaret();

                InvalidateVisual();
                return;
            }

            if (hit is null)
            {
                _selection.Clear();
                ResetDragState();
            }
            else
            {
                _selection.Select(hit);

                _activeResizeHandle = ResizeHandle.None;
                _activeArrowHandle = ArrowHandle.None;

                _resizeAnchorPoint = null;
                _draggingStroke = hit;
                _lastPointerPosition = null;
                _dragDeltaX = 0;
                _dragDeltaY = 0;

                if (hit.Kind == StrokeKind.Rectangle)
                {
                    var bounds = GetRectangleBounds(hit);

                    _activeResizeHandle =
                        HitTestResizeHandle(
                            bounds,
                            point);

                    if (_activeResizeHandle != ResizeHandle.None)
                    {
                        _resizeBeforeStart = hit.Points[0];
                        _resizeBeforeEnd = hit.Points[1];
                    }

                    switch (_activeResizeHandle)
                    {
                        case ResizeHandle.BottomRight:
                            _resizeAnchorPoint = new Point2D(
                                bounds.Left,
                                bounds.Top);
                            break;

                        case ResizeHandle.BottomLeft:
                            _resizeAnchorPoint = new Point2D(
                                bounds.Right,
                                bounds.Top);
                            break;

                        case ResizeHandle.TopRight:
                            _resizeAnchorPoint = new Point2D(
                                bounds.Left,
                                bounds.Bottom);
                            break;

                        case ResizeHandle.TopLeft:
                            _resizeAnchorPoint = new Point2D(
                                bounds.Right,
                                bounds.Bottom);
                            break;
                    }
                }
                else if (hit.Kind == StrokeKind.Arrow ||
                         hit.Kind == StrokeKind.Callout)
                {
                    _activeArrowHandle =
                        HitTestArrowHandle(
                            hit,
                            point);

                    if (_activeArrowHandle != ArrowHandle.None)
                    {
                        _arrowBeforeStart = hit.Points[0];
                        _arrowBeforeEnd = hit.Points[1];
                    }
                }

                if (_activeResizeHandle == ResizeHandle.None &&
                    _activeArrowHandle == ArrowHandle.None)
                {
                    _lastPointerPosition = point;
                }
            }

            InvalidateVisual();
            return;
        }

        _isToolInteractionActive = true;

        _activeStroke = _activeTool.Current.Begin(
            ToPoint2D(point));

        InvalidateVisual();
    }

    public void ContinueInteraction(Point point)
    {
        UpdateCursor(point);

        if (_activeTool.Current is SelectionTool)
        {
            if (_activeArrowHandle != ArrowHandle.None)
            {
                MoveArrowEndpoint(point);
                return;
            }

            if (_activeResizeHandle != ResizeHandle.None)
            {
                ResizeRectangle(point);
                return;
            }

            if (_draggingStroke is null ||
                _lastPointerPosition is null)
            {
                return;
            }

            var deltaX =
                point.X - _lastPointerPosition.Value.X;

            var deltaY =
                point.Y - _lastPointerPosition.Value.Y;

            _draggingStroke.Translate(
                deltaX,
                deltaY);

            _dragDeltaX += deltaX;
            _dragDeltaY += deltaY;
            _lastPointerPosition = point;

            InvalidateVisual();
            return;
        }

        if (!_isToolInteractionActive)
        {
            return;
        }

        _activeTool.Current.Continue(
            ToPoint2D(point));

        InvalidateVisual();
    }

    public void EndInteraction(Point point)
    {
        if (_activeTool.Current is SelectionTool)
        {
            if (_activeArrowHandle != ArrowHandle.None &&
                _draggingStroke is not null &&
                _arrowBeforeStart is not null &&
                _arrowBeforeEnd is not null)
            {
                var afterStart =
                    _draggingStroke.Points[0];

                var afterEnd =
                    _draggingStroke.Points[1];

                if (afterStart != _arrowBeforeStart.Value ||
                    afterEnd != _arrowBeforeEnd.Value)
                {
                    _undoStack.Push(
                        new ArrowEndpointMoveAction(
                            _draggingStroke,
                            _arrowBeforeStart.Value,
                            _arrowBeforeEnd.Value,
                            afterStart,
                            afterEnd));

                    _redoStack.Clear();
                }
            }
            else if (_activeResizeHandle != ResizeHandle.None)
            {
                // keep your existing rectangle resize history block here
            }
            else if (_draggingStroke is not null &&
                     (_dragDeltaX != 0 || _dragDeltaY != 0))
            {
                // keep your existing move history block here
            }

            if (_activeResizeHandle != ResizeHandle.None &&
                _draggingStroke is not null &&
                _resizeBeforeStart is not null &&
                _resizeBeforeEnd is not null)
            {
                var afterStart = _draggingStroke.Points[0];
                var afterEnd = _draggingStroke.Points[1];

                if (afterStart != _resizeBeforeStart.Value ||
                    afterEnd != _resizeBeforeEnd.Value)
                {
                    _undoStack.Push(
                        new ResizeStrokeAction(
                            _draggingStroke,
                            _resizeBeforeStart.Value,
                            _resizeBeforeEnd.Value,
                            afterStart,
                            afterEnd));

                    _redoStack.Clear();
                }
            }
            else if (_draggingStroke is not null &&
                     (_dragDeltaX != 0 || _dragDeltaY != 0))
            {
                _undoStack.Push(
                    new MoveStrokeAction(
                        _draggingStroke,
                        _dragDeltaX,
                        _dragDeltaY));

                _redoStack.Clear();
            }

            ResetDragState();

            InvalidateVisual();
            return;
        }

        if (!_isToolInteractionActive)
        {
            return;
        }

        var completedStroke = _activeTool.Current.End(
            ToPoint2D(point));

        if (completedStroke is not null)
        {
            _strokes.Add(completedStroke);

            if (completedStroke.Kind == StrokeKind.Callout)
            {
                completedStroke.CalloutGroupId =
                    _activeCalloutGroupId;

                completedStroke.CalloutNumber =
                    GetNextCalloutNumber(
                        _activeCalloutGroupId);

                _editingTextStroke = completedStroke;
                _isEditingText = true;
                _isCaretVisible = true;

                _caretTimer.Stop();
                _caretTimer.Start();
            }

            _undoStack.Push(
                new AddStrokeAction(
                    _strokes,
                    completedStroke,
                    _strokes.Count - 1));

            _redoStack.Clear();

            InvalidateVisual();
        }

        _activeStroke = null;
        _isToolInteractionActive = false;

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        foreach (var stroke in _strokes)
        {
            DrawStroke(context, stroke);
        }

        if (_activeStroke is not null)
        {
            DrawStroke(context, _activeStroke);
        }

        if (IsEditingText &&
            _isCaretVisible &&
            _editingTextStroke is not null)
        {
            DrawTextCaret(
                context,
                _editingTextStroke);
        }

        if (_selection.SelectedStroke is not null)
        {
            DrawSelectionOutline(
                context,
                _selection.SelectedStroke);
        }
    }

    private static void DrawStroke(
        DrawingContext context,
        Stroke stroke)
    {
        switch (stroke.Kind)
        {
            case StrokeKind.Freehand:
                DrawFreehand(context, stroke);
                break;

            case StrokeKind.Arrow:
                DrawArrow(context, stroke);
                break;

            case StrokeKind.Rectangle:
                DrawRectangle(context, stroke);
                break;

            case StrokeKind.Text:
                DrawText(context, stroke);
                break;

            case StrokeKind.Callout:
                DrawCallout(context, stroke);
                break;

            default:
                throw new NotSupportedException(
                    $"Unsupported stroke kind: {stroke.Kind}");
        }
    }

    private static void DrawFreehand(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var pen = CreatePen(stroke);
        var geometry = new StreamGeometry();

        using (var geometryContext = geometry.Open())
        {
            geometryContext.BeginFigure(
                ToAvaloniaPoint(stroke.Points[0]),
                isFilled: false);

            for (
                var index = 1;
                index < stroke.Points.Count;
                index++)
            {
                geometryContext.LineTo(
                    ToAvaloniaPoint(
                        stroke.Points[index]));
            }

            geometryContext.EndFigure(
                isClosed: false);
        }

        context.DrawGeometry(
            brush: null,
            pen,
            geometry);
    }

    private static void DrawArrow(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start =
            ToAvaloniaPoint(stroke.Points[0]);

        var end =
            ToAvaloniaPoint(stroke.Points[1]);

        var pen = CreatePen(stroke);

        context.DrawLine(
            pen,
            start,
            end);

        DrawArrowHead(
            context,
            pen,
            start,
            end);
    }

    private static void DrawArrowHead(
        DrawingContext context,
        Pen pen,
        Point start,
        Point end)
    {
        var direction = start - end;

        var length = Math.Sqrt(
            direction.X * direction.X +
            direction.Y * direction.Y);

        if (length < 1)
        {
            return;
        }

        var unitX = direction.X / length;
        var unitY = direction.Y / length;

        const double arrowHeadLength = 18;
        const double arrowHeadWidth = 8;

        var perpendicularX = -unitY;
        var perpendicularY = unitX;

        var left = new Point(
            end.X +
            unitX * arrowHeadLength +
            perpendicularX * arrowHeadWidth,
            end.Y +
            unitY * arrowHeadLength +
            perpendicularY * arrowHeadWidth);

        var right = new Point(
            end.X +
            unitX * arrowHeadLength -
            perpendicularX * arrowHeadWidth,
            end.Y +
            unitY * arrowHeadLength -
            perpendicularY * arrowHeadWidth);

        context.DrawLine(
            pen,
            end,
            left);

        context.DrawLine(
            pen,
            end,
            right);
    }

    private static void DrawCallout(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start =
            ToAvaloniaPoint(stroke.Points[0]);

        var end =
            ToAvaloniaPoint(stroke.Points[1]);

        var pen = CreatePen(stroke);

        context.DrawLine(
            pen,
            start,
            end);

        DrawArrowHead(
            context,
            pen,
            start,
            end);

        if (!string.IsNullOrEmpty(stroke.Text))
        {
            var text = new FormattedText(
                stroke.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                20,
                Brushes.White);

            var textPosition = new Point(
                end.X + 12,
                end.Y - text.Height / 2);

            context.DrawText(
                text,
                textPosition);
        }

        const double radius = 12;

        context.DrawEllipse(
            Brushes.White,
            pen,
            start,
            radius,
            radius);

        var number = new FormattedText(
            (stroke.CalloutNumber?.ToString() ?? "•"),
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            14,
            Brushes.Black);

        var numberPosition = new Point(
            start.X - number.Width / 2,
            start.Y - number.Height / 2);

        context.DrawText(
            number,
            numberPosition);
    }

    private int GetNextCalloutNumber(Guid groupId)
    {
        var usedNumbers =
            _strokes
                .Where(stroke =>
                    stroke.Kind == StrokeKind.Callout &&
                    stroke.CalloutGroupId == groupId &&
                    stroke.CalloutNumber.HasValue)
                .Select(stroke => stroke.CalloutNumber!.Value)
                .ToHashSet();

        var number = 1;

        while (usedNumbers.Contains(number))
        {
            number++;
        }

        return number;
    }

    private static void DrawRectangle(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start =
            ToAvaloniaPoint(stroke.Points[0]);

        var end =
            ToAvaloniaPoint(stroke.Points[1]);

        var x = Math.Min(
            start.X,
            end.X);

        var y = Math.Min(
            start.Y,
            end.Y);

        var width = Math.Abs(
            end.X - start.X);

        var height = Math.Abs(
            end.Y - start.Y);

        var rectangle = new Rect(
            x,
            y,
            width,
            height);

        context.DrawRectangle(
            brush: null,
            CreatePen(stroke),
            rectangle);
    }

    private static void DrawText(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count == 0 ||
            string.IsNullOrEmpty(stroke.Text))
        {
            return;
        }

        var position =
            ToAvaloniaPoint(stroke.Points[0]);

        var baseColor = stroke.Color switch
        {
            StrokeColor.Blue => Colors.DodgerBlue,
            StrokeColor.Black => Colors.Black,
            StrokeColor.Yellow => Colors.Yellow,
            _ => Colors.Red
        };

        var formattedText = new FormattedText(
            stroke.Text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            24,
            new SolidColorBrush(baseColor));

        context.DrawText(
            formattedText,
            position);
    }

    private static void DrawTextCaret(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count == 0)
        {
            return;
        }

        var isCallout =
            stroke.Kind == StrokeKind.Callout &&
            stroke.Points.Count > 1;

        var fontSize =
            isCallout ? 20.0 : 24.0;

        var position =
            isCallout
                ? new Point(
                    stroke.Points[1].X + 12,
                    stroke.Points[1].Y)
                : ToAvaloniaPoint(stroke.Points[0]);

        var formattedText = new FormattedText(
            stroke.Text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            fontSize,
            Brushes.White);

        var textWidth =
            formattedText.WidthIncludingTrailingWhitespace;

        var caretHeight =
            formattedText.Height > 0
                ? formattedText.Height
                : fontSize;

        var caretX =
            position.X + textWidth;

        var caretTop =
            isCallout
                ? position.Y - caretHeight / 2
                : position.Y;

        context.DrawLine(
            new Pen(
                Brushes.White,
                2),
            new Point(
                caretX,
                caretTop),
            new Point(
                caretX,
                caretTop + caretHeight));
    }

    private void StartCaret()
    {
        _isCaretVisible = true;

        _caretTimer.Stop();
        _caretTimer.Start();

        InvalidateVisual();
    }

    private static void DrawSelectionOutline(
        DrawingContext context,
        Stroke stroke)
    {
        switch (stroke.Kind)
        {
            case StrokeKind.Rectangle:
                DrawRectangleSelectionOutline(
                    context,
                    stroke);
                break;

            case StrokeKind.Arrow:
                DrawArrowSelectionOutline(
                    context,
                    stroke);
                break;

            case StrokeKind.Callout:
                DrawCalloutSelectionOutline(
                    context,
                    stroke);
                break;
        }
    }

    private static void DrawRectangleSelectionOutline(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start =
            ToAvaloniaPoint(stroke.Points[0]);

        var end =
            ToAvaloniaPoint(stroke.Points[1]);

        var left = Math.Min(
            start.X,
            end.X);

        var right = Math.Max(
            start.X,
            end.X);

        var top = Math.Min(
            start.Y,
            end.Y);

        var bottom = Math.Max(
            start.Y,
            end.Y);

        var bounds = new Rect(
            left - 4,
            top - 4,
            (right - left) + 8,
            (bottom - top) + 8);

        var pen = CreateSelectionPen();

        context.DrawRectangle(
            null,
            pen,
            bounds);

        DrawResizeHandle(
            context,
            new Point(left, top));

        DrawResizeHandle(
            context,
            new Point(right, top));

        DrawResizeHandle(
            context,
            new Point(left, bottom));

        DrawResizeHandle(
            context,
            new Point(right, bottom));
    }

    private static void DrawCalloutSelectionOutline(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start =
            ToAvaloniaPoint(stroke.Points[0]);

        var end =
            ToAvaloniaPoint(stroke.Points[1]);

        var selectionPen =
            new Pen(
                Brushes.White,
                2,
                dashStyle: new DashStyle(
                    new double[] { 4, 4 },
                    0));

        context.DrawLine(
            selectionPen,
            start,
            end);

        const double handleRadius = 6;

        context.DrawEllipse(
            Brushes.White,
            null,
            start,
            handleRadius,
            handleRadius);

        context.DrawEllipse(
            Brushes.White,
            null,
            end,
            handleRadius,
            handleRadius);
    }

    private static void DrawResizeHandle(
        DrawingContext context,
        Point center)
    {
        const double size = 8;

        var rect = new Rect(
            center.X - size / 2,
            center.Y - size / 2,
            size,
            size);

        context.DrawRectangle(
            Brushes.White,
            new Pen(
                Brushes.Black,
                1),
            rect);
    }

    private static ResizeHandle HitTestResizeHandle(
        Rect bounds,
        Point point)
    {
        const double size = 8;

        if (new Rect(
                bounds.TopLeft.X - size / 2,
                bounds.TopLeft.Y - size / 2,
                size,
                size).Contains(point))
        {
            return ResizeHandle.TopLeft;
        }

        if (new Rect(
                bounds.TopRight.X - size / 2,
                bounds.TopRight.Y - size / 2,
                size,
                size).Contains(point))
        {
            return ResizeHandle.TopRight;
        }

        if (new Rect(
                bounds.BottomLeft.X - size / 2,
                bounds.BottomLeft.Y - size / 2,
                size,
                size).Contains(point))
        {
            return ResizeHandle.BottomLeft;
        }

        if (new Rect(
                bounds.BottomRight.X - size / 2,
                bounds.BottomRight.Y - size / 2,
                size,
                size).Contains(point))
        {
            return ResizeHandle.BottomRight;
        }

        return ResizeHandle.None;
    }

    private static ArrowHandle HitTestArrowHandle(
        Stroke stroke,
        Point point)
    {
        if (stroke.Points.Count < 2)
        {
            return ArrowHandle.None;
        }

        const double size = 8;

        var start = ToAvaloniaPoint(
            stroke.Points[0]);

        var end = ToAvaloniaPoint(
            stroke.Points[1]);

        var startBounds = new Rect(
            start.X - size / 2,
            start.Y - size / 2,
            size,
            size);

        if (startBounds.Contains(point))
        {
            return ArrowHandle.Start;
        }

        var endBounds = new Rect(
            end.X - size / 2,
            end.Y - size / 2,
            size,
            size);

        if (endBounds.Contains(point))
        {
            return ArrowHandle.End;
        }

        return ArrowHandle.None;
    }

    private static Rect GetRectangleBounds(
        Stroke stroke)
    {
        var start = ToAvaloniaPoint(
            stroke.Points[0]);

        var end = ToAvaloniaPoint(
            stroke.Points[1]);

        return new Rect(
            Math.Min(start.X, end.X),
            Math.Min(start.Y, end.Y),
            Math.Abs(end.X - start.X),
            Math.Abs(end.Y - start.Y));
    }

    private void ResizeRectangle(
        Point point)
    {
        if (_draggingStroke is null ||
            _resizeAnchorPoint is null)
        {
            return;
        }

        switch (_activeResizeHandle)
        {
            case ResizeHandle.BottomRight:

                _draggingStroke.Points[0] =
                    _resizeAnchorPoint.Value;

                _draggingStroke.Points[1] =
                    ToPoint2D(point);

                break;

            case ResizeHandle.BottomLeft:
                _draggingStroke.Points[0] =
                    new Point2D(
                        point.X,
                        _resizeAnchorPoint.Value.Y);

                _draggingStroke.Points[1] =
                    new Point2D(
                        _resizeAnchorPoint.Value.X,
                        point.Y);

                break;

            case ResizeHandle.TopRight:
                _draggingStroke.Points[0] =
                    new Point2D(
                        _resizeAnchorPoint.Value.X,
                        point.Y);

                _draggingStroke.Points[1] =
                    new Point2D(
                        point.X,
                        _resizeAnchorPoint.Value.Y);

                break;

            case ResizeHandle.TopLeft:
                _draggingStroke.Points[0] =
                    ToPoint2D(point);

                _draggingStroke.Points[1] =
                    _resizeAnchorPoint.Value;

                break;
        }

        InvalidateVisual();
    }

    private void MoveArrowEndpoint(
        Point point)
    {
        if (_draggingStroke is null)
        {
            return;
        }

        switch (_activeArrowHandle)
        {
            case ArrowHandle.Start:
                _draggingStroke.Points[0] =
                    ToPoint2D(point);
                break;

            case ArrowHandle.End:
                _draggingStroke.Points[1] =
                    ToPoint2D(point);
                break;
        }

        InvalidateVisual();
    }

    private static void DrawArrowSelectionOutline(
        DrawingContext context,
        Stroke stroke)
    {
        if (stroke.Points.Count < 2)
        {
            return;
        }

        var start =
            ToAvaloniaPoint(stroke.Points[0]);

        var end =
            ToAvaloniaPoint(stroke.Points[^1]);

        var left = Math.Min(
            start.X,
            end.X);

        var right = Math.Max(
            start.X,
            end.X);

        var top = Math.Min(
            start.Y,
            end.Y);

        var bottom = Math.Max(
            start.Y,
            end.Y);

        var bounds = new Rect(
            left - 8,
            top - 8,
            Math.Max(
                (right - left) + 16,
                16),
            Math.Max(
                (bottom - top) + 16,
                16));

        var pen = CreateSelectionPen();

        context.DrawRectangle(
            null,
            pen,
            bounds);

        DrawResizeHandle(
            context,
            start);

        DrawResizeHandle(
            context,
            end);
    }

    private static Pen CreateSelectionPen()
    {
        return new Pen(
            Brushes.White,
            1.5,
            dashStyle: new DashStyle(
                new[] { 4.0, 4.0 },
                0));
    }

    private Stroke? HitTest(Point point)
    {
        const double tolerance = 8.0;

        for (
            var index = _strokes.Count - 1;
            index >= 0;
            index--)
        {
            var stroke = _strokes[index];

            if (IsHit(
                stroke,
                point,
                tolerance))
            {
                return stroke;
            }
        }

        return null;
    }

    private static bool IsHit(
        Stroke stroke,
        Point point,
        double tolerance)
    {
        return stroke.Kind switch
        {
            StrokeKind.Rectangle =>
                IsRectangleHit(
                    stroke,
                    point,
                    tolerance),

            StrokeKind.Arrow =>
                IsLineHit(
                    stroke,
                    point,
                    tolerance),

            StrokeKind.Text =>
                IsTextHit(
                    stroke,
                    point,
                    tolerance),

            StrokeKind.Callout =>
                IsCalloutHit(
                    stroke,
                    point,
                    tolerance),

            _ => false
        };
    }

    private static bool IsRectangleHit(
        Stroke stroke,
        Point point,
        double tolerance)
    {
        if (stroke.Points.Count < 2)
        {
            return false;
        }

        var start =
            ToAvaloniaPoint(stroke.Points[0]);

        var end =
            ToAvaloniaPoint(stroke.Points[1]);

        var left = Math.Min(
            start.X,
            end.X);

        var right = Math.Max(
            start.X,
            end.X);

        var top = Math.Min(
            start.Y,
            end.Y);

        var bottom = Math.Max(
            start.Y,
            end.Y);

        var expanded = new Rect(
            left - tolerance,
            top - tolerance,
            (right - left) +
            tolerance * 2,
            (bottom - top) +
            tolerance * 2);

        return expanded.Contains(point);
    }

    private static bool IsLineHit(
        Stroke stroke,
        Point point,
        double tolerance)
    {
        if (stroke.Points.Count < 2)
        {
            return false;
        }

        var start =
            ToAvaloniaPoint(stroke.Points[0]);

        var end =
            ToAvaloniaPoint(stroke.Points[^1]);

        var deltaX = end.X - start.X;
        var deltaY = end.Y - start.Y;

        var lengthSquared =
            deltaX * deltaX +
            deltaY * deltaY;

        if (lengthSquared == 0)
        {
            return CalculateDistance(
                point,
                start) <= tolerance;
        }

        var interpolation =
            ((point.X - start.X) * deltaX +
             (point.Y - start.Y) * deltaY) /
            lengthSquared;

        interpolation = Math.Clamp(
            interpolation,
            0,
            1);

        var nearest = new Point(
            start.X +
            interpolation * deltaX,
            start.Y +
            interpolation * deltaY);

        return CalculateDistance(
            point,
            nearest) <= tolerance;
    }

    private static bool IsTextHit(
        Stroke stroke,
        Point point,
        double tolerance)
    {
        if (stroke.Points.Count == 0)
        {
            return false;
        }

        var origin = ToAvaloniaPoint(
            stroke.Points[0]);

        var formattedText = new FormattedText(
            stroke.Text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            24,
            Brushes.White);

        var bounds = new Rect(
            origin.X,
            origin.Y,
            formattedText.Width,
            formattedText.Height);

        bounds = bounds.Inflate(tolerance);

        return bounds.Contains(point);
    }

    private static bool IsCalloutHit(
        Stroke stroke,
        Point point,
        double tolerance)
    {
        if (stroke.Points.Count < 2)
        {
            return false;
        }

        if (IsLineHit(
            stroke,
            point,
            tolerance))
        {
            return true;
        }

        if (string.IsNullOrEmpty(stroke.Text))
        {
            return false;
        }

        var end =
            ToAvaloniaPoint(stroke.Points[1]);

        var formattedText = new FormattedText(
            stroke.Text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            20,
            Brushes.White);

        var textBounds = new Rect(
            end.X + 12,
            end.Y - formattedText.Height / 2,
            formattedText.Width,
            formattedText.Height);

        return textBounds
            .Inflate(tolerance)
            .Contains(point);
    }

    private static double CalculateDistance(
        Point first,
        Point second)
    {
        var deltaX =
            first.X - second.X;

        var deltaY =
            first.Y - second.Y;

        return Math.Sqrt(
            deltaX * deltaX +
            deltaY * deltaY);
    }

    private static Pen CreatePen(Stroke stroke)
    {
        var baseColor = stroke.Color switch
        {
            StrokeColor.Blue =>
                Colors.DodgerBlue,

            StrokeColor.Black =>
                Colors.Black,

            StrokeColor.Yellow =>
                Colors.Yellow,

            _ =>
                Colors.Red
        };

        var brush = new SolidColorBrush(
            baseColor,
            stroke.Opacity);

        return new Pen
        {
            Brush = brush,
            Thickness = stroke.Thickness,
            LineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };
    }

    private static Point2D ToPoint2D(
        Point point)
    {
        return new Point2D(
            point.X,
            point.Y);
    }

    private static Point ToAvaloniaPoint(
        Point2D point)
    {
        return new Point(
            point.X,
            point.Y);
    }

    private void ResetDragState()
    {
        _draggingStroke = null;
        _lastPointerPosition = null;
        _activeResizeHandle = ResizeHandle.None;
        _resizeAnchorPoint = null;
        _dragDeltaX = 0;
        _dragDeltaY = 0;
        _resizeBeforeStart = null;
        _resizeBeforeEnd = null;
        _activeArrowHandle = ArrowHandle.None;
        _activeArrowHandle = ArrowHandle.None;
        _arrowBeforeStart = null;
        _arrowBeforeEnd = null;
    }

    private void ClearInvalidSelection()
    {
        var selectedStroke =
            _selection.SelectedStroke;

        if (selectedStroke is not null &&
            !_strokes.Contains(selectedStroke))
        {
            _selection.Clear();
        }
    }

    public void Clear()
    {
        _strokes.Clear();
        _undoStack.Clear();
        _redoStack.Clear();

        _activeStroke = null;
        _isToolInteractionActive = false;

        ResetDragState();
        _selection.Clear();

        InvalidateVisual();
    }

    public void StartNewCalloutGroup()
    {
        _activeCalloutGroupId = Guid.NewGuid();

        InvalidateVisual();
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
        {
            return;
        }

        var action = _undoStack.Pop();

        action.Undo();

        _redoStack.Push(action);

        ClearInvalidSelection();
        ResetDragState();

        InvalidateVisual();
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
        {
            return;
        }

        var action = _redoStack.Pop();

        action.Redo();

        _undoStack.Push(action);

        ClearInvalidSelection();
        ResetDragState();

        InvalidateVisual();
    }

    public void DeleteSelection()
    {
        if (!_selection.HasSelection)
        {
            return;
        }

        var stroke =
            _selection.SelectedStroke!;

        var index =
            _strokes.IndexOf(stroke);

        if (index < 0)
        {
            _selection.Clear();
            return;
        }

        _strokes.RemoveAt(index);

        _undoStack.Push(
            new DeleteStrokeAction(
                _strokes,
                stroke,
                index));

        _redoStack.Clear();
        _selection.Clear();
        ResetDragState();

        InvalidateVisual();
    }

    public void NudgeSelection(
        double deltaX,
        double deltaY)
    {
        if (!_selection.HasSelection)
        {
            return;
        }

        var stroke = _selection.SelectedStroke!;

        stroke.Translate(
            deltaX,
            deltaY);

        _undoStack.Push(
            new MoveStrokeAction(
                stroke,
                deltaX,
                deltaY));

        _redoStack.Clear();

        InvalidateVisual();
    }
}