using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>
    /// 實作INotifyPropertyChanged的簡單抽象類別
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}