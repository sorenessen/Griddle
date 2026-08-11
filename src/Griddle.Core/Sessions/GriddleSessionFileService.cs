using System;
using System.IO;
using Griddle.Core.Documents;

namespace Griddle.Core.Sessions;

public static class GriddleSessionFileService
{
    public static void Save(
        GriddleSession session,
        string filePath)
    {
        session.ModifiedAt =
            DateTime.UtcNow;

        var document =
            GriddleDocumentMapper.ToDocument(
                session);

        var json =
            GriddleDocumentSerializer.Serialize(
                document);

        File.WriteAllText(
            filePath,
            json);
    }

    public static GriddleSession Load(
        string filePath)
    {
        var json =
            File.ReadAllText(
                filePath);

        var document =
            GriddleDocumentSerializer.Deserialize(
                json);

        return GriddleDocumentMapper.ToSession(
            document);
    }
}