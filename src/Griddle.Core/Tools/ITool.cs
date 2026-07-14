using Griddle.Core.Geometry;
using Griddle.Core.Models;

namespace Griddle.Core.Tools;

public interface ITool
{
    string Name { get; }

    Stroke Begin(Point2D point);

    void Continue(Point2D point);

    Stroke End(Point2D point);
}