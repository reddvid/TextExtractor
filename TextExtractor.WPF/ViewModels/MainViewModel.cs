using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TextExtractor.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private IEnumerable<Screen> Screens { get; set; } = [];

    [ObservableProperty] private ObservableCollection<string> _displayList = [];

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasSelectedDisplay))]
    private string? _selectedDisplay;

    [ObservableProperty] private BitmapSource? _displayImage;

    public bool HasSelectedDisplay => !string.IsNullOrWhiteSpace(SelectedDisplay);

    public MainViewModel()
    {
        SelectedDisplay = null;
        
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
    private void DisplayScreenshot()
    {
        var selectedMonitor = Screens.FirstOrDefault(x => x.DeviceName.Equals(SelectedDisplay))!;

        Console.WriteLine(selectedMonitor);

        var left = selectedMonitor.Bounds.X + 48;
        var top = selectedMonitor.Bounds.Y + 84;
        var width = 620; //selectedMonitor.Bounds.Width;
        var height = 240; //selectedMonitor.Bounds.Height;

        using var screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var bmpGraphics = Graphics.FromImage(screenBmp);
        bmpGraphics.CopyFromScreen(left, top, 0, 0, new System.Drawing.Size(width, height));
        DisplayImage = Imaging.CreateBitmapSourceFromHBitmap(
            screenBmp.GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
    }
}