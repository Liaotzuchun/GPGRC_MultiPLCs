using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public interface IDialogService
    {
        Task<bool> Show(string msg, bool support_cancel);

        Task Show(string msg, TimeSpan delay);
    }
}
