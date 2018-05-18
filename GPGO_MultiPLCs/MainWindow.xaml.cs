using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GPGO_MultiPLCs.Helpers;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var dic = new TwoKeyDictionary<string, int, short>();
            dic.Add("ooxx", 100, 999);
            dic.Add("ggyy", 101, 888);

            dic["ooxx"] = 123;
            dic[101] = 456;
            Console.WriteLine(dic["ooxx"]);
            Console.WriteLine(dic[101]);

            Console.WriteLine(JsonConvert.SerializeObject(dic));

        }
    }
}
