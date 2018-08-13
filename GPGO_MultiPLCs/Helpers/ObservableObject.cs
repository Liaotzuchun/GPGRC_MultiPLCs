using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>使實作INotifyPropertyChanged變成非常簡單的類別</summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        /// <summary>取得欄位值</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">欄位名稱</param>
        /// <returns></returns>
        protected T Get<T>([CallerMemberName] string name = "")
        {
            if (_properties.TryGetValue(name, out var value))
            {
                return value == null ? default(T) : (T)value;
            }

            return default(T);
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>設定欄位值</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">設定值</param>
        /// <param name="name">欄位名稱</param>
        protected void Set<T>(T value, [CallerMemberName] string name = "")
        {
            _properties[name] = value;
            NotifyPropertyChanged(name);
        }

        /// <summary>設定欄位值，但不產生值變通知</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">設定值</param>
        /// <param name="name">欄位名稱</param>
        protected void Set_WithOutNotify<T>(T value, [CallerMemberName] string name = "")
        {
            _properties[name] = value;
        }

        /// <summary>設定欄位值，當值相同時不更改值也不通知</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">設定值</param>
        /// <param name="name">欄位名稱</param>
        protected void Set_WithOutNotifyWhenEquals<T>(T value, [CallerMemberName] string name = "")
        {
            if (Equals(value, Get<T>(name)))
            {
                return;
            }

            _properties[name] = value;
            NotifyPropertyChanged(name);
        }
    }
}