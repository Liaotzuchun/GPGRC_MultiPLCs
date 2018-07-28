﻿namespace GPGO_MultiPLCs.Models
{
    /// <summary>PLC訊號</summary>
    public enum SignalNames
    {
        //! PLC->PC
        //! 警報/提示
        降溫中,
        程式結束,
        加熱門未關,
        緊急停止,
        溫控器低溫異常,
        電源反相,
        OTP超溫異常,
        循環風車過載,
        冷卻進氣風車異常,
        超溫警報,
        停止後未開門,
        循環風車INV異常,
        充氮氣逾時,
        門未關定位異常,
        升恆溫逾時,

        //! 狀態
        手動模式,
        自動模式,
        自動停止,
        自動啟動,
        PC_ByPass,

        //! PC->PLC
        PC_HeartBeat
    }

    /// <summary>PLC資料</summary>
    public enum DataNames
    {
        //! PLC->PC
        爐內溫度_1,
        爐內溫度_2,
        爐內溫度_3,
        爐內溫度_4,
        爐內溫度_5,
        爐內溫度_6,
        爐內溫度_7,
        爐內溫度_8,
        溫控器溫度,
        片段剩餘時間,
        總剩餘時間,
        目前段數,

        //! PC->PLC
        配方名稱_01,
        配方名稱_02,
        配方名稱_03,
        配方名稱_04,
        配方名稱_05,
        配方名稱_06,
        配方名稱_07,
        配方名稱_08,
        配方名稱_09,
        配方名稱_10,
        配方名稱_11,
        配方名稱_12,
        配方名稱_13,
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
        充氣時間,
        使用段數
    }
}