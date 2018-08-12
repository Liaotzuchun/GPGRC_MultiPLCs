using System;
using GPGO_MultiPLCs.Helpers;

namespace GPGO_MultiPLCs.Models
{
    /// <summary>使用者帳號</summary>
    public sealed class User : BindableBase
    {
        /// <summary>使用者階層</summary>
        public enum UserLevel
        {
            D, //Guest
            C, //Operator
            B, //Leader
            A, //Manager
            S //GP
        }

        /// <summary>創建時間</summary>
        public DateTime CreatedTime
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        /// <summary>最後登入時間</summary>
        public DateTime LastLoginTime
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        /// <summary>階層</summary>
        public UserLevel Level
        {
            get => Get<UserLevel>();
            set => Set(value);
        }

        /// <summary>名稱</summary>
        public string Name
        {
            get => Get<string>();
            set => Set(value);
        }

        /// <summary>密碼</summary>
        public string Password
        {
            get => Get<string>();
            set => Set(value);
        }
    }
}