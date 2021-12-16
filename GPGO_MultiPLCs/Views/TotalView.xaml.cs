namespace GPGO_MultiPLCs.Views
{
    /// <summary>TotalView.xaml 的互動邏輯</summary>
    public partial class TotalView
    {
        public TotalView()
        {
            InitializeComponent();
            CA.LabelFormatter = val => $"No. {val + 1:00}";
            //BS.LabelFormatString = "{0}";
        }
    }
}