using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>主視窗</summary>
    public class MainWindow_ViewModel : ObservableObject
    {
        public RelayCommand ClosingCommand { get; }
        public RelayCommand LoadedCommand { get; }

        public int ViewIndex
        {
            get => Get<int>();
            set
            {
                Set(value);
                IndexChangedEvent?.Invoke(value);
            }
        }

        public event Func<User> CheckClosing;

        public event Action<int> IndexChangedEvent;

        public event Action<Dispatcher> LoadedEvent;

        public MainWindow_ViewModel(IDialogService dialog)
        {
            LoadedCommand = new RelayCommand(e =>
                                             {
                                                 LoadedEvent?.Invoke(e as Dispatcher);
                                             });

            ClosingCommand = new RelayCommand(async e =>
                                              {
                                                  if (!(e is CancelEventArgs ce))
                                                  {
                                                      return;
                                                  }

                                                  ce.Cancel = true;

                                                  if (CheckClosing != null && dialog != null)
                                                  {
                                                      var user = CheckClosing.Invoke();

                                                      if (user.Level > User.UserLevel.C)
                                                      {
                                                         var result = await dialog.ShowWithIntput(new Dictionary<Language, string>
                                                                                      {
                                                                                          { Language.TW, "請輸入權限密碼：" },
                                                                                          { Language.CHS, "请输入权限密码：" },
                                                                                          { Language.EN, "Please enter the permission password:" }
                                                                                      },
                                                                                      new Dictionary<Language, string> { { Language.TW, "驗證" }, { Language.CHS, "验证" }, { Language.EN, "Identify" } },
                                                                                      x => (x == user.Password,
                                                                                            new Dictionary<Language, string>
                                                                                            {
                                                                                                { Language.TW, "密碼錯誤！" }, { Language.CHS, "密码错误！" }, { Language.EN, "Wrong password!" }
                                                                                            }));

                                                          if (result.result)
                                                          {
                                                              Application.Current.Shutdown();
                                                          }
                                                      }
                                                      else
                                                      {
                                                          dialog.Show(new Dictionary<Language, string>
                                                                      {
                                                                          { Language.TW, "權限不足，不可關閉程式!" },
                                                                          { Language.CHS, "权限不足，不可关闭程序!" },
                                                                          { Language.EN, "Insufficient permissions,\n" + "can't close the program." }
                                                                      });
                                                      }
                                                  }
                                              });
        }
    }
}