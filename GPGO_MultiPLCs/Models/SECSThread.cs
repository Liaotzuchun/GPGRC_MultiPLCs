using GP_SECS_GEM;
using QGACTIVEXLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GPMVVM.PooledCollections;

namespace GPGO_MultiPLCs.Models;

public class SECSThread
{
    public enum PPStatus
    {
        Create = 1,
        Change = 2,
        Delete = 3
    }

    public enum ITRI_SV
    {
        GEM_PREVIOUS_PROCESS_STATE = 14,
        GEM_PROCESS_STATE          = 15,
        GEM_PP_EXEC_NAME           = 42
    }

    private readonly GOSECS  secsGem;
    private readonly EqpBase eqpBase;

    /// <summary> GPSECS服務設定檔案</summary>
    public SECSParameterSet SECSParameterSet { get; }

    public event Action<string>                                                                         TerminalMessage;
    public event Action<int, string, object>                                                            ECChange;
    public event Func<PLC_Recipe, bool>                                                                 UpsertRecipe;
    public event Func<string, ValueTask<bool>>                                                          DeleteRecipe;
    public event Func<int, string, HCACKValule>                                                         SetRecipe;
    public event Func<int, HCACKValule>                                                                 Start;
    public event Func<int, HCACKValule>                                                                 Stop;
    public event Func<int, (string LotID, string PartID, int layer, IList<string> Panels), HCACKValule> AddLOT;
    public event Func<int, HCACKValule>                                                                 CANCEL;
    public event Action<bool>                                                                           ONLINE_Changed;
    public event Action<bool>                                                                           CommEnable_Changed;
    public event Action<bool>                                                                           Communicating_Changed;
    public event Action                                                                                 GO_Local;
    public event Action                                                                                 GO_Remote;
    public event Func<string, HCACKValule>                                                              RetrieveLotData;

    public QGWrapper GemCore => secsGem?.AxQGWrapper;

    public void SendTerminalMessage(string message)
    {
        secsGem?.AxQGWrapper.SendTerminalMessage(message);
    }

    public void TerminalMessageConfirm()
    {
        secsGem?.AxQGWrapper.EventReportSend(21);
    }

    public void UpdateITRISV(ITRI_SV name, object value)
    {
        secsGem?.AxQGWrapper.UpdateSV((int)name, value);
    }

    public void UpdateSV(string name, object value)
    {
        if (secsGem != null && eqpBase != null)
        {
            secsGem.GemSVDataUpdateNew(eqpBase.EqpSVViewModel, name, value);
        }
    }

    public void UpdateDV(string name, object value)
    {
        if (secsGem == null)
        {
            return;
        }

        switch (name)
        {
            case "GemPPChangeName":
                secsGem.AxQGWrapper.UpdateSV(9, value);
                break;
            case "GemPPChangeStatus":
                secsGem.AxQGWrapper.UpdateSV(10, value);
                break;
            default:
                if (eqpBase != null)
                {
                    secsGem.GemDVDataUpdateNew(eqpBase.EqpDVViewModel, name, value);
                }

                break;
        }
    }

    public void UpdateEC(string name, object value)
    {
        if (eqpBase?.EqpECViewModel.DataCollection.FirstOrDefault(o => o.Name.Equals(name)) is {} ec && int.TryParse(ec.ID, out var ECID))
        {
            secsGem?.AxQGWrapper.UpdateEC(ECID, value);
        }
    }

    public void InvokeEvent(string name)
    {
        if (name == "GemProcessProgramChange")
        {
            secsGem?.AxQGWrapper.EventReportSend(3);
        }
        else if (eqpBase?.EqpEventViewModel.DataCollection.FirstOrDefault(o => o.Name.Equals(name)) is {} ce && int.TryParse(ce.ID, out var CEID))
        {
            secsGem?.AxQGWrapper.EventReportSend(CEID);
        }
    }

    public void InvokeAlarm(string name, bool val)
    {
        if (eqpBase?.EqpAlarmViewModel.DataCollection.FirstOrDefault(o => o.Name.Equals(name)) is {} ae && int.TryParse(ae.ID, out var ALID))
        {
            secsGem?.AxQGWrapper.AlarmReportSend(ALID, val ? 255 : 0);
        }
    }

    public bool Enable(bool val)
    {
        if (secsGem != null)
        {
            if (val && secsGem.AxQGWrapper.EnableComm() == 0)
            {
                secsGem.AxQSWrapper.Start(); //! 啟用通訊：AxQGWrapper.EnableComm() → AxQSWrapper.Start()
                return true;
            }

            if (!val && secsGem.AxQGWrapper.DisableComm() == 0)
            {
                secsGem.AxQSWrapper.Stop(); //! 中斷通訊：AxQGWrapper.DisableComm() → AxQSWrapper.Stop()
                return true;
            }
        }

        return false;
    }

    public bool Online(bool val) =>
        secsGem != null &&
        (val && secsGem.AxQGWrapper.OnLineRequest() == 0 || !val && secsGem.AxQGWrapper.OffLine() == 0);

    public bool Remote(bool val) => secsGem != null && (val && secsGem.AxQGWrapper.OnLineRemote() == 0 || !val && secsGem.AxQGWrapper.OnLineLocal() == 0);

    private void SetEqpBase()
    {
        var alarmpath = $"{SECSParameterSet.SECSParameter.FilePath}\\EqpInitData\\EqpAlarm.csv";
        var dvpath    = $"{SECSParameterSet.SECSParameter.FilePath}\\EqpInitData\\EqpDV.csv";
        var ecpath    = $"{SECSParameterSet.SECSParameter.FilePath}\\EqpInitData\\EqpEC.csv";
        var eventpath = $"{SECSParameterSet.SECSParameter.FilePath}\\EqpInitData\\EqpEvent.csv";
        var svpath    = $"{SECSParameterSet.SECSParameter.FilePath}\\EqpInitData\\EqpSV.csv";

        try
        {
            var alarms = File.ReadAllLines(alarmpath, Encoding.UTF8);
            foreach (var alarm in alarms)
            {
                var cols = alarm.Split(',');
                if (cols.Length < 7 || !int.TryParse(cols[1], out _))
                {
                    continue;
                }

                var _alarm = new EqpAlarmClass
                             {
                                 Name    = cols[0],
                                 ID      = cols[1],
                                 CD      = cols[2],
                                 Enabled = cols[3],
                                 Text    = cols[4],
                                 Trigger = cols[5],
                                 Down    = cols[6]
                             };

                eqpBase.EqpAlarmViewModel.DataCollection.Add(_alarm);
            }

            var dvs = File.ReadAllLines(dvpath, Encoding.UTF8);
            foreach (var dv in dvs)
            {
                var cols = dv.Split(',');
                if (cols.Length < 6 || !int.TryParse(cols[1], out _))
                {
                    continue;
                }

                var _dv = new EqpDVClass
                          {
                              Name       = cols[0],
                              ID         = cols[1],
                              Type       = cols[2],
                              Length     = cols[3],
                              Unit       = cols[4],
                              Definition = cols[5]
                          };

                eqpBase.EqpDVViewModel.DataCollection.Add(_dv);
            }

            var ecs = File.ReadAllLines(ecpath, Encoding.UTF8);
            foreach (var ec in ecs)
            {
                var cols = ec.Split(',');
                if (cols.Length < 10 || !int.TryParse(cols[1], out _))
                {
                    continue;
                }

                var _ec = new EqpECClass
                          {
                              Name         = cols[0],
                              ID           = cols[1],
                              Type         = cols[2],
                              MinValue     = cols[3],
                              MaxValue     = cols[4],
                              DefaultValue = cols[5],
                              Unit         = cols[6],
                              Definition   = cols[7],
                              Trigger      = cols[8],
                              Write        = cols[9]
                          };

                eqpBase.EqpECViewModel.DataCollection.Add(_ec);
            }

            var events = File.ReadAllLines(eventpath, Encoding.UTF8);
            foreach (var _event in events)
            {
                var cols = _event.Split(',');
                if (cols.Length < 4 || !int.TryParse(cols[1], out _))
                {
                    continue;
                }

                var __event = new EqpEventClass
                              {
                                  Name       = cols[0],
                                  ID         = cols[1],
                                  Definition = cols[2],
                                  Trigger    = cols[3]
                              };

                eqpBase.EqpEventViewModel.DataCollection.Add(__event);
            }

            var svs = File.ReadAllLines(svpath, Encoding.UTF8);
            foreach (var sv in svs)
            {
                var cols = sv.Split(',');
                if (cols.Length < 7 || !int.TryParse(cols[1], out _))
                {
                    continue;
                }

                var _sv = new EqpSVClass
                          {
                              Name       = cols[0],
                              ID         = cols[1],
                              Type       = cols[2],
                              Length     = cols[3],
                              Unit       = cols[4],
                              Definition = cols[5],
                              Trigger    = cols[6]
                          };

                eqpBase.EqpSVViewModel.DataCollection.Add(_sv);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
    }

    public SECSThread(int index)
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version;

        SECSParameterSet = new SECSParameterSet
                           {
                               SECSParameter =
                               {
                                   FilePath = $"C:\\ITRIinit\\{index}", //設定檔存放位置
                                   MDLN     = "GP_GO",
                                   SOFTREV  = $"{v.Major}.{v.Minor}.{v.Build}"
                               }
                           };

        secsGem = new GOSECS(SECSParameterSet.SECSParameter);

        eqpBase = new EqpBase
                  {
                      StationNO = $"{index}",
                      EqpAlarmViewModel = new EqpAlarmViewModel
                                          {
                                              DataCollection = new ObservableCollection<EqpAlarmClass>()
                                          },
                      EqpDVViewModel = new EqpDVViewModel
                                       {
                                           DataCollection = new ObservableCollection<EqpDVClass>()
                                       },
                      EqpECViewModel = new EqpECViewModel
                                       {
                                           DataCollection = new ObservableCollection<EqpECClass>()
                                       },
                      EqpEventViewModel = new EqpEventViewModel
                                          {
                                              DataCollection = new ObservableCollection<EqpEventClass>()
                                          },
                      EqpSVViewModel = new EqpSVViewModel
                                       {
                                           DataCollection = new ObservableCollection<EqpSVClass>()
                                       }
                  };

        SetEqpBase();

        secsGem.TerminalMessageEvent += message =>
                                        {
                                            TerminalMessage?.Invoke(message);
                                        };

        secsGem.ECChangeEvent += e =>
                                 {
                                     var (ECID, Value) = e;
                                     if (int.TryParse(ECID.Split('_').Last(), out var i))
                                     {
                                         ECChange?.Invoke(i, ECID, Value);
                                     }
                                 };

        secsGem.ECChangeEndEvent += i =>
                                    {
                                    };

        secsGem.InsertPPEvent += recipe => UpsertRecipe != null && UpsertRecipe.Invoke(recipe);

        secsGem.DeletePPEvent += recipeName =>
                                 {
                                     DeleteRecipe?.Invoke(recipeName);
                                 };
        //S2F41
        secsGem.ADDLOTCommand += r =>
                                 {
                                     if (r.RemoteCommandParameter.Count < 5)
                                     {
                                         return HCACKValule.ParameterInvalid;
                                     }

                                     if (r.RemoteCommandParameter[0].CPVAL.ObjectData is int[] { Length: > 0 } indexes &&
                                         r.RemoteCommandParameter[1].CPVAL.ObjectData is string lot                    &&
                                         r.RemoteCommandParameter[2].CPVAL.ObjectData is string part                   &&
                                         r.RemoteCommandParameter[3].CPVAL.ObjectData is int[] layers                  &&
                                         r.RemoteCommandParameter[4].CPVAL is SECSMessageBranches Branches)
                                     {
                                         var       i      = indexes[0];
                                         var       l      = layers[0];
                                         using var panels = Branches.SECSMessageObjects.Select(x => x.ObjectData?.ToString() ?? string.Empty).ToPooledList();

                                         return AddLOT?.Invoke(i, (lot, part, l, panels)) ?? HCACKValule.ParameterInvalid;
                                     }

                                     return HCACKValule.CantPerform;
                                 };

        secsGem.CANCELCommand += r =>
                                 {
                                     if (r.RemoteCommandParameter.Count < 1)
                                     {
                                         return HCACKValule.ParameterInvalid;
                                     }

                                     if (r.RemoteCommandParameter[0].CPVAL.ObjectData is int[] { Length: > 0 } indexes)
                                     {
                                         var i = indexes[0];

                                         return CANCEL?.Invoke(i) ?? HCACKValule.ParameterInvalid;
                                     }

                                     return HCACKValule.CantPerform;
                                 };
        secsGem.PP_SELECTCommand += r =>
                                    {
                                        if (r.RemoteCommandParameter.Count < 2)
                                        {
                                            return HCACKValule.ParameterInvalid;
                                        }

                                        if (r.RemoteCommandParameter[0].CPVAL.ObjectData is int[] o && int.TryParse(o[0].ToString(), out var i))
                                        {
                                            return SetRecipe?.Invoke(i, r.RemoteCommandParameter[1].CPVAL.ObjectData.ToString()) ?? HCACKValule.ParameterInvalid;
                                        }

                                        return HCACKValule.CantPerform;
                                    };

        secsGem.STARTCommand += r =>
                                {
                                    if (r.RemoteCommandParameter.Count < 1)
                                    {
                                        return HCACKValule.ParameterInvalid;
                                    }

                                    if (r.RemoteCommandParameter[0].CPVAL.ObjectData is int[] o && int.TryParse(o[0].ToString(), out var i))
                                    {
                                        return Start?.Invoke(i) ?? HCACKValule.ParameterInvalid;
                                    }

                                    return HCACKValule.CantPerform;
                                };

        secsGem.STOPCommand += r =>
                               {
                                   if (r.RemoteCommandParameter.Count < 1)
                                   {
                                       return HCACKValule.ParameterInvalid;
                                   }

                                   if (r.RemoteCommandParameter[0].CPVAL.ObjectData is int[] o && int.TryParse(o[0].ToString(), out var i))
                                   {
                                       return Stop?.Invoke(i) ?? HCACKValule.ParameterInvalid;
                                   }

                                   return HCACKValule.CantPerform;
                               };

        secsGem.RetrieveLotDataCommand += r =>
                                          {
                                              if (r.RemoteCommandParameter.Count < 1)
                                              {
                                                  return HCACKValule.ParameterInvalid;
                                              }

                                              if (r.RemoteCommandParameter[0].CPVAL.ObjectData is string lotid && !string.IsNullOrEmpty(lotid))
                                              {
                                                  return RetrieveLotData?.Invoke(lotid) ?? HCACKValule.ParameterInvalid;
                                              }

                                              return HCACKValule.CantPerform;
                                          };

        secsGem.SECSCommunicationControlViewModel.PropertyChanged += (s, e) =>
                                                                     {
                                                                         var vm = (SECSCommunicationControlViewModel)s;

                                                                         switch (e.PropertyName)
                                                                         {
                                                                             case nameof(GP_GEM.SECSCommunicationControlViewModel.CommunicatioinState):
                                                                                 CommEnable_Changed?.Invoke(vm.CommunicatioinState    != (int)COMM_STATE.DISABLE);
                                                                                 Communicating_Changed?.Invoke(vm.CommunicatioinState == (int)COMM_STATE.COMMUNICATING);

                                                                                 break;
                                                                             case nameof(GP_GEM.SECSCommunicationControlViewModel.IsOnLine):
                                                                                 ONLINE_Changed?.Invoke(vm.IsOnLine);
                                                                                 break;
                                                                             case nameof(GP_GEM.SECSCommunicationControlViewModel.IsRemote):
                                                                                 if (vm.IsRemote)
                                                                                 {
                                                                                     GO_Remote?.Invoke();
                                                                                 }

                                                                                 break;
                                                                             case nameof(GP_GEM.SECSCommunicationControlViewModel.IsLocal):
                                                                                 if (vm.IsLocal)
                                                                                 {
                                                                                     GO_Local?.Invoke();
                                                                                 }

                                                                                 break;
                                                                         }
                                                                     };
    }
}