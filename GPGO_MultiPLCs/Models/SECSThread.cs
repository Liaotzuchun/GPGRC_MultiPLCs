using GP_SECS_GEM;
using QSACTIVEXLib;
using Serilog;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GPGO_MultiPLCs.Models
{
    public class SECSThread
    {
        private GOSECS secsGem;
        private EqpBase eqpBase;

        public event Action<string> TerminalMessage;
        public event Action<int, string, object> ECChange;
        public event Func<PLC_Recipe, bool> UpsertRecipe;
        public event Action<string> DeleteRecipe;
        public event Action<int, string> SetRecipe;
        public event Action<int> Start;
        public event Action<int> Stop;
        public event Action<int, object> AddLOT;
        public event Func<int, string, ValueTask<object>> GetLOTInfo;

        public readonly Thread thread;
        public Dispatcher dp;

        public void UpdateSV(string name, object value)
        {
            dp?.InvokeAsync(() =>
                            {
                                secsGem.GemSVDataUpdateNew(eqpBase.EqpSVViewModel, name, value);
                            });
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

            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            tcs.Task.Wait();

            dp?.InvokeAsync(() =>
                            {
                                var secsParameterSet = new SECSParameterSet();
                                secsParameterSet.SECSParameter.HSMS_Connect_Mode = (int)HSMS_COMM_MODE.HSMS_PASSIVE_MODE;
                                secsParameterSet.SECSParameter.LDeviceID = deviceIndex;                          //todo:每台烤箱要有 Device Id
                                secsParameterSet.SECSParameter.NLocalPort = Convert.ToInt32($"600{deviceIndex}"); //todo:每台烤箱要有 Device Id
                                secsParameterSet.SECSParameter.NRemotePort = Convert.ToInt32($"600{deviceIndex}"); //todo:每台烤箱要有 Device Id
                                secsParameterSet.SECSParameter.FilePath = $"C:\\ITRIinit\\{deviceIndex}";       //設定檔存放位置
                                secsParameterSet.SECSParameter.MDLN = "GP_GO";
                                var v = Assembly.GetExecutingAssembly().GetName().Version;
                                secsParameterSet.SECSParameter.SOFTREV = $"{v.Major}.{v.Minor}.{v.Build}";
                                secsGem = new GOSECS(secsParameterSet.SECSParameter);

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
                                secsGem.STARTLOTCommand += r =>
                                                           {
                                                           };

                                secsGem.PP_SELECTCommand += r =>
                                                            {
                                                            };

                                secsGem.STARTCommand += r =>
                                                        {
                                                        };

                                secsGem.STOPCommand += r =>
                                                       {
                                                       };

                                secsGem.LOTMANAGEMENTCommand += r =>
                                                                {
                                                                };

                                secsGem.RetrieveLotDataCommand += r =>
                                                                  {
                                                                  };

                                eqpBase = SECSTool.GetEqpbase($"{index}");
                                //SECS_GEM.GemDVDataUpdateNew("","");
                                secsGem.GemSVDataUpdateNew(eqpBase.EqpSVViewModel, "PLCProgramVersion", "0001");
                                //var ALID = EqpBase.EqpAlarmViewModel.DataCollection.First(o => o.Name.Equals("AlarmSet")).ID;
                                //SECS_GEM.AxQGWrapper.AlarmReportSend(Convert.ToInt32(ALID), 255);
                                secsGem.AxQGWrapper.AlarmReportSend(1, 1);
                                try
                                {
                                    var ECID = eqpBase.EqpECViewModel.DataCollection.First(o => o.Name.Equals("AlarmSet")).ID;
                                    secsGem.AxQGWrapper.EventReportSend(Convert.ToInt32(ECID));
                                }
                                catch (Exception e)
                                {
                                    Log.Logger.Warning("EventSentAlarmSetError", e);
                                }
                            });
        }
    }
}