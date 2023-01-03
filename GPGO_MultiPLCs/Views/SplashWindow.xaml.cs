using System;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace GPGO_MultiPLCs.Views;

/// <summary>SplashWindow.xaml 的互動邏輯</summary>
public partial class SplashWindow
{
    public SplashWindow() => InitializeComponent();

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel =  true;
        Closing  -= Window_Closing;

        var da = new DoubleAnimation
                 {
                     EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut },
                     BeginTime      = TimeSpan.FromMilliseconds(1500),
                     Duration       = TimeSpan.FromMilliseconds(300),
                     From           = 1,
                     To             = 0
                 };

        da.Completed += (_, _) =>
                        {
                            Close();
                        };

        BeginAnimation(OpacityProperty, da);
    }
}