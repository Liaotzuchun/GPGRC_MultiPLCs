﻿using System.ComponentModel;
using System.Windows;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Views
{
    public sealed class BindingDataProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingDataProxy), new PropertyMetadata(null));

        [Bindable(true)]
        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new DependencyCommand();
        }
    }
}