using System.Collections.Generic;

namespace GPGO_MultiPLCs.Models
{
    public class PLC_DevicesMap
    {
        public Dictionary<DataNames, int> DataList;
        public Dictionary<DataNames, int> DataList_Ext;
        public Dictionary<DataNames, int> RecipeList;
        public Dictionary<SignalNames, int> SignalList;
        public Dictionary<SignalNames, int> SignalList_Ext;

        public PLC_DevicesMap(Dictionary<SignalNames, int> _SignalList, Dictionary<DataNames, int> _DataList, Dictionary<DataNames, int> _RecipeList)
        {
            SignalList = _SignalList;
            DataList = _DataList;
            RecipeList = _RecipeList;
        }

        public PLC_DevicesMap(Dictionary<SignalNames, int> _SignalList, Dictionary<DataNames, int> _DataList, Dictionary<DataNames, int> _RecipeList, Dictionary<SignalNames, int> _SignalList_Ext)
        {
            SignalList = _SignalList;
            DataList = _DataList;
            RecipeList = _RecipeList;
            SignalList_Ext = _SignalList_Ext;
        }

        public PLC_DevicesMap(Dictionary<SignalNames, int> _SignalList,
                              Dictionary<DataNames, int> _DataList,
                              Dictionary<DataNames, int> _RecipeList,
                              Dictionary<SignalNames, int> _SignalList_Ext,
                              Dictionary<DataNames, int> _DataList_Ext)
        {
            SignalList = _SignalList;
            DataList = _DataList;
            RecipeList = _RecipeList;
            SignalList_Ext = _SignalList_Ext;
            DataList_Ext = _DataList_Ext;
        }
    }
}