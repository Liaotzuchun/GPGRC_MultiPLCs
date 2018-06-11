using System;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public interface IDialogService<T>
    {
        Task<bool> Show(string msg, bool support_cancel);

        Task Show(string msg, TimeSpan delay);
        Task<(bool result, T intput)> ShowWithIntput(string msg, string header);

        Task<(bool result, T intput)> ShowWithIntput(string msg, string herder, Func<T, (bool result, string title_msg)> condition_fun);
    }
}