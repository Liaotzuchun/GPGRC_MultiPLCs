using GPMVVM.PLCService;
using System.Collections.Generic;

namespace GPGO_MultiPLCs.Models
{
    public class PLC_DevicesMap
    {
        public Dictionary<DataNames, (DataType, int)> DataList;
        public Dictionary<DataNames, (DataType, int)> DataList_Ext;
        public Dictionary<DataNames, (DataType, int)> RecipeList;
        public Dictionary<SignalNames, (BitType, int)> SignalList;
        public Dictionary<SignalNames, (BitType, int)> SignalList_Ext;

        public PLC_DevicesMap(Dictionary<SignalNames, (BitType, int)> _SignalList, Dictionary<DataNames, (DataType, int)> _DataList, Dictionary<DataNames, (DataType, int)> _RecipeList)
        {
            SignalList = _SignalList;
            DataList = _DataList;
            RecipeList = _RecipeList;
        }

        public PLC_DevicesMap(Dictionary<SignalNames, (BitType, int)> _SignalList, Dictionary<DataNames, (DataType, int)> _DataList, Dictionary<DataNames, (DataType, int)> _RecipeList, Dictionary<SignalNames, (BitType, int)> _SignalList_Ext)
        {
            SignalList = _SignalList;
            DataList = _DataList;
            RecipeList = _RecipeList;
            SignalList_Ext = _SignalList_Ext;
        }

        public PLC_DevicesMap(Dictionary<SignalNames, (BitType, int)> _SignalList,
                              Dictionary<DataNames, (DataType, int)> _DataList,
                              Dictionary<DataNames, (DataType, int)> _RecipeList,
                              Dictionary<SignalNames, (BitType, int)> _SignalList_Ext,
                              Dictionary<DataNames, (DataType, int)> _DataList_Ext)
        {
            SignalList = _SignalList;
            DataList = _DataList;
            RecipeList = _RecipeList;
            SignalList_Ext = _SignalList_Ext;
            DataList_Ext = _DataList_Ext;
        }
    }
}