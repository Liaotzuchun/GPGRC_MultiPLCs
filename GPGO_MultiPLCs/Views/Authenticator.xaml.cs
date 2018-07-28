﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace GPGO_MultiPLCs.Views
{
    /// <summary>Authenticator.xaml 的互動邏輯</summary>
    public partial class Authenticator : UserControl
    {
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PathText.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();

            if (TV.SelectedItem is TreeViewItem tvi)
            {
                tvi.IsSelected = false;
            }

            TV.Items.Clear();
            foreach (var s in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItem { Header = s, Tag = s, FontWeight = FontWeights.Normal };

                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
                TV.Items.Add(item);
            }

            TB.IsChecked = false;
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == null)
            {
                item.Items.Clear();
                try
                {
                    foreach (var s in Directory.EnumerateDirectories(item.Tag.ToString())
                                               .Where(x =>
                                                      {
                                                          var info = new DirectoryInfo(x).Attributes;

                                                          return !(info.HasFlag(FileAttributes.System) || info.HasFlag(FileAttributes.Hidden) || info.HasFlag(FileAttributes.ReadOnly));
                                                      }))
                    {
                        var subitem = new TreeViewItem { Header = s.Substring(s.LastIndexOf("\\", StringComparison.Ordinal) + 1), Tag = s, FontWeight = FontWeights.Normal };

                        subitem.Items.Add(null);
                        subitem.Expanded += Folder_Expanded;
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void NameBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            NameBox.Clear();
        }

        private void NameBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void Password_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var lng = Application.Current.Resources.MergedDictionaries.Last();
            lng.Source = new Uri("pack://application:,,,/Views/Languages/TW.xaml");
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            var lng = Application.Current.Resources.MergedDictionaries.Last();
            lng.Source = new Uri("pack://application:,,,/Views/Languages/CHS.xaml");
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            var lng = Application.Current.Resources.MergedDictionaries.Last();
            lng.Source = new Uri("pack://application:,,,/Views/Languages/EN.xaml");
        }

        private async void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Keyboard.ClearFocus();

                await Task.Delay(60);

                Keyboard.Focus(NameBox);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TV.Items.Clear();
            foreach (var s in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItem { Header = s, Tag = s, FontWeight = FontWeights.Normal };

                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
                TV.Items.Add(item);
            }
        }

        public Authenticator()
        {
            InitializeComponent();
        }
    }
}