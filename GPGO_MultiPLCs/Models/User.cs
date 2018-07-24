using System;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>
    /// 使用者帳號
    /// </summary>
    public sealed class User : ViewModelBase
    {
        /// <summary>
        /// 使用者階層
        /// </summary>
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

        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime CreatedTime
        {
            get => _CreatedTime;
            set
            {
                _CreatedTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 最後登入時間
        /// </summary>
        public DateTime LastLoginTime
        {
            get => _LastLoginTime;
            set
            {
                _LastLoginTime = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 階層
        /// </summary>
        public UserLevel Level
        {
            get => _Level;
            set
            {
                _Level = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// 密碼
        /// </summary>
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