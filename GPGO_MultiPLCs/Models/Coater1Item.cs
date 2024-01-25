using System.ComponentModel;
using GPMVVM.Helpers;

namespace GP_GRC.Models;
public class CoaterItem : ObservableObject, INotifyPropertyChanged
{
    public int Num
    {
        get => Get<int>();
        set => Set(value);
    }
    public string PanelName
    {
        get => Get<string>();
        set => Set(value);
    }
}
