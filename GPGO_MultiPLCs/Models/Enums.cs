namespace GPGRC_MultiPLCs.Models;

public enum RecipeState
{
    新增,
    修改,
    刪除
}

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
    待命 = 1,
    運轉中 = 2, //自動啟動
    錯誤 = 3,   //設備異常
    手動模式 = 5,
    自動模式 = 6,
    未知 = 7
}

public enum SetRecipeResult
{
    成功,
    條件不允許,
    PLC錯誤,
    比對不相符,
    無需變更
}
public enum RCIndex
{
    RC1,
    RC2,
    RC3
}