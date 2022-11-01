using GPGO_MultiPLCs.Helpers;
using Linearstar.Windows.RawInput;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace GPGO_MultiPLCs.Views;

/// <summary>MainWindow.xaml 的互動邏輯</summary>
public partial class MainWindow
{
    private void GlobalDialog_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (Visibility == Visibility.Visible)
        {
            Authenticator.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Panel.SetZIndex(PopGrid, int.MaxValue);

        var windowInteropHelper = new WindowInteropHelper(this);

        RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard,
                                      RawInputDeviceFlags.ExInputSink,
                                      windowInteropHelper.Handle);

        var source = HwndSource.FromHwnd(windowInteropHelper.Handle);
        source?.AddHook(Hook);
    }

    private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
    {
        if (msg == 0x00FF)
        {
#if DEBUG
            Extensions.IsReaderInput = true;
#else
            var data = RawInputData.FromHandle(lparam);
            var device = data.Device.ProductName;
            Extensions.IsReaderInput = !device.ToLower().Contains("keyboard");
#endif
        }

        return IntPtr.Zero;
    }

    public MainWindow()
    {
        InitializeComponent();
    }
}