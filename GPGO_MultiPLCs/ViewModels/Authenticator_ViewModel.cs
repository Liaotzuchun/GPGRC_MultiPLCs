using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GPGO_MultiPLCs.Helpers;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.ViewModels
{
    /// <summary>提供身分驗證登入和系統設定</summary>
    public class Authenticator_ViewModel : ObservableObject
    {
        /// <summary>最高權限帳號</summary>
        private readonly User GP = new User { Name = "GP", Password = "23555277", Level = User.UserLevel.S };

        /// <summary>最低權限帳號，訪客</summary>
        private readonly User Guest = new User { Name = "", Password = "", Level = User.UserLevel.GU };

        /// <summary>所有權限階級</summary>
        private readonly User.UserLevel[] Levels = { User.UserLevel.S, User.UserLevel.AD, User.UserLevel.MA, User.UserLevel.OP };

        private const string UsersPath = "Users.json";

        /// <summary>所有使用者列表</summary>
        private List<User> Users;

        /// <summary>新增使用者帳號</summary>
        public RelayCommand AddUser { get; }

        /// <summary>新增帳號可選擇指定的權限階級</summary>
        public IEnumerable<User.UserLevel> EditLevels => Levels.Where(x => x < NowUser.Level);

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

        /// <summary>設定資料輸入路徑</summary>
        public RelayCommand SetInputPath { get; }

        /// <summary>設定資料輸出路徑</summary>
        public RelayCommand SetPath { get; }

        /// <summary>登入視窗開啟時</summary>
        public RelayCommand StartLog { get; }

        /// <summary>更新使用者列表</summary>
        public RelayCommand UpdateUser { get; }

        /// <summary>依據權限過濾顯示的使用者列表</summary>
        public IQueryable<User> ViewUsers => Users?.AsQueryable().Where(x => x.Level < NowUser.Level);

        /// <summary>辨別是否可新增使用者</summary>
        public bool Add_Enable
        {
            get => Get<bool>();
            set => Set(value);
        }

        /// <summary>設定使用者權限(管理使用者時)</summary>
        public User.UserLevel EditLevel
        {
            get => Get<User.UserLevel>();
            set
            {
                Set(value);
                Update_Enable = Users.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase) && (x.Password != EditPassword || x.Level != value));
                Remove_Enable = Users.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase) && x.Password == EditPassword && x.Level == value);
            }
        }

        /// <summary>設定使用者名稱</summary>
        public string EditName
        {
            get => Get<string>();
            set
            {
                Set(value);
                Update_Enable = Users.Exists(x => string.Equals(x.Name, value, StringComparison.CurrentCultureIgnoreCase) && (x.Password != EditPassword || x.Level != EditLevel));
                Add_Enable = !string.IsNullOrEmpty(EditPassword) && Users.TrueForAll(x => !string.Equals(x.Name, value, StringComparison.CurrentCultureIgnoreCase));
                Remove_Enable = Users.Exists(x => string.Equals(x.Name, value, StringComparison.CurrentCultureIgnoreCase) && x.Password == EditPassword && x.Level == EditLevel);
            }
        }

        /// <summary>設定使用者密碼</summary>
        public string EditPassword
        {
            get => Get<string>();
            set
            {
                Set(value);
                Update_Enable = Users.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase) && (x.Password != value || x.Level != EditLevel));
                Add_Enable = !string.IsNullOrEmpty(value) && Users.TrueForAll(x => !string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase));
                Remove_Enable = Users.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase) && x.Password == value && x.Level == EditLevel);
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
                    Set("", nameof(EditName));
                    Set("", nameof(EditPassword));
                    Set(User.UserLevel.GU, nameof(EditLevel));
                }
                else
                {
                    var user = SelectedUser;
                    Set(user.Name, nameof(EditName));
                    Set(user.Password, nameof(EditPassword));
                    Set(user.Level, nameof(EditLevel));
                }

                Update_Enable = Users.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase) && (x.Password != EditPassword || x.Level != EditLevel));
                Add_Enable = !string.IsNullOrEmpty(EditPassword) && EditLevel != User.UserLevel.GU && Users.TrueForAll(x => !string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase));
                Remove_Enable = Users.Exists(x => string.Equals(x.Name, EditName, StringComparison.CurrentCultureIgnoreCase) && x.Password == EditPassword && x.Level == EditLevel);
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

        /// <summary>讀取所有使用者列表</summary>
        public void LoadUsers()
        {
            if (File.Exists(UsersPath))
            {
                try
                {
                    if (UsersPath.ReadFromJsonFile<List<User>>() is List<User> val)
                    {
                        Users = val;
                    }
                    else
                    {
                        Users = new List<User>();
                        File.Move(UsersPath, "Users" + DateTime.Now.Ticks + ".back");
                        SaveUsers();
                    }
                }
                catch
                {
                    Users = new List<User>();
                    File.Move(UsersPath, "Users" + DateTime.Now.Ticks + ".back");
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
                Users?.WriteToJsonFile(UsersPath);
            }
            catch (Exception ex)
            {
                ex.RecordError();
            }
        }

        public Authenticator_ViewModel()
        {
            IsShown = Visibility.Collapsed;
            LoadUsers();

            NowUser = Users.Where(x => x.Level == User.UserLevel.OP && x.LastLoginTime.Ticks != 0).OrderByDescending(x => x.LastLoginTime).FirstOrDefault() ?? Guest;

            GT = new GlobalTempSettings();
            GT.Load();

            UpdateUser = new RelayCommand(e =>
                                          {
                                              if (Update_Enable)
                                              {
                                                  SelectedUser.Password = EditPassword;
                                                  SelectedUser.Level = EditLevel;
                                                  SaveUsers();
                                              }
                                          });

            AddUser = new RelayCommand(e =>
                                       {
                                           if (Add_Enable)
                                           {
                                               Users.Add(new User { Name = EditName, Password = EditPassword, Level = EditLevel, CreatedTime = DateTime.Now });
                                               NotifyPropertyChanged(nameof(ViewUsers));
                                               NotifyPropertyChanged(nameof(Add_Enable));
                                               SaveUsers();
                                           }
                                       });

            RemoveUser = new RelayCommand(e =>
                                          {
                                              if (Remove_Enable)
                                              {
                                                  Users.RemoveAll(x => x.Name == EditName);
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

                                                        if (TypedName == GP.Name && pass == GP.Password)
                                                        {
                                                            NowUser = GP;

                                                            return true;
                                                        }

                                                        if (Users != null && Users.Find(x => x.Name == TypedName && x.Password == pass) is User _user)
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
                                            TypedName = NowUser.Name;
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

            SetInputPath = new RelayCommand(e=>
                                             {
                                                 if (e is string str && Directory.Exists(str))
                                                 {
                                                     GT.DataInputPath = str;
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