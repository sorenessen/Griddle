using Griddle.Core.Geometry;
using Griddle.Core.Models;
using Griddle.Core.Sessions;
using Griddle.Core.Captures;

namespace Griddle.Core.Documents;

public static class GriddleDocumentMapper
{
    public static GriddleDocument ToDocument(
        GriddleSession session)
    {
        return new GriddleDocument
        {
            Version = GriddleDocument.CurrentVersion,
            SessionId = session.Id,
            Name = session.Name,
            CreatedAt = session.CreatedAt,
            ModifiedAt = session.ModifiedAt,
            Strokes = session.Strokes
                .Select(ToDocument)
                .ToList(),

            Captures = session.Captures
                .Select(ToDocument)
                .ToList()

        };
    }

    public static GriddleSession ToSession(
        GriddleDocument document)
    {
        return new GriddleSession
        {
            Id = document.SessionId,
            Name = document.Name,
            CreatedAt = document.CreatedAt,
            ModifiedAt = document.ModifiedAt,
            Strokes = document.Strokes
                .Select(ToStroke)
                .ToList(),

            Captures = document.Captures
                .Select(ToCapture)
                .ToList()
        };
    }

    private static StrokeDocument ToDocument(
        Stroke stroke)
    {
        return new StrokeDocument
        {
            Id = stroke.Id,
            Color = stroke.Color,
            Thickness = stroke.Thickness,
            Opacity = stroke.Opacity,
            Kind = stroke.Kind,
            CalloutNumber = stroke.CalloutNumber,
            CalloutGroupId = stroke.CalloutGroupId,
            CalloutLabelPosition =
                stroke.CalloutLabelPosition,
            Text = stroke.Text,
            IsVisible = stroke.IsVisible,
            Points = stroke.Points
                .Select(point =>
                    new PointDocument
                    {
                        X = point.X,
                        Y = point.Y
                    })
                .ToList()
        };
    }

    private static Stroke ToStroke(
        StrokeDocument document)
    {
        var stroke = new Stroke(
            document.Color,
            document.Thickness,
            document.Opacity,
            document.Kind,
            document.Id)
        {
            CalloutNumber =
                document.CalloutNumber,

            CalloutGroupId =
                document.CalloutGroupId,

            CalloutLabelPosition =
                document.CalloutLabelPosition,

            Text =
                document.Text,

            IsVisible =
                document.IsVisible
        };

        foreach (var point in document.Points)
        {
            stroke.Points.Add(
                new Point2D(
                    point.X,
                    point.Y));
        }

        return stroke;
    }

    private static CaptureDocument ToDocument(
        GriddleCapture capture)
    {
        return new CaptureDocument
        {
            Id = capture.Id,
            Kind = capture.Kind,
            CreatedAt = capture.CreatedAt,
            FileName = capture.FileName,
            Width = capture.Width,
            Height = capture.Height,
            DisplayName = capture.DisplayName,
            IncludesAnnotations =
                capture.IncludesAnnotations
        };
    }

    private static GriddleCapture ToCapture(
        CaptureDocument document)
    {
        return new GriddleCapture
        {
            Id = document.Id,
            Kind = document.Kind,
            CreatedAt = document.CreatedAt,
            FileName = document.FileName,
            Width = document.Width,
            Height = document.Height,
            DisplayName = document.DisplayName,
            IncludesAnnotations =
                document.IncludesAnnotations
        };
    }
}