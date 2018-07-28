using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>提供身分驗證登入和系統設定</summary>
    public class Authenticator_ViewModel : ViewModelBase
    {
        /// <summary>最高權限帳號</summary>
        private readonly User GP = new User { Name = "GP", Password = "23555277", Level = User.UserLevel.S };

        /// <summary>最低權限帳號，訪客</summary>
        private readonly User Guest = new User { Name = "", Password = "", Level = User.UserLevel.D };

        /// <summary>所有權限階級</summary>
        private readonly User.UserLevel[] Levels = { User.UserLevel.S, User.UserLevel.A, User.UserLevel.B, User.UserLevel.C };

        private bool _Add_Enable;
        private User.UserLevel _EditLevel;
        private string _EditName;
        private string _EditPassword;
        private Visibility _IsShown = Visibility.Collapsed;
        private User _NowUser;
        private bool _Remove_Enable;
        private User _SelectedUser;
        private string _TypedName;
        private bool _Update_Enable;

        /// <summary>所有使用者列表</summary>
        private List<User> Users;

        /// <summary>新增使用者帳號</summary>
        public RelayCommand AddUser { get; }

        /// <summary>新增帳號可選擇指定的權限階級</summary>
        public IEnumerable<User.UserLevel> EditLevels => Levels.Where(x => x < _NowUser.Level);

        /// <summary>當登入視窗關閉時</summary>
        public RelayCommand ExitLog { get; }

        /// <summary>系統參數</summary>
        public GlobalTempSettings GT { get; }

        /// <summary>登入使用者</summary>
        public CommandWithResult<bool> Login { get; }

        /// <summary>登出使用者</summary>
        public RelayCommand Logout { get; }

        /// <summary>移除使用者</summary>
        public RelayCommand RemoveUser { get; }

        /// <summary>設定資料輸出路徑</summary>
        public RelayCommand SetPath { get; }

        /// <summary>登入視窗開啟時</summary>
        public RelayCommand StartLog { get; }

        /// <summary>更新使用者列表</summary>
        public RelayCommand UpdateUser { get; }

        /// <summary>依據權限過濾顯示的使用者列表</summary>
        public IQueryable<User> ViewUsers => Users?.AsQueryable().Where(x => x.Level < _NowUser.Level);

        /// <summary>辨別是否可新增使用者</summary>
        public bool Add_Enable
        {
            get => _Add_Enable;
            set
            {
                _Add_Enable = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>設定權限</summary>
        public User.UserLevel EditLevel
        {
            get => _EditLevel;
            set
            {
                _EditLevel = value;
                NotifyPropertyChanged();
                Update_Enable = Users.Exists(x => string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase) && (x.Password != _EditPassword || x.Level != _EditLevel));
                Remove_Enable = Users.Exists(x => string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase) && x.Password == _EditPassword && x.Level == _EditLevel);
            }
        }

        /// <summary>設定使用者名稱</summary>
        public string EditName
        {
            get => _EditName;
            set
            {
                _EditName = value;
                NotifyPropertyChanged();
                Update_Enable = Users.Exists(x => string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase) && (x.Password != _EditPassword || x.Level != _EditLevel));
                Add_Enable = !string.IsNullOrEmpty(_EditPassword) && Users.TrueForAll(x => !string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase));
                Remove_Enable = Users.Exists(x => string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase) && x.Password == _EditPassword && x.Level == _EditLevel);
            }
        }

        /// <summary>設定使用者密碼</summary>
        public string EditPassword
        {
            get => _EditPassword;
            set
            {
                _EditPassword = value;
                NotifyPropertyChanged();
                Update_Enable = Users.Exists(x => string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase) && (x.Password != _EditPassword || x.Level != _EditLevel));
                Add_Enable = !string.IsNullOrEmpty(_EditPassword) && Users.TrueForAll(x => !string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase));
                Remove_Enable = Users.Exists(x => string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase) && x.Password == _EditPassword && x.Level == _EditLevel);
            }
        }

        /// <summary>是否顯示系統參數頁面</summary>
        public Visibility IsShown
        {
            get => _IsShown;
            set
            {
                _IsShown = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>目前登入的使用者</summary>
        public User NowUser
        {
            get => _NowUser;
            set
            {
                _NowUser = value;
                NotifyPropertyChanged();

                SelectedUser = null;
                NotifyPropertyChanged(nameof(ViewUsers));
                NotifyPropertyChanged(nameof(EditLevels));
            }
        }

        /// <summary>辨別是否可移除使用者</summary>
        public bool Remove_Enable
        {
            get => _Remove_Enable;
            set
            {
                _Remove_Enable = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>目前選取的使用者</summary>
        public User SelectedUser
        {
            get => _SelectedUser;
            set
            {
                _SelectedUser = value;
                NotifyPropertyChanged();

                if (_SelectedUser == null)
                {
                    _EditName = "";
                    _EditPassword = "";
                    _EditLevel = User.UserLevel.C;
                }
                else
                {
                    _EditName = _SelectedUser.Name;
                    _EditPassword = _SelectedUser.Password;
                    _EditLevel = _SelectedUser.Level;
                }

                NotifyPropertyChanged(nameof(EditName));
                NotifyPropertyChanged(nameof(EditPassword));
                NotifyPropertyChanged(nameof(EditLevel));
                Update_Enable = Users.Exists(x => string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase) && (x.Password != _EditPassword || x.Level != _EditLevel));
                Add_Enable = !string.IsNullOrEmpty(_EditPassword) &&
                             _EditLevel != User.UserLevel.D &&
                             Users.TrueForAll(x => !string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase));
                Remove_Enable = Users.Exists(x => string.Equals(x.Name, _EditName, StringComparison.CurrentCultureIgnoreCase) && x.Password == _EditPassword && x.Level == _EditLevel);
            }
        }

        /// <summary>登入名稱</summary>
        public string TypedName
        {
            get => _TypedName;
            set
            {
                _TypedName = value.Length > 20 ? value.Substring(0, 20) : value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>辨別是否可變更使用者設定</summary>
        public bool Update_Enable
        {
            get => _Update_Enable;
            set
            {
                _Update_Enable = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>讀取所有使用者列表</summary>
        public void LoadUsers()
        {
            if (File.Exists("Users.json"))
            {
                try
                {
                    if (JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("Users.json", Encoding.Unicode)) is List<User> val)
                    {
                        Users = val;
                    }
                    else
                    {
                        Users = new List<User>();
                        File.Move("Users.json", "Users" + DateTime.Now.Ticks + ".back");
                        SaveUsers();
                    }
                }
                catch
                {
                    Users = new List<User>();
                    File.Move("Users.json", "Users" + DateTime.Now.Ticks + ".back");
                    SaveUsers();
                }
            }
            else
            {
                Users = new List<User>();
                SaveUsers();
            }
        }

        /// <summary>儲存使用者列表</summary>
        public void SaveUsers()
        {
            try
            {
                if (Users != null)
                {
                    File.WriteAllText("Users.json", JsonConvert.SerializeObject(Users, Formatting.Indented), Encoding.Unicode);
                }
            }
            catch
            {
            }
        }

        public Authenticator_ViewModel()
        {
            LoadUsers();

            NowUser = Users.Where(x => x.Level == User.UserLevel.C && x.LastLoginTime.Ticks != 0).OrderByDescending(x => x.LastLoginTime).FirstOrDefault() ?? Guest;

            GT = new GlobalTempSettings();
            GT.Load();

            UpdateUser = new RelayCommand(e =>
                                          {
                                              if (_Update_Enable)
                                              {
                                                  _SelectedUser.Password = _EditPassword;
                                                  _SelectedUser.Level = _EditLevel;
                                                  SaveUsers();
                                              }
                                          });

            AddUser = new RelayCommand(e =>
                                       {
                                           if (_Add_Enable)
                                           {
                                               Users.Add(new User { Name = _EditName, Password = _EditPassword, Level = _EditLevel, CreatedTime = DateTime.Now });
                                               NotifyPropertyChanged(nameof(ViewUsers));
                                               NotifyPropertyChanged(nameof(Add_Enable));
                                               SaveUsers();
                                           }
                                       });

            RemoveUser = new RelayCommand(e =>
                                          {
                                              if (_Remove_Enable)
                                              {
                                                  Users.RemoveAll(x => x.Name == _EditName);
                                                  NotifyPropertyChanged(nameof(ViewUsers));
                                                  NotifyPropertyChanged(nameof(Remove_Enable));
                                                  SaveUsers();
                                              }
                                          });

            Login = new CommandWithResult<bool>(e =>
                                                {
                                                    LoadUsers();

                                                    if (e is PasswordBox password)
                                                    {
                                                        var pass = password.Password;

                                                        password.Clear();

                                                        if (_TypedName == GP.Name && pass == GP.Password)
                                                        {
                                                            NowUser = GP;

                                                            return true;
                                                        }

                                                        if (Users != null && Users.Find(x => x.Name == _TypedName && x.Password == pass) is User _user)
                                                        {
                                                            _user.LastLoginTime = DateTime.Now;
                                                            NowUser = _user;
                                                            SaveUsers();

                                                            return true;
                                                        }

                                                        return false;
                                                    }

                                                    return false;
                                                }) { Result = true };

            Logout = new RelayCommand(e =>
                                      {
                                          Login.Result = true;
                                          TypedName = "";
                                          NowUser = Guest;

                                          if (e is PasswordBox password)
                                          {
                                              password.Clear();
                                          }
                                      });

            StartLog = new RelayCommand(e =>
                                        {
                                            Login.Result = true;
                                            TypedName = _NowUser.Name;
                                            SelectedUser = null;
                                            IsShown = Visibility.Visible;

                                            if (e is PasswordBox password)
                                            {
                                                password.Clear();
                                            }
                                        });

            ExitLog = new RelayCommand(e =>
                                       {
                                           Login.Result = true;
                                           IsShown = Visibility.Collapsed;

                                           if (e is PasswordBox password)
                                           {
                                               password.Clear();
                                           }
                                       });

            SetPath = new RelayCommand(e =>
                                       {
                                           if (e is string str && Directory.Exists(str))
                                           {
                                               GT.DataOutputPath = str;
                                           }
                                       });
        }
    }
}