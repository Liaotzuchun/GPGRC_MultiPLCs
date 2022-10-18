namespace GPGO_MultiPLCs.Models;

public enum RecordState
{
    待命,
    升溫中,
    恆溫中,
    降溫中
}

public enum CodeType
{
    Panel,
    SubPanel,
    JobNo
}

public enum Status
{
    離線 = -1, //PLC斷線
    未知,
    運轉中 = 1, //自動啟動
    待命  = 2,  //手動模式
    準備中,     //手動模式
    維修 = 8,   //NOTE 沒有
    停止 = 16,  //自動停止
    錯誤 = 4    //設備異常
}

public enum SetRecipeResult
{
    成功,
    條件不允許,
    PLC錯誤,
    比對錯誤
}