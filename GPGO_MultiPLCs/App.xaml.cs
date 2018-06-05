using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace GPGO_MultiPLCs
{
    /// <summary>
    ///     App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
        }
    }
}