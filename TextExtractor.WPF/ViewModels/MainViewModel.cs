using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TextExtractor.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<string> _displayList = [];

    public MainViewModel()
    {
        RefreshDisplayList();
    }

    private void RefreshDisplayList()
    {
        var monitors = Screen.AllScreens;

        if (monitors.Length != 0) DisplayItems(monitors);
    }

    private void DisplayItems(IEnumerable<Screen> monitors)
    {
        DisplayList.Clear();
        ;
        foreach (var monitor in monitors)
        {
            DisplayList.Add(monitor.DeviceName);
        }
    }
}