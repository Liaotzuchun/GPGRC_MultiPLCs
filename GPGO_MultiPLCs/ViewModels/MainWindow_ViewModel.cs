﻿using GPMVVM.Helpers;
using GPMVVM.Models;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Threading;

namespace GPGO_MultiPLCs.ViewModels;

/// <summary>主視窗</summary>
public class MainWindow_ViewModel : ObservableObject
{
    public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    public RelayCommand ClosingCommand { get; }
    public RelayCommand LoadedCommand  { get; }

    public int ViewIndex
    {
        get => Get<int>();
        set
        {
            Set(value);
            IndexChangedEvent?.Invoke(value);
        }
    }

    public event Action CheckClosing;

    public event Action<int> IndexChangedEvent;

    public event Action<Dispatcher> LoadedEvent;

    public MainWindow_ViewModel()
    {
        LoadedCommand = new RelayCommand(e =>
                                         {
                                             LoadedEvent?.Invoke(e as Dispatcher);
                                         });

        ClosingCommand = new RelayCommand(e =>
                                          {
                                              if (e is not CancelEventArgs ce)
                                              {
                                                  return;
                                              }

                                              ce.Cancel = true;
                                              CheckClosing?.Invoke();
                                          });
    }
}