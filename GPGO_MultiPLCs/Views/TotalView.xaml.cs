﻿using System.Windows.Controls;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>
    ///     TotalView.xaml 的互動邏輯
    /// </summary>
    public partial class TotalView : UserControl
    {
        private void CB_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((ComboBox)sender).Text = "";
        }

        public TotalView()
        {
            InitializeComponent();
            CA.LabelFormatter = val => "No. " + (val + 1).ToString("00");
            //BS.LabelFormatString = "{0}";
        }
    }
}