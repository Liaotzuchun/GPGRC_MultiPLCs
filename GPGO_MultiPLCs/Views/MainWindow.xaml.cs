using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using GPGRC_MultiPLCs.Helpers;
using Linearstar.Windows.RawInput;
using Serilog;

namespace GPGRC_MultiPLCs.Views;

/// <summary>MainWindow.xaml 的互動邏輯</summary>
public partial class MainWindow
{
    public MainWindow() => InitializeComponent();

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

    private IntPtr Hook(IntPtr hwnd,
                        int msg,
                        IntPtr wparam,
                        IntPtr lparam,
                        ref bool handled)
    {
        if (msg == 0x00FF)
        {
            try
            {
                var data = RawInputData.FromHandle(lparam);

                if (Extensions.IsGodMode)
                {
                    Extensions.IsReaderInput = true;
                }
                else if (data.Device == null || string.IsNullOrEmpty(data.Device.ProductName))
                {
                    Extensions.IsReaderInput = false;
                }
                else
                {
                    var device = data.Device.ProductName?.ToLower();
                    Debug.WriteLine(device);
                    Extensions.IsReaderInput = device != null && !device.Contains("keyboard");
                    Debug.WriteLine(Extensions.IsReaderInput);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
            }
        }

        return IntPtr.Zero;
    }
}