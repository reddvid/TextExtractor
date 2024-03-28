using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaddleOCRSharp;

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
    private async Task DisplayScreenshot()
    {
        var selectedMonitor = Screens.FirstOrDefault(x => x.DeviceName.Equals(SelectedDisplay))!;

        Console.WriteLine(selectedMonitor);

        var left = selectedMonitor.Bounds.X + 48;
        var top = selectedMonitor.Bounds.Y + 106;
        var width = 620; //selectedMonitor.Bounds.Width;
        var height = 38; //selectedMonitor.Bounds.Height;

        using var screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var bmpGraphics = Graphics.FromImage(screenBmp);
        bmpGraphics.CopyFromScreen(left, top, 0, 0, new System.Drawing.Size(width, height));
        DisplayImage = Imaging.CreateBitmapSourceFromHBitmap(
            screenBmp.GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        await Task.Delay(2000);

        var imageFile = SaveBitmapToFile(DisplayImage);

        await Task.Delay(2000);

        var questTitle = ExtractText(imageFile);

        CreateTextFile(questTitle);

        DeleteImageFile(imageFile);
    }

    private void DeleteImageFile(string imageFile)
    {
        File.Delete(imageFile);
    }

    private void CreateTextFile(string questTitle)
    {
        using StreamWriter sw =
            File.CreateText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ac.txt");

        if (!string.IsNullOrWhiteSpace(questTitle))
            sw.Write("Quest: " + questTitle);
        else
            sw.Write("");
    }

    private string SaveBitmapToFile(BitmapSource image)
    {
        string root = Environment.CurrentDirectory;
        var filePath = root + "/" + DateTime.Now.ToString("MMddyyyy_hhmmtt") + ".png";
        using var fileStream = new FileStream(filePath, FileMode.Create);
        BitmapEncoder
            encoder =
                new PngBitmapEncoder(); // You can choose a different format (e.g., JpegBitmapEncoder, BmpBitmapEncoder, etc.)
        encoder.Frames.Add(BitmapFrame.Create(image));
        encoder.Save(fileStream);

        return filePath;
    }

    private string ExtractText(string filePath)
    {
        OCRModelConfig config = new OCRModelConfig();
        string root = Environment.CurrentDirectory;
        string modelPathroot = root + @"\en";
        config.det_infer = modelPathroot + @"\en_PP-OCRv3_det_infer";
        config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
        config.rec_infer = modelPathroot + @"\en_PP-OCRv3_rec_infer";
        config.keys = modelPathroot + @"\en_dict.txt";

        try
        {
            OCRParameter oCRParameter = new OCRParameter();
            PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
            OCRResult ocrResult = engine.DetectText(filePath);

            if (ocrResult != null)
            {
                Console.WriteLine(ocrResult.Text);

                return ocrResult.Text;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return string.Empty;
    }
}