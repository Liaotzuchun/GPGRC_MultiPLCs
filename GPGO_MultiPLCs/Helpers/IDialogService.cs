using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>
    /// 訊息類型
    /// </summary>
    public enum DialogMsgType
    {
        /// <summary>
        /// 一般訊息
        /// </summary>
        Normal,
        /// <summary>
        /// 警告訊息
        /// </summary>
        Alert,
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        Alarm
    }

    /// <summary>
    /// 可供顯示對話窗口的基礎介面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDialogService<T>
    {
        /// <summary>
        /// 顯示確認訊息視窗
        /// </summary>
        /// <param name="msg">顯示訊息</param>
        /// <param name="support_cancel">是否顯示取消鍵</param>
        /// <param name="type">訊息類型</param>
        /// <returns></returns>
        Task<bool> Show(Dictionary<GlobalTempSettings.Language, string> msg, bool support_cancel, DialogMsgType type = DialogMsgType.Normal);

        /// <summary>
        /// 顯示提示訊息(指定時間內消失)
        /// </summary>
        /// <param name="msg">顯示訊息</param>
        /// <param name="delay">存留時間</param>
        /// <param name="type">訊息類型</param>
        /// <returns></returns>
        Task Show(Dictionary<GlobalTempSettings.Language, string> msg, TimeSpan delay, DialogMsgType type = DialogMsgType.Normal);

        /// <summary>
        /// 顯示OP可輸入的訊息對話視窗
        /// </summary>
        /// <param name="msg">顯示訊息</param>
        /// <param name="header">顯示標題</param>
        /// <returns>OP輸入的訊息</returns>
        Task<(bool result, T intput)> ShowWithIntput(Dictionary<GlobalTempSettings.Language, string> msg, Dictionary<GlobalTempSettings.Language, string> header);

        /// <summary>
        /// 顯示OP可輸入的訊息對話視窗，並可依據條件比對回傳比對結果
        /// </summary>
        /// <param name="msg">顯示訊息</param>
        /// <param name="header">標題訊息</param>
        /// <param name="condition_fun">條件委派</param>
        /// <returns>OP輸入的訊息和條件比對結果</returns>
        Task<(bool result, T intput)> ShowWithIntput(Dictionary<GlobalTempSettings.Language, string> msg,
                                                     Dictionary<GlobalTempSettings.Language, string> header,
                                                     Func<T, (bool result, Dictionary<GlobalTempSettings.Language, string> title_msg)> condition_fun);
    }
}