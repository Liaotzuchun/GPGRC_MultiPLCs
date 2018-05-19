using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Models
{
    public enum SignalNames
    {
        程式結束,
        加熱門未關,
        緊急停止,
        OTP超溫異常,
        循環風車過載,
        超溫警報,
        停止後未開門,
        循環風車INV異常,
        升恆溫逾時
    }

    public enum DataNames
    {
        目標溫度_1,
        目標溫度_2,
        目標溫度_3,
        目標溫度_4,
        目標溫度_5,
        目標溫度_6,
        目標溫度_7,
        目標溫度_8,
        升溫時間_1,
        升溫時間_2,
        升溫時間_3,
        升溫時間_4,
        升溫時間_5,
        升溫時間_6,
        升溫時間_7,
        升溫時間_8,
        恆溫溫度_1,
        恆溫溫度_2,
        恆溫溫度_3,
        恆溫溫度_4,
        恆溫溫度_5,
        恆溫溫度_6,
        恆溫溫度_7,
        恆溫溫度_8,
        恆溫時間_1,
        恆溫時間_2,
        恆溫時間_3,
        恆溫時間_4,
        恆溫時間_5,
        恆溫時間_6,
        恆溫時間_7,
        恆溫時間_8,
        降溫溫度,
        使用段速
    }
}
