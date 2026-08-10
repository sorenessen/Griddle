using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Griddle.App.ViewModels;

public sealed class DisplayOption : INotifyPropertyChanged
{
    private bool _isActive;

    public DisplayOption(
        int index,
        string name)
    {
        Index = index;
        Name = name;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Index { get; }

    public string Name { get; }

    public bool IsActive
    {
        get => _isActive;

        set
        {
            if (_isActive == value)
            {
                return;
            }

            _isActive = value;

            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}