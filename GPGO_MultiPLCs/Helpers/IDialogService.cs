using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public interface IDialogService<T>
    {
        Task<(bool result, T intput)> ShowWithIntput(string msg);

        Task<(bool result, T intput)> ShowWithIntput(string msg, Func<T, (bool result, string error_msg)> condition_fun);

        Task<bool> Show(string msg, bool support_cancel);

        Task Show(string msg, TimeSpan delay);
    }
}