using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.Helpers
{
    public enum DialogMsgType
    {
        Normal,
        Alert,
        Alarm
    }

    public interface IDialogService<T>
    {
        Task<bool> Show(Dictionary<GlobalTempSettings.Language, string> msg, bool support_cancel, DialogMsgType type = DialogMsgType.Normal);
        Task Show(Dictionary<GlobalTempSettings.Language, string> msg, TimeSpan delay, DialogMsgType type = DialogMsgType.Normal);
        Task<(bool result, T intput)> ShowWithIntput(Dictionary<GlobalTempSettings.Language, string> msg, Dictionary<GlobalTempSettings.Language, string> header);

        Task<(bool result, T intput)> ShowWithIntput(Dictionary<GlobalTempSettings.Language, string> msg,
                                                     Dictionary<GlobalTempSettings.Language, string> herder,
                                                     Func<T, (bool result, Dictionary<GlobalTempSettings.Language, string> title_msg)> condition_fun);
    }
}