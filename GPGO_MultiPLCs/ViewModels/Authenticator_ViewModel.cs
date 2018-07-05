using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GPGO_MultiPLCs.Helpers;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs.ViewModels
{
    public class Authenticator_ViewModel : ViewModelBase
    {
        public enum UserLevel
        {
            D, //Guest
            C, //Operator
            B, //Leader
            A, //Manager
            S //GP
        }

        private readonly UserLevel[] Levels = { UserLevel.S, UserLevel.A, UserLevel.B, UserLevel.C };
        private readonly User GP = new User { Name = "GP", Password = "23555277", Level = UserLevel.S };
        private readonly User Guest = new User { Name = "", Password = "", Level = UserLevel.D };

        private List<User> Users;

        private bool _Update_Enable;
        private bool _Add_Enable;
        private bool _Remove_Enable;
        private string _TypedName;
        private string _EditName;
        private string _EditPassword;
        private UserLevel _EditLevel;
        private User _NowUser;
        private int _SelectedIndex = -1;
        private User _SelectedUser;
        private Visibility _IsShown = Visibility.Collapsed;

        public RelayCommand UpdateUser { get; }
        public RelayCommand AddUser { get; }
        public RelayCommand RemoveUser { get; }
        public CommandWithResult<bool> Login { get; }
        public RelayCommand Logout { get; }
        public RelayCommand StartLog { get; }
        public RelayCommand ExitLog { get; }

        public IEnumerable<UserLevel> EditLevels => Levels.Where(x => x < _NowUser.Level);

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
                Update_Enable = Users.Exists(x => x.Name == _EditName && (x.Password != _EditPassword || x.Level != _EditLevel));
                Add_Enable = !string.IsNullOrEmpty(_EditPassword) && Users.TrueForAll(x => x.Name != _EditName);
                Remove_Enable = Users.Exists(x => x.Name == _EditName && x.Password == _EditPassword && x.Level == _EditLevel);
            }
        }

        public string EditPassword
        {
            get => _EditPassword;
            set
            {
                _EditPassword = value;
                NotifyPropertyChanged();
                Update_Enable = Users.Exists(x => x.Name == _EditName && (x.Password != _EditPassword || x.Level != _EditLevel));
                Add_Enable = !string.IsNullOrEmpty(_EditPassword) && Users.TrueForAll(x => x.Name != _EditName);
                Remove_Enable = Users.Exists(x => x.Name == _EditName && x.Password == _EditPassword && x.Level == _EditLevel);
            }
        }

        public UserLevel EditLevel
        {
            get => _EditLevel;
            set
            {
                _EditLevel = value;
                NotifyPropertyChanged();
                Update_Enable = Users.Exists(x => x.Name == _EditName && (x.Password != _EditPassword || x.Level != _EditLevel));
                Remove_Enable = Users.Exists(x => x.Name == _EditName && x.Password == _EditPassword && x.Level == _EditLevel);
            }
        }

        public User NowUser
        {
            get => _NowUser;
            set
            {
                _NowUser = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ViewUsers));
                NotifyPropertyChanged(nameof(EditLevels));
            }
        }

        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                _SelectedIndex = value;
                NotifyPropertyChanged();

                SelectedUser = _SelectedIndex == -1 ? null : ViewUsers.ElementAtOrDefault(_SelectedIndex);
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
                    _EditLevel = UserLevel.D;
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
                Update_Enable = Users.Exists(x => x.Name == _EditName && (x.Password != _EditPassword || x.Level != _EditLevel));
                Add_Enable = !string.IsNullOrEmpty(_EditPassword) && _EditLevel != UserLevel.D && Users.TrueForAll(x => x.Name != _EditName);
                Remove_Enable = Users.Exists(x => x.Name == _EditName && x.Password == _EditPassword && x.Level == _EditLevel);
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

        public void Save()
        {
            try
            {
                if (Users != null)
                {
                    File.WriteAllText("Users.json", JsonConvert.SerializeObject(Users, Formatting.Indented), Encoding.UTF8);
                }
            }
            catch
            {
            }
        }

        public void Load()
        {
            if (File.Exists("Users.json"))
            {
                try
                {
                    if (JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("Users.json", Encoding.UTF8)) is List<User> val)
                    {
                        Users = val;
                    }
                    else
                    {
                        Users = new List<User>();
                        File.Move("Users.json", "Users" + DateTime.Now.Ticks + ".back");
                        Save();
                    }
                }
                catch
                {
                    Users = new List<User>();
                    File.Move("Users.json", "Users" + DateTime.Now.Ticks + ".back");
                    Save();
                }
            }
            else
            {
                Users = new List<User>();
                Save();
            }
        }

        public sealed class User : ViewModelBase
        {
            public string _Name;
            public string _Password;
            public UserLevel _Level;
            public DateTime _LastLoginTime;
            public DateTime _CreatedTime;

            public string Name
            {
                get => _Name;
                set
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }

            public string Password
            {
                get => _Password;
                set
                {
                    _Password = value;
                    NotifyPropertyChanged();
                }
            }

            public UserLevel Level
            {
                get => _Level;
                set
                {
                    _Level = value;
                    NotifyPropertyChanged();
                }
            }

            public DateTime LastLoginTime
            {
                get => _LastLoginTime;
                set
                {
                    _LastLoginTime = value;
                    NotifyPropertyChanged();
                }
            }

            public DateTime CreatedTime
            {
                get => _CreatedTime;
                set
                {
                    _CreatedTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Authenticator_ViewModel()
        {
            Load();

            NowUser = Users.Where(x => x.Level == UserLevel.C).OrderByDescending(x => x.LastLoginTime).FirstOrDefault() ?? Guest;

            UpdateUser = new RelayCommand(e =>
                                          {
                                              if (_Update_Enable)
                                              {
                                                  _SelectedUser.Password = _EditPassword;
                                                  _SelectedUser.Level = _EditLevel;
                                                  Save();
                                              }
                                          });

            AddUser = new RelayCommand(e =>
                                       {
                                           if (_Add_Enable)
                                           {
                                               Users.Add(new User { Name = _EditName, Password = _EditPassword, Level = _EditLevel, CreatedTime = DateTime.Now });
                                               NotifyPropertyChanged(nameof(ViewUsers));
                                               Save();
                                           }
                                       });

            RemoveUser = new RelayCommand(e =>
                                          {
                                              if (_Remove_Enable)
                                              {
                                                  Users.RemoveAll(x => x.Name == _EditName);
                                                  NotifyPropertyChanged(nameof(ViewUsers));
                                                  Save();
                                              }
                                          });

            Login = new CommandWithResult<bool>(e =>
                                                {
                                                    Load();

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
                                                            Save();

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
                                            TypedName = "";
                                            IsShown = Visibility.Visible;
                                            Users = new List<User>();
                                            NotifyPropertyChanged(nameof(ViewUsers));

                                            if (e is PasswordBox password)
                                            {
                                                password.Clear();
                                            }
                                        });

            ExitLog = new RelayCommand(e =>
                                       {
                                           Login.Result = true;
                                           TypedName = "";
                                           IsShown = Visibility.Collapsed;
                                           Users = new List<User>();
                                           NotifyPropertyChanged(nameof(ViewUsers));

                                           if (e is PasswordBox password)
                                           {
                                               password.Clear();
                                           }
                                       });
        }
    }
}