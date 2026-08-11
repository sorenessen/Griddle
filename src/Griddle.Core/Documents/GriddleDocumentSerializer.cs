using System.Text.Json;
using System.Text.Json.Serialization;

namespace Griddle.Core.Documents;

public static class GriddleDocumentSerializer
{
    private static readonly JsonSerializerOptions Options =
        new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

    public static string Serialize(
        GriddleDocument document)
    {
        return JsonSerializer.Serialize(
            document,
            Options);
    }

    public static GriddleDocument Deserialize(
        string json)
    {
        var document =
            JsonSerializer.Deserialize<GriddleDocument>(
                json,
                Options);

        if (document is null)
        {
            throw new InvalidOperationException(
                "Unable to deserialize Griddle document.");
        }

        if (document.Version >
            GriddleDocument.CurrentVersion)
        {
            throw new NotSupportedException(
                $"Griddle document version " +
                $"{document.Version} is newer than " +
                $"supported version " +
                $"{GriddleDocument.CurrentVersion}.");
        }

        return document;
    }
}