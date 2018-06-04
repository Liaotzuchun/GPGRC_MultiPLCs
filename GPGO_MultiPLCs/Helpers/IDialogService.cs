using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public interface IDialogService<T>
    {
        Task<(bool result, T intput)> ShowWithIntput(string msg);

        Task<(bool result, T intput)> ShowWithIntput(string msg, T condition);

        Task<(bool result, T intput)> ShowWithIntput(string msg, IEnumerable<T> conditions);

        Task<bool> Show(string msg, bool support_cancel);

        Task Show(string msg, TimeSpan delay);
    }
}