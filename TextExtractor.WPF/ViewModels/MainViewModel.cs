using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TextExtractor.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private IEnumerable<Screen> Screens { get; set; } = [];
    
    [ObservableProperty] private ObservableCollection<string> _displayList = [];
    [ObservableProperty] private string? _selectedDisplay;

    partial void OnSelectedDisplayChanged(string? value)
    {
        ShowDisplayPreview();
    }
    
    public MainViewModel()
    {
        RefreshDisplayList();
    }

    private void RefreshDisplayList()
    {
        Screens = Screen.AllScreens;

        if (Screens.Any()) DisplayItems();
    }

    private void DisplayItems()
    {
        DisplayList.Clear();
        ;
        foreach (var monitor in Screens)
        {
            DisplayList.Add(monitor.DeviceName);
        }
    }

    [RelayCommand]
    private void ShowDisplayPreview()
    {
        var selectedMonitor = Screens.FirstOrDefault(x => x.DeviceName.Equals(SelectedDisplay));
        
        Console.WriteLine(selectedMonitor);
    }
}