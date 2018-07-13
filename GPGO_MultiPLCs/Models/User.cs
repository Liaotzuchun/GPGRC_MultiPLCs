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

        private DateTime _CreatedTime;
        private DateTime _LastLoginTime;
        private UserLevel _Level;

        private string _Name;
        private string _Password;

        public DateTime CreatedTime
        {
            get => _CreatedTime;
            set
            {
                _CreatedTime = value;
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

        public UserLevel Level
        {
            get => _Level;
            set
            {
                _Level = value;
                NotifyPropertyChanged();
            }
        }

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
    }
}