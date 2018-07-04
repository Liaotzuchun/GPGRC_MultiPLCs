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

        private readonly User GP = new User { Name = "GP", Password = "23555277", Level = UserLevel.S };
        private readonly User Guest = new User { Name = "", Password = "", Level = UserLevel.D };

        private List<User> Users;
        private string _TypedName;
        private User _NowUser;
        private Visibility _IsShown = Visibility.Collapsed;

        public RelayCommand AddUser { get; }
        public RelayCommand RemoveUser { get; }
        public CommandWithResult<bool> Login { get; }
        public RelayCommand Logout { get; }
        public RelayCommand StartLog { get; }
        public RelayCommand ExitLog { get; }

        public string TypedName
        {
            get => _TypedName;
            set
            {
                _TypedName = value;
                NotifyPropertyChanged();
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

        public User[] ViewUsers => Users?.Where(x => x.Level < _NowUser.Level).ToArray();

        public void Save()
        {
            try
            {
                if (Users != null)
                {
                    File.WriteAllText("Users.json", JsonConvert.SerializeObject(Users), Encoding.UTF8);
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

        public sealed class User
        {
            public string Name { get; set; }
            public string Password { get; set; }
            public UserLevel Level { get; set; }
            public DateTime LastLoginTime { get; set; }
            public DateTime CreatedTime { get; set; }
        }

        public Authenticator_ViewModel()
        {
            Load();

            NowUser = Users.Count > 0 ? Users.OrderByDescending(x => x.LastLoginTime).First() : Guest;

            AddUser = new RelayCommand(e =>
                                       {
                                           if (Users.Count(x => x.Name == _TypedName) == 0)
                                           {
                                               Users.Add(new User { Name = _TypedName, Password = e as string, Level = _NowUser.Level - 1, CreatedTime = DateTime.Now });
                                               Save();
                                           }
                                       });

            RemoveUser = new RelayCommand(e =>
                                          {
                                              Users.RemoveAll(x => x.Name == _TypedName && x.Level < NowUser.Level);
                                              Save();
                                          });

            Login = new CommandWithResult<bool>(e =>
                                                {
                                                    if (e is PasswordBox password)
                                                    {
                                                        if (_TypedName == GP.Name && password.Password == GP.Password)
                                                        {
                                                            NowUser = GP;

                                                            return true;
                                                        }

                                                        if (Users != null && Users.Find(x => x.Name == _TypedName && x.Password == password.Password) is User _user)
                                                        {
                                                            NowUser = _user;

                                                            return true;
                                                        }

                                                        return false;
                                                    }

                                                    return false;
                                                }) { Result = true };


            Logout = new RelayCommand(e=>
                                      {
                                          Login.Result = true;
                                          TypedName = "";
                                          NowUser = Guest;

                                          if (e is PasswordBox password) password.Clear();
                                      });

            StartLog = new RelayCommand(e=>
                                        {
                                            Login.Result = true;
                                            TypedName = "";
                                            IsShown = Visibility.Visible;

                                            if (e is PasswordBox password) password.Clear();
                                        });

            ExitLog = new RelayCommand(e=>
                                       {
                                           Login.Result = true;
                                           TypedName = "";
                                           IsShown = Visibility.Collapsed;

                                           if (e is PasswordBox password) password.Clear();
                                       });
        }
    }
}