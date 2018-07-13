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
    public class Authenticator_ViewModel : ViewModelBase
    {
        private readonly User.UserLevel[] Levels = { User.UserLevel.S, User.UserLevel.A, User.UserLevel.B, User.UserLevel.C };
        private readonly User GP = new User { Name = "GP", Password = "23555277", Level = User.UserLevel.S };
        private readonly User Guest = new User { Name = "", Password = "", Level = User.UserLevel.D };

        private List<User> Users;

        private bool _Update_Enable;
        private bool _Add_Enable;
        private bool _Remove_Enable;
        private string _TypedName;
        private string _EditName;
        private string _EditPassword;
        private User.UserLevel _EditLevel;
        private User _NowUser;
        private User _SelectedUser;
        private Visibility _IsShown = Visibility.Collapsed;

        public RelayCommand UpdateUser { get; }
        public RelayCommand AddUser { get; }
        public RelayCommand RemoveUser { get; }
        public CommandWithResult<bool> Login { get; }
        public RelayCommand Logout { get; }
        public RelayCommand StartLog { get; }
        public RelayCommand ExitLog { get; }
        public RelayCommand SetPath { get; }
        public GlobalTempSettings GT { get; }

        public IEnumerable<User.UserLevel> EditLevels => Levels.Where(x => x < _NowUser.Level);

        public IQueryable<User> ViewUsers => Users?.AsQueryable().Where(x => x.Level < _NowUser.Level);

        public bool Update_Enable
        {
            get => _Update_Enable;
            set
            {
                _Update_Enable = value;
                NotifyPropertyChanged();
            }
        }

        public bool Add_Enable
        {
            get => _Add_Enable;
            set
            {
                _Add_Enable = value;
                NotifyPropertyChanged();
            }
        }

        public bool Remove_Enable
        {
            get => _Remove_Enable;
            set
            {
                _Remove_Enable = value;
                NotifyPropertyChanged();
            }
        }

        public string TypedName
        {
            get => _TypedName;
            set
            {
                _TypedName = value.Length > 20 ? value.Substring(0, 20) : value;
                NotifyPropertyChanged();
            }
        }

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

        public Visibility IsShown
        {
            get => _IsShown;
            set
            {
                _IsShown = value;
                NotifyPropertyChanged();
            }
        }

        public string DataOutputPath
        {
            get
            {
                if (File.Exists("OutputPath.txt"))
                {
                    try
                    {
                        return File.ReadAllText("OutputPath.txt", Encoding.Unicode);
                    }
                    catch
                    {
                        File.WriteAllText("OutputPath.txt", "D:\\", Encoding.Unicode);
                        return "D:\\";
                    }
                }
                else
                {
                    File.WriteAllText("OutputPath.txt", "D:\\", Encoding.Unicode);
                    return "D:\\";
                }
            }
            set
            {
                try
                {
                    File.WriteAllText("OutputPath.txt", value, Encoding.Unicode);
                }
                catch
                {

                }

                NotifyPropertyChanged();
            }
        }

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
                                           if(e is string str && Directory.Exists(str))
                                           {
                                               DataOutputPath = str;
                                           }
                                       });
        }
    }
}