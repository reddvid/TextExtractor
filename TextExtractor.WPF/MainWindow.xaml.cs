using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextExtractor.WPF.ViewModels;

namespace TextExtractor.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel ViewModel { get; } = new();
    private const int HOTKEY_ID = 9000;

// Modifiers:
    private const uint MOD_NONE = 0x0000; //[NONE]
    private const uint MOD_ALT = 0x0001; //ALT
    private const uint MOD_CONTROL = 0x0002; //CTRL
    private const uint MOD_SHIFT = 0x0004; //SHIFT
    private const uint MOD_WIN = 0x0008; //WINDOWS

// CAPS LOCK:
    private const uint VK_CAPITAL = 0x14;

    // T:
    private const uint VK_T = 0x54;
    private const uint VK_SNAPSHOT = 0x2C;

    private HwndSource _source;

    public MainWindow()
    {
        InitializeComponent();

        DataContext = ViewModel;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        IntPtr handle = new WindowInteropHelper(this).Handle;
        _source = HwndSource.FromHwnd(handle);
        _source.AddHook(HwndHook);

        RegisterHotKey(handle, HOTKEY_ID, MOD_CONTROL, VK_SNAPSHOT); // CTRL + CAPS_LOCK
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_HOTKEY = 0x0312;
        switch (msg)
        {
            case WM_HOTKEY:
                switch (wParam.ToInt32())
                {
                    case HOTKEY_ID:
                        int vkey = (int)lParam >> 16 & 0xFFFF;
                        if (vkey == VK_SNAPSHOT)
                        {
                            Console.WriteLine("Hotkey Pressed!!!");
                            if (string.IsNullOrWhiteSpace(ViewModel.SelectedDisplay) &&
                                ViewModel.DisplayList.Count != 0)
                            {
                                DisplaysList.SelectedIndex = 0;

                                break;
                            }
                            
                            ViewModel.DisplayScreenshotCommand.Execute(default);
                        }

                        handled = true;
                        break;
                }

                break;
        }

        return IntPtr.Zero;
    }

    // DLL libraries used to manage hotkeys
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


    protected override void OnClosing(CancelEventArgs e)
    { 
        IntPtr handle = new WindowInteropHelper(this).Handle;
        _source = HwndSource.FromHwnd(handle);
        _source.AddHook(HwndHook);
        
        UnregisterHotKey(handle, HOTKEY_ID);
        base.OnClosing(e);
    }
}