using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>RecipeControl.xaml 的互動邏輯</summary>
    public partial class RecipeControl : UserControl
    {
        public RecipeControl()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            TC.SelectedIndex = 0;
        }
    }
}