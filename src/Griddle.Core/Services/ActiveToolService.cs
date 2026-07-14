using Griddle.Core.Tools;

namespace Griddle.Core.Services;

public sealed class ActiveToolService
{
    private ITool _current;

    public ActiveToolService(ITool initialTool)
    {
        _current = initialTool;
    }

    public event EventHandler? CurrentToolChanged;

    public ITool Current
    {
        get => _current;
        set
        {
            if (ReferenceEquals(_current, value))
            {
                return;
            }

            _current = value;
            CurrentToolChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}