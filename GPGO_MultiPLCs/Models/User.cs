using System;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    public sealed class User : ViewModelBase
    {
        public enum UserLevel
        {
            D, //Guest
            C, //Operator
            B, //Leader
            A, //Manager
            S //GP
        }

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
}