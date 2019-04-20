using System;
using System.ComponentModel;

namespace GPGO_MultiPLCs.Helpers
{
    public interface IRecipe<T> : INotifyPropertyChanged, IEquatable<T> where T : class, new()
    {
        string RecipeName { get; set; }
        DateTime Updated { get; set; }
        string Editor { get; set; }
        T Copy(string user);
        void CopyValue(string user, T recipe);
    }
}
