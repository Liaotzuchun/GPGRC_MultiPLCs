﻿using GP_SECS_GEM;
using QSACTIVEXLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using QGACTIVEXLib;

namespace GPGO_MultiPLCs.Models
{
    public class SECSThread
    {
        public enum PPStatus
        {
            Create = 1,
            Change = 2,
            Delete = 3
        }

        private GOSECS  secsGem;
        private EqpBase eqpBase;
        /// <summary> GPSECS服務設定檔案</summary>
        public SECSParameterSet SECSParameterSet
        {
            get;
            private set;
        }

        public event Action<string>                                                              TerminalMessage;
        public event Action<int, string, object>                                                 ECChange;
        public event Func<PLC_Recipe, bool>                                                      UpsertRecipe;
        public event Action<string>                                                                     DeleteRecipe;
        public event Func<int, string, HCACKValule>                                              SetRecipe;
        public event Func<int, HCACKValule>                                                      Start;
        public event Func<int, HCACKValule>                                                      Stop;
        public event Func<int, (string LotID, string PartID, IList<string> Panels), HCACKValule> AddLOT;
        public event Func<int, string, ValueTask<object>, HCACKValule>                           GetLOTInfo;
        public event Action<bool>                                                                ONLINE_Changed;
        public event Action<bool>                                                                CommEnable_Changed;
        public event Action<bool>                                                                Communicating_Changed;
        public event Action                                                                      GO_Local;
        public event Action                                                                      GO_Remote;

        public readonly Thread     thread;
        public          Dispatcher dp;

        public QGWrapper GemCore => secsGem?.AxQGWrapper;

        public void UpdateSV(string name, object value)
        {
            dp?.InvokeAsync(() =>
                            {
                                var a = secsGem?.GemSVDataUpdateNew(eqpBase.EqpSVViewModel, name, value);
                            });
        }

        public void UpdateDV(string name, object value)
        {
            dp?.InvokeAsync(() =>
                            {
                                switch (name)
                                {
                                    case "GemPPChangeName":
                                        secsGem?.AxQGWrapper.UpdateSV(9, value);
                                        break;
                                    case "GemPPChangeStatus":
                                        secsGem?.AxQGWrapper.UpdateSV(10, value);
                                        break;
                                    default:
                                        secsGem?.GemDVDataUpdateNew(eqpBase.EqpDVViewModel, name, value);
                                        break;
                                }
                            });
        }

        public void UpdateEC(string name, object value)
        {
            dp?.InvokeAsync(() =>
                            {
                                if (eqpBase?.EqpECViewModel.DataCollection.FirstOrDefault(o => o.Name.Equals(name)) is EqpECClass ec && int.TryParse(ec.ID, out var ECID))
                                {
                                    secsGem?.AxQGWrapper.UpdateEC(ECID, value);
                                }
                            });
        }

        public void InvokeEvent(string name)
        {
            dp?.InvokeAsync(() =>
                            {
                                if (name == "GemProcessProgramChange")
                                {
                                    secsGem?.AxQGWrapper.EventReportSend(9);
                                }
                                else if (eqpBase?.EqpEventViewModel.DataCollection.FirstOrDefault(o => o.Name.Equals(name)) is EqpEventClass ce && int.TryParse(ce.ID, out var CEID))
                                {
                                    secsGem?.AxQGWrapper.EventReportSend(CEID);
                                }
                            });
        }

        public void InvokeAlarm(string name, bool val)
        {
            dp?.InvokeAsync(() =>
                            {
                                if (eqpBase?.EqpAlarmViewModel.DataCollection.FirstOrDefault(o => o.Name.Equals(name)) is EqpAlarmClass ae && int.TryParse(ae.ID, out var ALID))
                                {
                                    secsGem?.AxQGWrapper.AlarmReportSend(ALID, val ? 255 : 0);
                                }
                            });
        }

        public async Task<bool> Enable(bool val)
        {
            if (dp == null)
            {
                return false;
            }

            var result = false;
            await dp.InvokeAsync(() =>
                                 {
                                     if (secsGem == null)
                                     {
                                         return;
                                     }

                                     if (val && secsGem.AxQGWrapper.EnableComm() == 0)
                                     {
                                         result = true;
                                     }
                                     else if (!val && secsGem.AxQGWrapper.DisableComm() == 0)
                                     {
                                         result = true;
                                     }
                                 });
            return result;
        }

        public async Task<bool> Online(bool val)
        {
            if (dp == null)
            {
                return false;
            }

            var result = false;
            await dp.InvokeAsync(() =>
                                 {
                                     if (secsGem == null)
                                     {
                                         return;
                                     }

                                     if (val && secsGem.AxQGWrapper.OnLineRequest() == 1)
                                     {
                                         result = true;
                                     }
                                     else if (!val && secsGem.AxQGWrapper.OffLine() == 1)
                                     {
                                         result = true;
                                     }
                                 });
            return result;
        }

        public async Task<bool> Remote(bool val)
        {
            if (dp == null)
            {
                return false;
            }

            var result = false;
            await dp.InvokeAsync(() =>
                                 {
                                     if (secsGem == null)
                                     {
                                         return;
                                     }

                                     if (val && secsGem.AxQGWrapper.OnLineRemote() == 1)
                                     {
                                         result = true;
                                     }
                                     else if (!val && secsGem.AxQGWrapper.OnLineLocal() == 1)
                                     {
                                         result = true;
                                     }
                                 });
            return result;
        }

        public SECSThread(int index)
        {
            var deviceIndex = index;

            var tcs = new TaskCompletionSource<bool>();
            thread = new Thread(() =>
                                {
                                    dp = Dispatcher.CurrentDispatcher;
                                    tcs.SetResult(true);
                                    Dispatcher.Run();
                                })
                     {
                         IsBackground = true
                     };

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            tcs.Task.Wait();

            dp?.InvokeAsync(() =>
                            {
                                SECSParameterSet = new SECSParameterSet();
                                //secsParameterSet.SECSParameter.HSMS_Connect_Mode = (int)HSMS_COMM_MODE.HSMS_PASSIVE_MODE;
                                //secsParameterSet.SECSParameter.LDeviceID         = deviceIndex;                          //todo:每台烤箱要有 Device Id
                                //secsParameterSet.SECSParameter.NLocalPort        = Convert.ToInt32($"600{deviceIndex}"); //todo:每台烤箱要有 Device Id
                                //secsParameterSet.SECSParameter.NRemotePort       = Convert.ToInt32($"600{deviceIndex}"); //todo:每台烤箱要有 Device Id
                                //secsParameterSet.SECSParameter.FilePath          = $"C:\\ITRIinit\\{deviceIndex}";       //設定檔存放位置
                                SECSParameterSet.SECSParameter.FilePath = $"C:\\ITRIinit\\{deviceIndex}";       //設定檔存放位置
                                SECSParameterSet.SECSParameter.MDLN = "GP_GO";
                                var v = Assembly.GetExecutingAssembly().GetName().Version;
                                SECSParameterSet.SECSParameter.SOFTREV = $"{v.Major}.{v.Minor}.{v.Build}";
                                secsGem                                = new GOSECS(SECSParameterSet.SECSParameter);

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

                                secsGem.InsertPPEvent += recipe => UpsertRecipe != null && UpsertRecipe.Invoke(recipe); //todo:收到新增或修改配方指令

                                secsGem.DeletePPEvent += recipeName =>
                                                         {
                                                             DeleteRecipe?.Invoke(recipeName);
                                                         };
                                //S2F41
                                secsGem.ADDLOTCommand += r =>
                                                         {
                                                             if (r.RemoteCommandParameter.Count < 4)
                                                             {
                                                                 return HCACKValule.ParameterInvalid;
                                                             }
                                                             //a8101.bank@fubon.com
                                                             if (r.RemoteCommandParameter[0].CPVAL.ObjectData is int[] indexes && indexes.Length > 0 &&
                                                                 r.RemoteCommandParameter[1].CPVAL.ObjectData is string lot &&
                                                                 r.RemoteCommandParameter[2].CPVAL.ObjectData is string part &&
                                                                 r.RemoteCommandParameter[3].CPVAL is SECSMessageBranches Branches)
                                                             {
                                                                 var i      = indexes[0];
                                                                 var panels = Branches.SECSMessageObjects.Select(x => x.ObjectData.ToString()).ToList();

                                                                 return AddLOT?.Invoke(i, (lot, part, panels)) ?? HCACKValule.CantPerform;
                                                             }

                                                             return HCACKValule.CantPerform;
                                                         };

                                secsGem.PP_SELECTCommand += r =>
                                                            {
                                                                if (r.RemoteCommandParameter.Count < 2)
                                                                {
                                                                    return HCACKValule.ParameterInvalid;
                                                                }
                                                                int[] o = r.RemoteCommandParameter[0].CPVAL.ObjectData as int[];
                                                                if (int.TryParse(o[0].ToString(), out var i))
                                                                {
                                                                    return SetRecipe?.Invoke(i, r.RemoteCommandParameter[1].CPVAL.ObjectData.ToString()) ?? HCACKValule.CantPerform;
                                                                }

                                                                return HCACKValule.CantPerform;
                                                            };

                                secsGem.STARTCommand += r =>
                                                        {
                                                            if (r.RemoteCommandParameter.Count < 1)
                                                            {
                                                                return HCACKValule.ParameterInvalid;
                                                            }

                                                            int[] o = r.RemoteCommandParameter[0].CPVAL.ObjectData as int[];
                                                            if (int.TryParse(o[0].ToString(), out var i))
                                                            {
                                                                return Start?.Invoke(i) ?? HCACKValule.CantPerform;
                                                            }

                                                            return HCACKValule.CantPerform;
                                                        };

                                secsGem.STOPCommand += r =>
                                                       {
                                                           if (r.RemoteCommandParameter.Count < 1)
                                                           {
                                                               return HCACKValule.ParameterInvalid;
                                                           }

                                                           int[] o = r.RemoteCommandParameter[0].CPVAL.ObjectData as int[];
                                                           if (int.TryParse(o[0].ToString(), out var i))
                                                           {
                                                               return Stop?.Invoke(i) ?? HCACKValule.CantPerform;
                                                           }

                                                           return HCACKValule.CantPerform;
                                                       };

                                secsGem.RetrieveLotDataCommand += r => HCACKValule.CantPerform; //todo
                                eqpBase = SECSTool.GetEqpbase($"{index}");
                                //SECS_GEM.GemDVDataUpdateNew("","");
                                //secsGem.GemSVDataUpdateNew(eqpBase.EqpSVViewModel, "PLCProgramVersion", "0001");
                                //var ALID = EqpBase.EqpAlarmViewModel.DataCollection.First(o => o.Name.Equals("AlarmSet")).ID;
                                //SECS_GEM.AxQGWrapper.AlarmReportSend(Convert.ToInt32(ALID), 255);
                                //secsGem.AxQGWrapper.AlarmReportSend(1, 1);

                                //if (eqpBase.EqpEventViewModel.DataCollection.FirstOrDefault(o => o.Name.Equals("AlarmSet")) is EqpEventClass ce && int.TryParse(ce.ID, out var CEID))
                                //{
                                //    secsGem.AxQGWrapper.EventReportSend(CEID);
                                //}

                                secsGem.SECSCommunicationControlViewModel.PropertyChanged += (s, e) =>
                                                                                             {
                                                                                                 var vm = (SECSCommunicationControlViewModel)s;

                                                                                                 switch (e.PropertyName)
                                                                                                 {
                                                                                                     case nameof(GP_GEM.SECSCommunicationControlViewModel.CommunicatioinState):
                                                                                                         CommEnable_Changed?.Invoke(vm.CommunicatioinState != (int)COMM_STATE.DISABLE);
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
                            });
        }
    }
}