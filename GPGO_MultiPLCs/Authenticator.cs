using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GPGO_MultiPLCs.Helpers;
using Newtonsoft.Json;

namespace GPGO_MultiPLCs
{
    public class Authenticator : ViewModelBase
    {
        public enum UserLevel
        {
            C,
            B,
            A,
            S
        }

        private readonly User GP = new User { Name = "GP", Password = "23555277", Level = UserLevel.S };

        private List<User> Users;
        private string _TypedName;
        private string _TypedPassword;
        private User _NowUser;

        public RelayCommand AddUser { get; }
        public RelayCommand RemoveUser { get; }
        public RelayCommand Login { get; }

        public string TypedName
        {
            get => _TypedName;
            set
            {
                _TypedName = value;
                NotifyPropertyChanged();
            }
        }

        public string TypedPassword
        {
            get => _TypedPassword;
            set
            {
                _TypedPassword = value;
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

        private User[] ViewUsers => Users?.Where(x => x.Level < _NowUser.Level).ToArray();

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
            public string Name;
            public string Password;
            public UserLevel Level;
            public DateTime LastLoginTime;
            public DateTime CreatedTime;
        }

        public Authenticator()
        {
            Load();

            NowUser = Users.Count > 0 ? Users.OrderByDescending(x => x.LastLoginTime).First() : GP;

            AddUser = new RelayCommand(e =>
                                       {
                                           if (Users.Count(x => x.Name == _TypedName) == 0)
                                           {
                                               Users.Add(new User { Name = _TypedName, Password = _TypedPassword, Level = _NowUser.Level - 1, CreatedTime = DateTime.Now });
                                               Save();
                                           }
                                       });

            RemoveUser = new RelayCommand(e =>
                                          {
                                              Users.RemoveAll(x => x.Name == _TypedName && x.Level < NowUser.Level);
                                              Save();
                                          });

            Login = new RelayCommand(e =>
                                     {
                                         if (Users != null && Users.Find(x => x.Name == _TypedName && x.Password == _TypedPassword) is User _user)
                                         {
                                             NowUser = _user;
                                         }
                                     });
        }
    }
}