using GPGO_MultiPLCs.Models;
using GPMVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>提供身分驗證登入和系統設定</summary>
    public class Authenticator_ViewModel : ObservableObject
    {
        private class Users : RecipeFileBase<Users>
        {
            public List<User> List
            {
                get => Get<List<User>>();
                set => Set(value);
            }

            public Users() : base("Users") => List = new List<User>();
        }

        /// <summary>最高權限帳號</summary>
        private readonly User GP = new User
                                   {
                                       Name     = "GP",
                                       Password = "23555277",
                                       Level    = UserLevel.Super
                                   };

        /// <summary>最低權限帳號，訪客</summary>
        private readonly User Guest = new User
                                      {
                                          Name     = "Guest",
                                          Password = "",
                                          Level    = UserLevel.Guest
                                      };

        /// <summary>所有權限階級</summary>
        private readonly UserLevel[] Levels = {UserLevel.Super, UserLevel.Administrator, UserLevel.Manager, UserLevel.Operator};

        /// <summary>所有使用者列表</summary>
        private readonly Users UserList;

        /// <summary>新增使用者帳號</summary>
        public RelayCommand AddUser { get; }

        /// <summary>新增帳號可選擇指定的權限階級</summary>
        public IEnumerable<UserLevel> EditLevels => Levels.Where(x => x < NowUser.Level);

        /// <summary>當登入視窗關閉時</summary>
        public RelayCommand ExitLog { get; }

        /// <summary>系統參數</summary>
        public GlobalSettings Settings { get; }

        /// <summary>登入使用者</summary>
        public CommandWithResult<bool> Login { get; }

        /// <summary>登出使用者</summary>
        public RelayCommand Logout { get; }

        /// <summary>移除使用者</summary>
        public RelayCommand RemoveUser { get; }

        /// <summary>設定資料輸入路徑</summary>
        public RelayCommand SetInputPath { get; }

        /// <summary>設定資料輸出路徑</summary>
        public RelayCommand SetPath { get; }

        /// <summary>登入視窗開啟時</summary>
        public RelayCommand StartLog { get; }

        /// <summary>更新使用者列表</summary>
        public RelayCommand UpdateUser { get; }

        /// <summary>依據權限過濾顯示的使用者列表</summary>
        public List<User> ViewUsers => UserList.List.Where(x => x.Level < NowUser.Level).OrderByDescending(x => x.Level).ThenByDescending(x => x.LastLoginTime).ToList();

        /// <summary>辨別是否可新增使用者</summary>
        public bool Add_Enable
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>設定使用者權限(管理使用者時)</summary>
        public UserLevel EditLevel
        {
            get => Get<UserLevel>();
            set
            {
                Set(value);

                CheckEnable();
            }
        }

        /// <summary>設定使用者名稱</summary>
        public string EditName
        {
            get => Get<string>();
            set
            {
                Set(value);

                CheckEnable();
            }
        }

        /// <summary>設定使用者密碼</summary>
        public string EditPassword
        {
            get => Get<string>();
            set
            {
                Set(value);

                CheckEnable();
            }
        }

        /// <summary>是否顯示系統參數頁面</summary>
        public Visibility IsShown
        {
            get => Get<Visibility>();
            set => Set(value);
        }

        /// <summary>目前登入的使用者</summary>
        public User NowUser
        {
            get => Get<User>();
            set
            {
                Set(value);

                SelectedUser = null;
                NotifyPropertyChanged(nameof(ViewUsers));
                NotifyPropertyChanged(nameof(EditLevels));
            }
        }

        /// <summary>辨別是否可移除使用者</summary>
        public bool Remove_Enable
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>目前選取的使用者</summary>
        public User SelectedUser
        {
            get => Get<User>();
            set
            {
                Set(value);

                if (value == null)
                {
                    Set("",                 nameof(EditName));
                    Set("",                 nameof(EditPassword));
                    Set(UserLevel.Operator, nameof(EditLevel));
                }
                else
                {
                    var user = SelectedUser;
                    Set(user.Name,     nameof(EditName));
                    Set(user.Password, nameof(EditPassword));
                    Set(user.Level,    nameof(EditLevel));
                }

                CheckEnable();
            }
        }

        /// <summary>登入名稱</summary>
        public string TypedName
        {
            get => Get<string>();
            set
            {
                value = value.Length > 20 ? value.Substring(0, 20) : value;
                Set(value);
            }
        }

        /// <summary>辨別是否可變更使用者設定</summary>
        public bool Update_Enable
        {
            get => Get<bool>();
            set => Set(value);
        }

        public void CheckEnable()
        {
            if (EditName == GP.Name || string.IsNullOrEmpty(EditName) || string.IsNullOrEmpty(EditPassword))
            {
                Update_Enable = false;
                Remove_Enable = false;
                Add_Enable    = false;
            }
            else
            {
                Update_Enable = ViewUsers.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase) && (x.Password != EditPassword || x.Level != EditLevel));
                Remove_Enable = ViewUsers.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase) && x.Password == EditPassword && x.Level == EditLevel);
                Add_Enable    = !UserList.List.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public Authenticator_ViewModel()
        {
            IsShown = Visibility.Collapsed;

            UserList = new Users();
            UserList.Load(false);
            UserList.RegisterChanged();

            NowUser = UserList.List.Where(x => x.Level < UserLevel.Manager && x.LastLoginTime.Ticks != 0).OrderByDescending(x => x.LastLoginTime).FirstOrDefault() ?? Guest;

            Settings = new GlobalSettings();
            Settings.Load(false);
            Settings.RegisterChanged();

            UpdateUser = new RelayCommand(e =>
                                          {
                                              if (Update_Enable && SelectedUser != null)
                                              {
                                                  SelectedUser.Password = EditPassword;
                                                  SelectedUser.Level    = EditLevel;
                                                  UserList.Save();
                                              }
                                          });

            AddUser = new RelayCommand(e =>
                                       {
                                           if (Add_Enable)
                                           {
                                               UserList.List.Add(new User
                                                                 {
                                                                     Name        = EditName,
                                                                     Password    = EditPassword,
                                                                     Level       = EditLevel,
                                                                     CreatedTime = DateTime.Now
                                                                 });
                                               NotifyPropertyChanged(nameof(ViewUsers));
                                               NotifyPropertyChanged(nameof(Add_Enable));
                                               UserList.Save();
                                           }
                                       });

            RemoveUser = new RelayCommand(e =>
                                          {
                                              if (Remove_Enable)
                                              {
                                                  UserList.List.RemoveAll(x => x.Name == EditName);
                                                  NotifyPropertyChanged(nameof(ViewUsers));
                                                  NotifyPropertyChanged(nameof(Remove_Enable));
                                                  UserList.Save();
                                              }
                                          });

            Login = new CommandWithResult<bool>(e =>
                                                {
                                                    UserList.Load(true);

                                                    if (e is PasswordBox password)
                                                    {
                                                        var pass = password.Password;

                                                        password.Clear();

                                                        if (TypedName == GP.Name && pass == GP.Password)
                                                        {
                                                            NowUser = GP;

                                                            return true;
                                                        }

                                                        if (UserList.List.Find(x => x.Name == TypedName && x.Password == pass) is User _user)
                                                        {
                                                            _user.LastLoginTime = DateTime.Now;
                                                            NowUser             = _user;
                                                            UserList.Save();

                                                            return true;
                                                        }

                                                        return false;
                                                    }

                                                    return false;
                                                })
                    {
                        Result = true
                    };

            Logout = new RelayCommand(e =>
                                      {
                                          Login.Result = true;
                                          TypedName    = "";
                                          NowUser      = Guest;

                                          if (e is PasswordBox password)
                                          {
                                              password.Clear();
                                          }
                                      });

            StartLog = new RelayCommand(e =>
                                        {
                                            Login.Result = true;
                                            TypedName    = NowUser.Name;
                                            SelectedUser = null;
                                            IsShown      = Visibility.Visible;

                                            if (e is PasswordBox password)
                                            {
                                                password.Clear();
                                            }
                                        });

            ExitLog = new RelayCommand(e =>
                                       {
                                           Login.Result = true;
                                           IsShown      = Visibility.Collapsed;

                                           if (e is PasswordBox password)
                                           {
                                               password.Clear();
                                           }
                                       });

            SetInputPath = new RelayCommand(e =>
                                            {
                                                if (e is string str && Directory.Exists(str))
                                                {
                                                    Settings.DataInputPath = str;
                                                }
                                            });

            SetPath = new RelayCommand(e =>
                                       {
                                           if (e is string str && Directory.Exists(str))
                                           {
                                               Settings.DataOutputPath = str;
                                           }
                                       });
        }
    }
}