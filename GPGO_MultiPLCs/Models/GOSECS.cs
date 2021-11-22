using GP_SECS_GEM;
using GPCore;
using GPMVVM.Models;
using QGACTIVEXLib;
using System;
using System.Linq;

namespace GPGO_MultiPLCs.Models
{
    public class GOSECS : GP_GEM
    {
        #region "GSESECS 目前除了基底外使用的變數 再想辦法拿掉"

        /// <summary>
        /// 前一個Download的PP
        /// </summary>
        private string LastDownloadPPID;

        #endregion "GSESECS 目前除了基底外使用的變數 再想辦法拿掉"

        #region "SECS條件初始化區"

        /// <summary> 使用在Coater的GEM</summary>
        public GOSECS(SECSParameter SECSParameter) : base(SECSParameter) {}

        #endregion "SECS條件初始化區"

        #region "覆寫基底函數區"

        /// <summary> When SECS driver receive Remote Command. </summary>
        public override RemoteCommandResponse RemoteCommandControl(RemoteCommand RemoteCommand)
        {
            object Value;
            var    HCACK = HCACKValule.Acknowledge;
            switch (RemoteCommand.RCMD)
            {
                case "ADDLOT":
                    HCACK = ADDLOTCommand?.Invoke(RemoteCommand) ?? HCACKValule.CantPerform;
                    break;
                case "CANCEL":
                    HCACK = CANCELCommand?.Invoke(RemoteCommand) ?? HCACKValule.CantPerform;
                    break;
                case "PP_SELECT":
                    AxQGWrapper.GetSV(4, out _, out Value);
                    HCACK = Convert.ToInt32(Value) == 4 ? HCACKValule.CantPerform : PP_SELECTCommand?.Invoke(RemoteCommand) ?? HCACKValule.CantPerform;
                    break;
                case "START":
                    AxQGWrapper.GetSV(4, out _, out Value);
                    HCACK = Convert.ToInt32(Value) == 4 ? HCACKValule.CantPerform : STARTCommand?.Invoke(RemoteCommand) ?? HCACKValule.CantPerform;
                    break;
                case "STOP":
                    HCACK = STOPCommand?.Invoke(RemoteCommand) ?? HCACKValule.CantPerform;
                    break;
                case "RETRIEVELOTDATA":
                    HCACK = RetrieveLotDataCommand?.Invoke(RemoteCommand) ?? HCACKValule.CantPerform;
                    break;
                case "GO_LOCAL":
                    if (!AxQGWrapper.OnLineLocal().Equals(0))
                    {
                        HCACK = HCACKValule.CantPerform;
                    }

                    break;
                case "GO_REMOTE":
                    if (!AxQGWrapper.OnLineRemote().Equals(0))
                    {
                        HCACK = HCACKValule.CantPerform;
                    }

                    break;
                default:
                    HCACK = HCACKValule.CmdNotExist;
                    break;
            }

            return new RemoteCommandResponse(HCACK);
        }

        /// <summary> 
        /// Reported event to send encoded SECS message to remote side via additional component of SECS driver
        /// </summary>
        public override void GEM_QGEvent(int lID, int S, int F, int W_Bit, int SystemBytes, object RawData, int Length)
        {
            if (S == 7 && F == 4) //S7F24 Not Formatted Process Program Data
            {
                if (RawData is byte[] Tmp && Tmp[2] == 0)
                {
                    var s = new StreamReaderIni($"C:\\ITRIinit\\0\\ProcessJob\\{LastDownloadPPID}.pjb").Sections;

                    if (s?.Count > 0)
                    {
                        var A = s.First();

                        var recipe = new PLC_Recipe(LastDownloadPPID, "SECSGEM-HOST", UserLevel.Manager);

                        foreach (var nv in A.Value.ItemElements)
                        {
                            switch (nv.Key)
                            {
                                case nameof(PLC_Recipe.StepCounts):
                                    if (short.TryParse(nv.Value, out var n))
                                    {
                                        recipe.StepCounts = n;
                                    }

                                    break;
                                case nameof(PLC_Recipe.TemperatureSetpoint_1):
                                    if (double.TryParse(nv.Value, out var tt1))
                                    {
                                        recipe.TemperatureSetpoint_1 = tt1;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampTime_1):
                                    if (double.TryParse(nv.Value, out var rt1))
                                    {
                                        recipe.RampTime_1 = rt1;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampAlarm_1):
                                    if (double.TryParse(nv.Value, out var rtt1))
                                    {
                                        recipe.RampAlarm_1 = rtt1;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellTime_1):
                                    if (double.TryParse(nv.Value, out var dt1))
                                    {
                                        recipe.DwellTime_1 = dt1;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellAlarm_1):
                                    if (double.TryParse(nv.Value, out var dtt1))
                                    {
                                        recipe.DwellAlarm_1 = dtt1;
                                    }

                                    break;
                                case nameof(PLC_Recipe.TemperatureSetpoint_2):
                                    if (double.TryParse(nv.Value, out var tt2))
                                    {
                                        recipe.TemperatureSetpoint_2 = tt2;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampTime_2):
                                    if (double.TryParse(nv.Value, out var rt2))
                                    {
                                        recipe.RampTime_2 = rt2;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampAlarm_2):
                                    if (double.TryParse(nv.Value, out var rtt2))
                                    {
                                        recipe.RampAlarm_2 = rtt2;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellTime_2):
                                    if (double.TryParse(nv.Value, out var dt2))
                                    {
                                        recipe.DwellTime_2 = dt2;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellAlarm_2):
                                    if (double.TryParse(nv.Value, out var dtt2))
                                    {
                                        recipe.DwellAlarm_2 = dtt2;
                                    }

                                    break;
                                case nameof(PLC_Recipe.TemperatureSetpoint_3):
                                    if (double.TryParse(nv.Value, out var tt3))
                                    {
                                        recipe.TemperatureSetpoint_3 = tt3;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampTime_3):
                                    if (double.TryParse(nv.Value, out var rt3))
                                    {
                                        recipe.RampTime_3 = rt3;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampAlarm_3):
                                    if (double.TryParse(nv.Value, out var rtt3))
                                    {
                                        recipe.RampAlarm_3 = rtt3;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellTime_3):
                                    if (double.TryParse(nv.Value, out var dt3))
                                    {
                                        recipe.DwellTime_3 = dt3;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellAlarm_3):
                                    if (double.TryParse(nv.Value, out var dtt3))
                                    {
                                        recipe.DwellAlarm_3 = dtt3;
                                    }

                                    break;
                                case nameof(PLC_Recipe.TemperatureSetpoint_4):
                                    if (double.TryParse(nv.Value, out var tt4))
                                    {
                                        recipe.TemperatureSetpoint_4 = tt4;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampTime_4):
                                    if (double.TryParse(nv.Value, out var rt4))
                                    {
                                        recipe.RampTime_4 = rt4;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampAlarm_4):
                                    if (double.TryParse(nv.Value, out var rtt4))
                                    {
                                        recipe.RampAlarm_4 = rtt4;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellTime_4):
                                    if (double.TryParse(nv.Value, out var dt4))
                                    {
                                        recipe.DwellTime_4 = dt4;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellAlarm_4):
                                    if (double.TryParse(nv.Value, out var dtt4))
                                    {
                                        recipe.DwellAlarm_4 = dtt4;
                                    }

                                    break;
                                case nameof(PLC_Recipe.TemperatureSetpoint_5):
                                    if (double.TryParse(nv.Value, out var tt5))
                                    {
                                        recipe.TemperatureSetpoint_5 = tt5;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampTime_5):
                                    if (double.TryParse(nv.Value, out var rt5))
                                    {
                                        recipe.RampTime_5 = rt5;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampAlarm_5):
                                    if (double.TryParse(nv.Value, out var rtt5))
                                    {
                                        recipe.RampAlarm_5 = rtt5;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellTime_5):
                                    if (double.TryParse(nv.Value, out var dt5))
                                    {
                                        recipe.DwellTime_5 = dt5;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellAlarm_5):
                                    if (double.TryParse(nv.Value, out var dtt5))
                                    {
                                        recipe.DwellAlarm_5 = dtt5;
                                    }

                                    break;
                                case nameof(PLC_Recipe.TemperatureSetpoint_6):
                                    if (double.TryParse(nv.Value, out var tt6))
                                    {
                                        recipe.TemperatureSetpoint_6 = tt6;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampTime_6):
                                    if (double.TryParse(nv.Value, out var rt6))
                                    {
                                        recipe.RampTime_6 = rt6;
                                    }

                                    break;
                                case nameof(PLC_Recipe.RampAlarm_6):
                                    if (double.TryParse(nv.Value, out var rtt6))
                                    {
                                        recipe.RampAlarm_6 = rtt6;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellTime_6):
                                    if (double.TryParse(nv.Value, out var dt6))
                                    {
                                        recipe.DwellTime_6 = dt6;
                                    }

                                    break;
                                case nameof(PLC_Recipe.DwellAlarm_6):
                                    if (double.TryParse(nv.Value, out var dtt6))
                                    {
                                        recipe.DwellAlarm_6 = dtt6;
                                    }

                                    break;
                                case nameof(PLC_Recipe.CoolingTime):
                                    if (double.TryParse(nv.Value, out var ct))
                                    {
                                        recipe.CoolingTime = ct;
                                    }

                                    break;
                                case nameof(PLC_Recipe.CoolingTemperature):
                                    if (double.TryParse(nv.Value, out var ctt))
                                    {
                                        recipe.CoolingTemperature = ctt;
                                    }

                                    break;
                                case nameof(PLC_Recipe.ProgramEndWarningTime):
                                    if (double.TryParse(nv.Value, out var ps))
                                    {
                                        recipe.ProgramEndWarningTime = ps;
                                    }

                                    break;
                            }
                        }

                        if (InsertPPEvent != null && !InsertPPEvent.Invoke(recipe))
                        {
                            Tmp[2] = 1;
                            //todo
                        }
                        else
                        {
                            Tmp[2] = 0;
                        }
                    }
                }

                AxQSWrapper.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
            }

            if (S == 7 && F == 24) //S7F24 Formatted Process Program Data
            {
                if (RawData is byte[] Tmp && Tmp[2] == 0)
                {
                    var A = new StreamReaderIni($"C:\\ITRIinit\\0\\ProcessJob\\{LastDownloadPPID}.pjb").Sections.First();

                    var recipe = new PLC_Recipe(LastDownloadPPID, "SECSGEM-HOST", UserLevel.Manager);

                    foreach (var nv in A.Value.ItemElements)
                    {
                        switch (nv.Key)
                        {
                            case nameof(PLC_Recipe.StepCounts):
                                if (short.TryParse(nv.Value, out var n))
                                {
                                    recipe.StepCounts = n;
                                }

                                break;
                            case nameof(PLC_Recipe.TemperatureSetpoint_1):
                                if (double.TryParse(nv.Value, out var tt1))
                                {
                                    recipe.TemperatureSetpoint_1 = tt1;
                                }

                                break;
                            case nameof(PLC_Recipe.RampTime_1):
                                if (double.TryParse(nv.Value, out var rt1))
                                {
                                    recipe.RampTime_1 = rt1;
                                }

                                break;
                            case nameof(PLC_Recipe.RampAlarm_1):
                                if (double.TryParse(nv.Value, out var rtt1))
                                {
                                    recipe.RampAlarm_1 = rtt1;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellTime_1):
                                if (double.TryParse(nv.Value, out var dt1))
                                {
                                    recipe.DwellTime_1 = dt1;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellAlarm_1):
                                if (double.TryParse(nv.Value, out var dtt1))
                                {
                                    recipe.DwellAlarm_1 = dtt1;
                                }

                                break;
                            case nameof(PLC_Recipe.TemperatureSetpoint_2):
                                if (double.TryParse(nv.Value, out var tt2))
                                {
                                    recipe.TemperatureSetpoint_2 = tt2;
                                }

                                break;
                            case nameof(PLC_Recipe.RampTime_2):
                                if (double.TryParse(nv.Value, out var rt2))
                                {
                                    recipe.RampTime_2 = rt2;
                                }

                                break;
                            case nameof(PLC_Recipe.RampAlarm_2):
                                if (double.TryParse(nv.Value, out var rtt2))
                                {
                                    recipe.RampAlarm_2 = rtt2;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellTime_2):
                                if (double.TryParse(nv.Value, out var dt2))
                                {
                                    recipe.DwellTime_2 = dt2;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellAlarm_2):
                                if (double.TryParse(nv.Value, out var dtt2))
                                {
                                    recipe.DwellAlarm_2 = dtt2;
                                }

                                break;
                            case nameof(PLC_Recipe.TemperatureSetpoint_3):
                                if (double.TryParse(nv.Value, out var tt3))
                                {
                                    recipe.TemperatureSetpoint_3 = tt3;
                                }

                                break;
                            case nameof(PLC_Recipe.RampTime_3):
                                if (double.TryParse(nv.Value, out var rt3))
                                {
                                    recipe.RampTime_3 = rt3;
                                }

                                break;
                            case nameof(PLC_Recipe.RampAlarm_3):
                                if (double.TryParse(nv.Value, out var rtt3))
                                {
                                    recipe.RampAlarm_3 = rtt3;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellTime_3):
                                if (double.TryParse(nv.Value, out var dt3))
                                {
                                    recipe.DwellTime_3 = dt3;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellAlarm_3):
                                if (double.TryParse(nv.Value, out var dtt3))
                                {
                                    recipe.DwellAlarm_3 = dtt3;
                                }

                                break;
                            case nameof(PLC_Recipe.TemperatureSetpoint_4):
                                if (double.TryParse(nv.Value, out var tt4))
                                {
                                    recipe.TemperatureSetpoint_4 = tt4;
                                }

                                break;
                            case nameof(PLC_Recipe.RampTime_4):
                                if (double.TryParse(nv.Value, out var rt4))
                                {
                                    recipe.RampTime_4 = rt4;
                                }

                                break;
                            case nameof(PLC_Recipe.RampAlarm_4):
                                if (double.TryParse(nv.Value, out var rtt4))
                                {
                                    recipe.RampAlarm_4 = rtt4;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellTime_4):
                                if (double.TryParse(nv.Value, out var dt4))
                                {
                                    recipe.DwellTime_4 = dt4;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellAlarm_4):
                                if (double.TryParse(nv.Value, out var dtt4))
                                {
                                    recipe.DwellAlarm_4 = dtt4;
                                }

                                break;
                            case nameof(PLC_Recipe.TemperatureSetpoint_5):
                                if (double.TryParse(nv.Value, out var tt5))
                                {
                                    recipe.TemperatureSetpoint_5 = tt5;
                                }

                                break;
                            case nameof(PLC_Recipe.RampTime_5):
                                if (double.TryParse(nv.Value, out var rt5))
                                {
                                    recipe.RampTime_5 = rt5;
                                }

                                break;
                            case nameof(PLC_Recipe.RampAlarm_5):
                                if (double.TryParse(nv.Value, out var rtt5))
                                {
                                    recipe.RampAlarm_5 = rtt5;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellTime_5):
                                if (double.TryParse(nv.Value, out var dt5))
                                {
                                    recipe.DwellTime_5 = dt5;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellAlarm_5):
                                if (double.TryParse(nv.Value, out var dtt5))
                                {
                                    recipe.DwellAlarm_5 = dtt5;
                                }

                                break;
                            case nameof(PLC_Recipe.TemperatureSetpoint_6):
                                if (double.TryParse(nv.Value, out var tt6))
                                {
                                    recipe.TemperatureSetpoint_6 = tt6;
                                }

                                break;
                            case nameof(PLC_Recipe.RampTime_6):
                                if (double.TryParse(nv.Value, out var rt6))
                                {
                                    recipe.RampTime_6 = rt6;
                                }

                                break;
                            case nameof(PLC_Recipe.RampAlarm_6):
                                if (double.TryParse(nv.Value, out var rtt6))
                                {
                                    recipe.RampAlarm_6 = rtt6;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellTime_6):
                                if (double.TryParse(nv.Value, out var dt6))
                                {
                                    recipe.DwellTime_6 = dt6;
                                }

                                break;
                            case nameof(PLC_Recipe.DwellAlarm_6):
                                if (double.TryParse(nv.Value, out var dtt6))
                                {
                                    recipe.DwellAlarm_6 = dtt6;
                                }

                                break;
                            case nameof(PLC_Recipe.CoolingTime):
                                if (double.TryParse(nv.Value, out var ct))
                                {
                                    recipe.CoolingTime = ct;
                                }

                                break;
                            case nameof(PLC_Recipe.CoolingTemperature):
                                if (double.TryParse(nv.Value, out var ctt))
                                {
                                    recipe.CoolingTemperature = ctt;
                                }

                                break;
                            case nameof(PLC_Recipe.ProgramEndWarningTime):
                                if (double.TryParse(nv.Value, out var ps))
                                {
                                    recipe.ProgramEndWarningTime = ps;
                                }

                                break;
                        }
                    }

                    Tmp[2] = InsertPPEvent != null && !InsertPPEvent.Invoke(recipe) ? (byte)1 : (byte)0;
                }

                AxQSWrapper.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
            }
            else if (S == 2 && F == 16) //S2F16 New Equipment Constant Acknoledge
            {
                if (RawData is byte[] Tmp && Tmp[2] == 0)
                {
                    ECChangeEndEvent?.Invoke(SystemBytes);
                }

                AxQSWrapper.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
            }
            else
            {
                //ShowSECSIIMessage RawData
                AxQSWrapper.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
            }
        }

        /// <summary> 
        /// Reported event to indicate that the specific process program is changed.
        /// </summary>
        public override void GEM_PPEvent(PP_TYPE MsgID, string PPID)
        {
            //int.TryParse(PPID.Substring(2), out int RecipeID);
            switch (MsgID)
            {
                //Group1
                case PP_TYPE.PP_DELETE: //1 S7F17 若有多個PPID被刪除時，QuickGem會回報多次PP_DELETE
                    DeletePPEvent?.Invoke(PPID);
                    break;
                case PP_TYPE.PP_UPLOAD: //2 S7F5,S7F25 Process program is uploaded
                    break;
                case PP_TYPE.PP_DOWNLOAD: //3 S7F3 Unformatted process program is downloaded
                    LastDownloadPPID = PPID;
                    break;
                case PP_TYPE.PP_DOWNLOAD_S7F23:
                    //GPCore.StreamReaderIni A = new GPCore.StreamReaderIni($"D:\\ITRIinit\\ProcessJob\\{PPID}.pjb");
                    LastDownloadPPID = PPID;
                    break;
                case PP_TYPE.PP_DELETE_END: //9
                    break;
                //Group3
                case PP_TYPE.RECEIVE_NEW_EC:
                    ECChangeEvent?.Invoke((PPID.Split(',')[0], PPID.Split(',')[1]));
                    break;

                #region S2F41

                case PP_TYPE.RECEIVE_S2F41_RCMD: //73
                    break;
                case PP_TYPE.RECEIVE_S2F41_CPNAME: //74
                    break;
                case PP_TYPE.RECEIVE_S2F41_CPVAL: //75
                    break;
                case PP_TYPE.RECEIVE_S2F41_RCMD_END: //76
                    break;
                case PP_TYPE.RECEIVE_S2F41_ERROR_RCMD: //77
                    break;

                #endregion
            }
        }

        /// <summary> 
        /// Reported event to indicate the received terminal message from remote side
        /// </summary>
        public override void GEM_TerminalMsgReceive(string Message) { TerminalMessageEvent?.Invoke(Message); }

        #endregion "覆寫基底函數區"

        #region "EventCollection"

        /// <summary>
        /// 收到Terminal訊息
        /// </summary>
        public event Action<string> TerminalMessageEvent;

        /// <summary>
        /// Host 下載 PP事件
        /// </summary>
        public event Func<PLC_Recipe, bool> InsertPPEvent;

        /// <summary>
        /// Host 刪除 PP事件
        /// </summary>
        public event Action<string> DeletePPEvent;

        /// <summary>
        /// Host下達改變EC事件
        /// </summary>
        public event Action<int> ECChangeEndEvent;

        /// <summary>
        /// Host下達改變EC事件(EC參數)
        /// </summary>
        public event Action<(string ECID, string Value)> ECChangeEvent;

        #endregion "EventCollection"

        #region "RemoteCommand"

        public event Func<RemoteCommand, HCACKValule> ADDLOTCommand;
        public event Func<RemoteCommand, HCACKValule> CANCELCommand;
        public event Func<RemoteCommand, HCACKValule> PP_SELECTCommand;
        public event Func<RemoteCommand, HCACKValule> STARTCommand;
        public event Func<RemoteCommand, HCACKValule> STOPCommand;
        public event Func<RemoteCommand, HCACKValule> RetrieveLotDataCommand;

        #endregion "RemoteCommand"
    }
}