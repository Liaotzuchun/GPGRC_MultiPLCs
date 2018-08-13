using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>使實作INotifyPropertyChanged變成非常簡單的類別</summary>
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

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

        protected void Set<T>(T value, [CallerMemberName] string name = "")
        {
            _properties[name] = value;
            NotifyPropertyChanged(name);
        }

        protected void Set_WithOutNotify<T>(T value, [CallerMemberName] string name = "")
        {
            _properties[name] = value;
        }

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