using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GP_SECS_GEM;
using GPCore;
using GPMVVM.Models;
using QGACTIVEXLib;

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
            var HCACK = HCACKValule.Acknowledge;
            switch (RemoteCommand.RCMD)
            {
                case "STARTLOT":
                    STARTLOTCommand?.Invoke(RemoteCommand);
                    break;
                case "PP_SELECT":
                    PP_SELECTCommand?.Invoke(RemoteCommand);
                    break;
                case "START":
                    STARTCommand?.Invoke(RemoteCommand);
                    break;
                case "STOP":
                    STOPCommand?.Invoke(RemoteCommand);
                    break;
                case "LOTMANAGEMENT":
                    LOTMANAGEMENTCommand?.Invoke(RemoteCommand);
                    break;
                case "RETRIEVELOTDATA":
                    RetrieveLotDataCommand?.Invoke(RemoteCommand);
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
                    var A = new StreamReaderIni($"C:\\ITRIinit\\0\\ProcessJob\\{LastDownloadPPID}.pjb").Sections.First();

                    var recipe = new PLC_Recipe(A.Key, "", UserLevel.Manager);

                    foreach (var nv in A.Value.ItemElements)
                    {
                        switch (nv.Key)
                        {
                            case "StepCount":
                                if (short.TryParse(nv.Value, out var n))
                                {
                                    recipe.UsedSegmentCounts = n;
                                }

                                break;
                            case "TEMPERATURESV_STEP1":
                                if (double.TryParse(nv.Value, out var tt1))
                                {
                                    recipe.TargetTemperature_1 = tt1;
                                }

                                break;
                            case "RampTimeSV_STEP1":
                                if (double.TryParse(nv.Value, out var rt1))
                                {
                                    recipe.HeatingTime_1 = rt1;
                                }

                                break;
                            case "RampTimeoutSV_STEP1":
                                if (double.TryParse(nv.Value, out var rtt1))
                                {
                                    recipe.HeatingAlarm_1 = rtt1;
                                }

                                break;
                            case "DwellTimeSV_STEP1":
                                if (double.TryParse(nv.Value, out var dt1))
                                {
                                    recipe.WarmingTime_1 = dt1;
                                }

                                break;
                            case "DwellTimeoutSV_STEP1":
                                if (double.TryParse(nv.Value, out var dtt1))
                                {
                                    recipe.WarmingAlarm_1 = dtt1;
                                }

                                break;
                            case "TEMPERATURESV_STEP2":
                                if (double.TryParse(nv.Value, out var tt2))
                                {
                                    recipe.TargetTemperature_2 = tt2;
                                }

                                break;
                            case "RampTimeSV_STEP2":
                                if (double.TryParse(nv.Value, out var rt2))
                                {
                                    recipe.HeatingTime_2 = rt2;
                                }

                                break;
                            case "RampTimeoutSV_STEP2":
                                if (double.TryParse(nv.Value, out var rtt2))
                                {
                                    recipe.HeatingAlarm_2 = rtt2;
                                }

                                break;
                            case "DwellTimeSV_STEP2":
                                if (double.TryParse(nv.Value, out var dt2))
                                {
                                    recipe.WarmingTime_2 = dt2;
                                }

                                break;
                            case "DwellTimeoutSV_STEP2":
                                if (double.TryParse(nv.Value, out var dtt2))
                                {
                                    recipe.WarmingAlarm_2 = dtt2;
                                }

                                break;
                            case "TEMPERATURESV_STEP3":
                                if (double.TryParse(nv.Value, out var tt3))
                                {
                                    recipe.TargetTemperature_3 = tt3;
                                }

                                break;
                            case "RampTimeSV_STEP3":
                                if (double.TryParse(nv.Value, out var rt3))
                                {
                                    recipe.HeatingTime_3 = rt3;
                                }

                                break;
                            case "RampTimeoutSV_STEP3":
                                if (double.TryParse(nv.Value, out var rtt3))
                                {
                                    recipe.HeatingAlarm_3 = rtt3;
                                }

                                break;
                            case "DwellTimeSV_STEP3":
                                if (double.TryParse(nv.Value, out var dt3))
                                {
                                    recipe.WarmingTime_3 = dt3;
                                }

                                break;
                            case "DwellTimeoutSV_STEP3":
                                if (double.TryParse(nv.Value, out var dtt3))
                                {
                                    recipe.WarmingAlarm_3 = dtt3;
                                }

                                break;
                            case "TEMPERATURESV_STEP4":
                                if (double.TryParse(nv.Value, out var tt4))
                                {
                                    recipe.TargetTemperature_4 = tt4;
                                }

                                break;
                            case "RampTimeSV_STEP4":
                                if (double.TryParse(nv.Value, out var rt4))
                                {
                                    recipe.HeatingTime_4 = rt4;
                                }

                                break;
                            case "RampTimeoutSV_STEP4":
                                if (double.TryParse(nv.Value, out var rtt4))
                                {
                                    recipe.HeatingAlarm_4 = rtt4;
                                }

                                break;
                            case "DwellTimeSV_STEP4":
                                if (double.TryParse(nv.Value, out var dt4))
                                {
                                    recipe.WarmingTime_4 = dt4;
                                }

                                break;
                            case "DwellTimeoutSV_STEP4":
                                if (double.TryParse(nv.Value, out var dtt4))
                                {
                                    recipe.WarmingAlarm_4 = dtt4;
                                }

                                break;
                            case "TEMPERATURESV_STEP5":
                                if (double.TryParse(nv.Value, out var tt5))
                                {
                                    recipe.TargetTemperature_5 = tt5;
                                }

                                break;
                            case "RampTimeSV_STEP5":
                                if (double.TryParse(nv.Value, out var rt5))
                                {
                                    recipe.HeatingTime_5 = rt5;
                                }

                                break;
                            case "RampTimeoutSV_STEP5":
                                if (double.TryParse(nv.Value, out var rtt5))
                                {
                                    recipe.HeatingAlarm_5 = rtt5;
                                }

                                break;
                            case "DwellTimeSV_STEP5":
                                if (double.TryParse(nv.Value, out var dt5))
                                {
                                    recipe.WarmingTime_5 = dt5;
                                }

                                break;
                            case "DwellTimeoutSV_STEP5":
                                if (double.TryParse(nv.Value, out var dtt5))
                                {
                                    recipe.WarmingAlarm_5 = dtt5;
                                }

                                break;
                            case "TEMPERATURESV_STEP6":
                                if (double.TryParse(nv.Value, out var tt6))
                                {
                                    recipe.TargetTemperature_6 = tt6;
                                }

                                break;
                            case "RampTimeSV_STEP6":
                                if (double.TryParse(nv.Value, out var rt6))
                                {
                                    recipe.HeatingTime_6 = rt6;
                                }

                                break;
                            case "RampTimeoutSV_STEP6":
                                if (double.TryParse(nv.Value, out var rtt6))
                                {
                                    recipe.HeatingAlarm_6 = rtt6;
                                }

                                break;
                            case "DwellTimeSV_STEP6":
                                if (double.TryParse(nv.Value, out var dt6))
                                {
                                    recipe.WarmingTime_6 = dt6;
                                }

                                break;
                            case "DwellTimeoutSV_STEP6":
                                if (double.TryParse(nv.Value, out var dtt6))
                                {
                                    recipe.WarmingAlarm_6 = dtt6;
                                }

                                break;
                            case "CoolingTimeSV":
                                if (double.TryParse(nv.Value, out var ct))
                                {
                                    recipe.CoolingTime = ct;
                                }

                                break;
                            case "CoolingTemperatureAlarmSV":
                                if (double.TryParse(nv.Value, out var ctt))
                                {
                                    recipe.CoolingTemperature = ctt;
                                }

                                break;
                            case "ProgramEndWarningTimeSV":
                                if (double.TryParse(nv.Value, out var ps))
                                {
                                    recipe.ProgramStopAlarmTime = ps;
                                }

                                break;
                        }
                    }

                    if (InsertPPEvent != null && !InsertPPEvent.Invoke(recipe))
                    {
                        Tmp[2] = 1;
                    }
                    else
                    {
                        Tmp[2] = 0;
                    }
                }

                AxQSWrapper.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
            }

            if (S == 7 && F == 24) //S7F24 Formatted Process Program Data
            {
                if (RawData is byte[] Tmp && Tmp[2] == 0)
                {
                    var A = new StreamReaderIni($"C:\\ITRIinit\\0\\ProcessJob\\{LastDownloadPPID}.pjb").Sections.First();

                    var recipe = new PLC_Recipe(A.Key, "", UserLevel.Manager);

                    foreach (var nv in A.Value.ItemElements)
                    {
                        switch (nv.Key)
                        {
                            case "StepCount":
                                if (short.TryParse(nv.Value, out var n))
                                {
                                    recipe.UsedSegmentCounts = n;
                                }

                                break;
                            case "TEMPERATURESV_STEP1":
                                if (double.TryParse(nv.Value, out var tt1))
                                {
                                    recipe.TargetTemperature_1 = tt1;
                                }

                                break;
                            case "RampTimeSV_STEP1":
                                if (double.TryParse(nv.Value, out var rt1))
                                {
                                    recipe.HeatingTime_1 = rt1;
                                }

                                break;
                            case "RampTimeoutSV_STEP1":
                                if (double.TryParse(nv.Value, out var rtt1))
                                {
                                    recipe.HeatingAlarm_1 = rtt1;
                                }

                                break;
                            case "DwellTimeSV_STEP1":
                                if (double.TryParse(nv.Value, out var dt1))
                                {
                                    recipe.WarmingTime_1 = dt1;
                                }

                                break;
                            case "DwellTimeoutSV_STEP1":
                                if (double.TryParse(nv.Value, out var dtt1))
                                {
                                    recipe.WarmingAlarm_1 = dtt1;
                                }

                                break;
                            case "TEMPERATURESV_STEP2":
                                if (double.TryParse(nv.Value, out var tt2))
                                {
                                    recipe.TargetTemperature_2 = tt2;
                                }

                                break;
                            case "RampTimeSV_STEP2":
                                if (double.TryParse(nv.Value, out var rt2))
                                {
                                    recipe.HeatingTime_2 = rt2;
                                }

                                break;
                            case "RampTimeoutSV_STEP2":
                                if (double.TryParse(nv.Value, out var rtt2))
                                {
                                    recipe.HeatingAlarm_2 = rtt2;
                                }

                                break;
                            case "DwellTimeSV_STEP2":
                                if (double.TryParse(nv.Value, out var dt2))
                                {
                                    recipe.WarmingTime_2 = dt2;
                                }

                                break;
                            case "DwellTimeoutSV_STEP2":
                                if (double.TryParse(nv.Value, out var dtt2))
                                {
                                    recipe.WarmingAlarm_2 = dtt2;
                                }

                                break;
                            case "TEMPERATURESV_STEP3":
                                if (double.TryParse(nv.Value, out var tt3))
                                {
                                    recipe.TargetTemperature_3 = tt3;
                                }

                                break;
                            case "RampTimeSV_STEP3":
                                if (double.TryParse(nv.Value, out var rt3))
                                {
                                    recipe.HeatingTime_3 = rt3;
                                }

                                break;
                            case "RampTimeoutSV_STEP3":
                                if (double.TryParse(nv.Value, out var rtt3))
                                {
                                    recipe.HeatingAlarm_3 = rtt3;
                                }

                                break;
                            case "DwellTimeSV_STEP3":
                                if (double.TryParse(nv.Value, out var dt3))
                                {
                                    recipe.WarmingTime_3 = dt3;
                                }

                                break;
                            case "DwellTimeoutSV_STEP3":
                                if (double.TryParse(nv.Value, out var dtt3))
                                {
                                    recipe.WarmingAlarm_3 = dtt3;
                                }

                                break;
                            case "TEMPERATURESV_STEP4":
                                if (double.TryParse(nv.Value, out var tt4))
                                {
                                    recipe.TargetTemperature_4 = tt4;
                                }

                                break;
                            case "RampTimeSV_STEP4":
                                if (double.TryParse(nv.Value, out var rt4))
                                {
                                    recipe.HeatingTime_4 = rt4;
                                }

                                break;
                            case "RampTimeoutSV_STEP4":
                                if (double.TryParse(nv.Value, out var rtt4))
                                {
                                    recipe.HeatingAlarm_4 = rtt4;
                                }

                                break;
                            case "DwellTimeSV_STEP4":
                                if (double.TryParse(nv.Value, out var dt4))
                                {
                                    recipe.WarmingTime_4 = dt4;
                                }

                                break;
                            case "DwellTimeoutSV_STEP4":
                                if (double.TryParse(nv.Value, out var dtt4))
                                {
                                    recipe.WarmingAlarm_4 = dtt4;
                                }

                                break;
                            case "TEMPERATURESV_STEP5":
                                if (double.TryParse(nv.Value, out var tt5))
                                {
                                    recipe.TargetTemperature_5 = tt5;
                                }

                                break;
                            case "RampTimeSV_STEP5":
                                if (double.TryParse(nv.Value, out var rt5))
                                {
                                    recipe.HeatingTime_5 = rt5;
                                }

                                break;
                            case "RampTimeoutSV_STEP5":
                                if (double.TryParse(nv.Value, out var rtt5))
                                {
                                    recipe.HeatingAlarm_5 = rtt5;
                                }

                                break;
                            case "DwellTimeSV_STEP5":
                                if (double.TryParse(nv.Value, out var dt5))
                                {
                                    recipe.WarmingTime_5 = dt5;
                                }

                                break;
                            case "DwellTimeoutSV_STEP5":
                                if (double.TryParse(nv.Value, out var dtt5))
                                {
                                    recipe.WarmingAlarm_5 = dtt5;
                                }

                                break;
                            case "TEMPERATURESV_STEP6":
                                if (double.TryParse(nv.Value, out var tt6))
                                {
                                    recipe.TargetTemperature_6 = tt6;
                                }

                                break;
                            case "RampTimeSV_STEP6":
                                if (double.TryParse(nv.Value, out var rt6))
                                {
                                    recipe.HeatingTime_6 = rt6;
                                }

                                break;
                            case "RampTimeoutSV_STEP6":
                                if (double.TryParse(nv.Value, out var rtt6))
                                {
                                    recipe.HeatingAlarm_6 = rtt6;
                                }

                                break;
                            case "DwellTimeSV_STEP6":
                                if (double.TryParse(nv.Value, out var dt6))
                                {
                                    recipe.WarmingTime_6 = dt6;
                                }

                                break;
                            case "DwellTimeoutSV_STEP6":
                                if (double.TryParse(nv.Value, out var dtt6))
                                {
                                    recipe.WarmingAlarm_6 = dtt6;
                                }

                                break;
                            case "CoolingTimeSV":
                                if (double.TryParse(nv.Value, out var ct))
                                {
                                    recipe.CoolingTime = ct;
                                }

                                break;
                            case "CoolingTemperatureAlarmSV":
                                if (double.TryParse(nv.Value, out var ctt))
                                {
                                    recipe.CoolingTemperature = ctt;
                                }

                                break;
                            case "ProgramEndWarningTimeSV":
                                if (double.TryParse(nv.Value, out var ps))
                                {
                                    recipe.ProgramStopAlarmTime = ps;
                                }

                                break;
                        }
                    }

                    if (InsertPPEvent != null && !InsertPPEvent.Invoke(recipe))
                    {
                        Tmp[2] = 1;
                    }
                    else
                    {
                        Tmp[2] = 0;
                    }
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

                case PP_TYPE.PP_LOAD_INQUIRE:
                    break;
                case PP_TYPE.PP_INQUIRE_GRANT:
                    break;
                case PP_TYPE.PP_SEND_ACK:
                    break;
                case PP_TYPE.PP_REQUEST_RESULT:
                    break;
                case PP_TYPE.CMD_PP_LOAD_INQUIRE:
                    break;
                case PP_TYPE.CMD_UNFORMATTED_PP_SEND:
                    break;
                case PP_TYPE.CMD_UNFORMATTED_PP_REQUEST:
                    break;
                case PP_TYPE.CMD_FORMATTED_PP_SEND:
                    break;
                case PP_TYPE.CMD_FORMATTED_PP_REQUEST:
                    break;
                case PP_TYPE.CMD_SEARCH_ALL_LOCAL_PPID:
                    break;
                case PP_TYPE.CMD_SEARCH_ONE_LOCAL_PPID:
                    break;
                case PP_TYPE.CMD_CHANGE_PP_DIRECTORY:
                    break;
                case PP_TYPE.CMD_VERIFY_LOCAL_FORMATTED_PP:
                    break;
                case PP_TYPE.CMD_VERIFY_LOCAL_FORMATTED_PP_CCODE_IN_ORDER:
                    break;
                case PP_TYPE.CMD_ALARM_SET:
                    break;
                case PP_TYPE.CMD_REPLY_S7F4_ACK:
                    break;
                case PP_TYPE.CMD_REPLY_S7F18_ACK:
                    break;
                case PP_TYPE.CMD_REPLY_S7F24_ACK:
                    break;
                case PP_TYPE.CMD_REPLY_S2F42_HCACK:
                    break;
                case PP_TYPE.CMD_REPLY_S2F42_CPACK_ADD:
                    break;
                case PP_TYPE.CMD_SEND_S7F27:
                    break;
                case PP_TYPE.CMD_UPDATE_MULTI_CONTINUOUS_SV:
                    break;
                case PP_TYPE.CMD_UPDATE_MULTI_DISPERSE_SV:
                    break;
                case PP_TYPE.CMD_DATE_TIME_REQUEST:
                    break;
                case PP_TYPE.SEND_CE_RPT_NAME:
                    break;
                case PP_TYPE.SEND_AL_RPT_TEXT:
                    break;
                case PP_TYPE.RECEIVE_LOCAL_PPID_DATA:
                    break;
                case PP_TYPE.RECEIVE_LOCAL_PPID_END:
                    break;
                case PP_TYPE.RECEIVE_E87_PROCEED_WITH_CARRIER:
                    break;
                case PP_TYPE.RECEIVE_E87_PROCEED_WITH_CARRIER_CAPACITY:
                    break;
                case PP_TYPE.RECEIVE_E87_PROCEED_WITH_CARRIER_CONTENT_MAP:
                    break;
                case PP_TYPE.RECEIVE_E87_PROCEED_WITH_CARRIER_SLOP_MAP:
                    break;
                case PP_TYPE.RECEIVE_E87_PROCEED_WITH_CARRIER_SUBSTRATE_COUNT:
                    break;
                case PP_TYPE.RECEIVE_E87_PROCEED_WITH_CARRIER_USAGE:
                    break;
                case PP_TYPE.RECEIVE_E87_PROCEED_WITH_CARRIER_DATA_END:
                    break;
                case PP_TYPE.RECEIVE_E87_CANCEL_CARRIER_AT_PORT:
                    break;
                case PP_TYPE.RECEIVE_E87_CANCEL_CARRIER:
                    break;
                case PP_TYPE.RECEIVE_E87_CARRIER_RELEASE:
                    break;
                case PP_TYPE.RECEIVE_E87_CARRIER_SERVICE:
                    break;
                case PP_TYPE.RECEIVE_E87_CARRIER_SERVICE_DATA_END:
                    break;
                case PP_TYPE.RECEIVE_E87_CHG_ACCESS_MANUAL_ALL:
                    break;
                case PP_TYPE.RECEIVE_E87_CHG_ACCESS_MANUAL:
                    break;
                case PP_TYPE.RECEIVE_E87_CHG_ACCESS_AUTO_ALL:
                    break;
                case PP_TYPE.RECEIVE_E87_CHG_ACCESS_AUTO:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_CREATE_ENHANCE:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_CREATE_MORE_CARRIER:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_CREATE_ENHANCE_END:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_MULTI_CREATE_BEGIN:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_MULTI_CREATE_DATA:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_MULTI_CREATE_MORE_CARRIER:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_MULTI_CREATE_END:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_COMMAND_STARTPROCESS:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_COMMAND_ABORT:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_COMMAND_STOP:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_COMMAND_CANCEL:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_COMMAND_PAUSE:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_COMMAND_RESUME:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_DEQUEUE:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_DEQUEUE_DATA_END:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_DEQUEUE_ALL:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_GET_ALL_JOB:
                    break;
                case PP_TYPE.RECEIVE_E40_PJ_GET_SPACE:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_OBJID:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRORDERMGMT:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_STARTMETHOD:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_CARRIERINPUTSPEC_NUMBER:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_MTRLOUTSPEC_NUMBER:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_DATA_END:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_COMMAND_START:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_COMMAND_PAUSE:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_COMMAND_RESUME:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_COMMAND_CANCEL:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_COMMAND_DESELECT:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_COMMAND_STOP:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_COMMAND_ABORT:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_COMMAND_HOQ:
                    break;
                case PP_TYPE.CMD_E87_REPLY_S3F18_CARRIER_ACTION_ACK:
                    break;
                case PP_TYPE.CMD_E87_CLEAR_S3F18_ERRCODE_:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F18_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_REPLY_S3F28_CHANGE_ACCESS_ACK:
                    break;
                case PP_TYPE.CMD_E87_CLEAR_S3F28_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F28_PTN1_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F28_PTN2_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F28_PTN3_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F28_PTN4_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F28_PTN5_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F28_PTN6_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F28_PTN7_ERRCODE:
                    break;
                case PP_TYPE.CMD_E87_ADD_S3F28_PTN8_ERRCODE:
                    break;
                case PP_TYPE.CMD_E40_REPLY_S16F6_PJ_COMMAND_ACK:
                    break;
                case PP_TYPE.CMD_E40_CLEAR_S16F6_ERRCODE:
                    break;
                case PP_TYPE.CMD_E40_ADD_S16F6_ERRCODE:
                    break;
                case PP_TYPE.CMD_E40_REPLY_S16F12_PJ_CREATE_ACK:
                    break;
                case PP_TYPE.CMD_E40_CLEAR_S16F12_ERRCODE:
                    break;
                case PP_TYPE.CMD_E40_REPLY_S16F16_PJ_MULTI_CREATE_ACK:
                    break;
                case PP_TYPE.CMD_E40_CLEAR_S16F16_ERRCODE:
                    break;
                case PP_TYPE.CMD_E40_ADD_S16F16_ERRCODE:
                    break;
                case PP_TYPE.CMD_E40_ADD_S16F16_PJID:
                    break;
                case PP_TYPE.CMD_E40_REPLY_S16F18_PJ_DEQUEUE_ACK:
                    break;
                case PP_TYPE.CMD_E40_CLEAR_S16F18_PJID:
                    break;
                case PP_TYPE.CMD_E40_ADD_S16F18_PJID:
                    break;
                case PP_TYPE.CMD_E40_CLEAR_S16F18_ERRCODE:
                    break;
                case PP_TYPE.CMD_E40_ADD_S16F18_ERRCODE:
                    break;
                case PP_TYPE.CMD_E40_REPLY_S16F20_PJ_GET_ALL_JOB_ACK:
                    break;
                case PP_TYPE.CMD_E40_REPLY_S16F20_PJ_STATE_CLEAR:
                    break;
                case PP_TYPE.CMD_E40_ADD_S16F20_PJ_STATE:
                    break;
                case PP_TYPE.CMD_E40_REPLY_S16F22_PJ_GET_SPACE_SEND:
                    break;
                case PP_TYPE.CMD_E94_REPLY_S14F10_CJ_CREATE_ACK:
                    break;
                case PP_TYPE.CMD_E94_CLEAR_S14F10_ERRCODE:
                    break;
                case PP_TYPE.CMD_E94_ADD_S14F10_ERRCODE:
                    break;
                case PP_TYPE.CMD_E94_REPLY_S16F28_CJ_CAMMAND_ACK:
                    break;
                case PP_TYPE.CMD_E94_CLEAR_S16F28_ERRCODE:
                    break;
                case PP_TYPE.CMD_E94_ADD_S16F28_ERRCODE:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_1:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_2:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_3:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_4:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_5:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_6:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_7:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_8:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_PRCTRLSPEC_MAX:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_CARRIERID_INPUT_1:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_CARRIERID_INPUT_MAX:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_CARRIERID_OUT_SRC:
                    break;
                case PP_TYPE.RECEIVE_E94_CJ_CREATE_CARRIERID_OUT_DST:
                    break;
                case PP_TYPE.CMD_INIT_S14F1_GET_ATTR_REQUEST:
                    break;
                case PP_TYPE.CMD_ADD_S14F1_OBJSPEC_OBJTYPE:
                    break;
                case PP_TYPE.CMD_ADD_S14F1_OBJID:
                    break;
                case PP_TYPE.CMD_ADD_S14F1_OBJECT_QUALIFIER:
                    break;
                case PP_TYPE.CMD_ADD_S14F1_ATTRIBUTE:
                    break;
                case PP_TYPE.CMD_SEND_S14F1_GET_ATTR_REQUEST:
                    break;
                case PP_TYPE.RECEIVE_S14F2_GET_ATTR_DATA:
                    break;
                case PP_TYPE.RECEIVE_S14F2_OBJID:
                    break;
                case PP_TYPE.RECEIVE_S14F2_ATTRID:
                    break;
                case PP_TYPE.RECEIVE_S14F2_ATTRDATA:
                    break;
                case PP_TYPE.RECEIVE_S14F2_OBJACK:
                    break;
                case PP_TYPE.RECEIVE_S14F2_ERRCODE:
                    break;
                case PP_TYPE.RECEIVE_S14F2_ERRTEXT:
                    break;
                case PP_TYPE.RECEIVE_S14F2_GET_ATTR_DATA_END:
                    break;
                case PP_TYPE.RECEIVE_E39_S14F1_REQUEST_OBJ_ALL:
                    break;
                case PP_TYPE.RECEIVE_E39_S14F1_REQUEST_OBJ_LIST_BEGIN:
                    break;
                case PP_TYPE.RECEIVE_E39_S14F1_REQUEST_OBJ_LIST_OBJID:
                    break;
                case PP_TYPE.RECEIVE_E39_S14F1_REQUEST_OBJ_LIST_END:
                    break;
                case PP_TYPE.CMD_E39_INIT_S14F2_OBJ_NUMBER:
                    break;
                case PP_TYPE.CMD_E39_ADD_S14F2_OBJ_DATA:
                    break;
                case PP_TYPE.CMD_E39_ADD_S14F2_ERRCODE:
                    break;
                case PP_TYPE.CMD_E39_REPLY_S14F2_OBJACK:
                    break;
                case PP_TYPE.CMD_INIT_S12F1_MAP_SETUP_DATA_SEND:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_MID:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_IDTYP:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_FNLOC:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_FFROT:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_ORLOC:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_RPSEL:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_REFP_XY:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_DUTMS:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_XDIES:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_YDIES:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_ROWCT:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_COLCT:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_NULBC:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_PRDCT:
                    break;
                case PP_TYPE.CMD_ADD_S12F1_MAP_ITEM_PRAXI:
                    break;
                case PP_TYPE.CMD_SEND_S12F1_MAP_SETUP_DATA_SEND:
                    break;
                case PP_TYPE.CMD_INIT_S12F3_MAP_SETUP_DATA_REQUEST:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_MID:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_IDTYP:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_MAPFT:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_FNLOC:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_FFROT:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_ORLOC:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_PRAXI:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_BCEQU:
                    break;
                case PP_TYPE.CMD_ADD_S12F3_MAP_ITEM_NULBC:
                    break;
                case PP_TYPE.CMD_SEND_S12F3_MAP_SETUP_DATA_REQUEST:
                    break;
                case PP_TYPE.CMD_INIT_S12F5_MAP_TRANSMIT_INQUIRE:
                    break;
                case PP_TYPE.CMD_ADD_S12F5_MAP_ITEM_MID:
                    break;
                case PP_TYPE.CMD_ADD_S12F5_MAP_ITEM_IDTYP:
                    break;
                case PP_TYPE.CMD_ADD_S12F5_MAP_ITEM_MAPFT:
                    break;
                case PP_TYPE.CMD_ADD_S12F5_MAP_ITEM_MLCL:
                    break;
                case PP_TYPE.CMD_SEND_S12F5_MAP_TRANSMIT_INQUIRE:
                    break;
                case PP_TYPE.CMD_INIT_S12F9_MAP_DATA_SEND_TYPE2:
                    break;
                case PP_TYPE.CMD_ADD_S12F9_MAP_ITEM_MID:
                    break;
                case PP_TYPE.CMD_ADD_S12F9_MAP_ITEM_IDTYP:
                    break;
                case PP_TYPE.CMD_ADD_S12F9_MAP_ITEM_STRP_XY:
                    break;
                case PP_TYPE.CMD_ADD_S12F9_MAP_ITEM_BINLT:
                    break;
                case PP_TYPE.CMD_SEND_S12F9_MAP_DATA_SEND_TYPE2:
                    break;
                case PP_TYPE.CMD_GET_S12F9_MESSAGE_LENGTH:
                    break;
                case PP_TYPE.CMD_INIT_S12F15_MAP_DATA_REQUEST_TYPE2:
                    break;
                case PP_TYPE.CMD_ADD_S12F15_MAP_ITEM_MID:
                    break;
                case PP_TYPE.CMD_ADD_S12F15_MAP_ITEM_IDTYP:
                    break;
                case PP_TYPE.CMD_SEND_S12F15_MAP_DATA_REQUEST_TYPE2:
                    break;
                case PP_TYPE.RECEIVE_S12F2_MAP_SETUP_DATA_ACK:
                    break;
                case PP_TYPE.RECEIVE_S12F4_MAP_SETUP_DATA_L0:
                    break;
                case PP_TYPE.RECEIVE_S12F4_MAP_SETUP_DATA:
                    break;
                case PP_TYPE.RECEIVE_S12F4_MAP_SETUP_DATA_END:
                    break;
                case PP_TYPE.RECEIVE_S12F6_MAP_TRANSMIT_GRANT:
                    break;
                case PP_TYPE.REPORT_S12F9_MESSAGE_LENGTH:
                    break;
                case PP_TYPE.RECEIVE_S12F10_MAP_DATA_ACK_TYPE2:
                    break;
                case PP_TYPE.RECEIVE_S12F16_MAP_DATA_TYPE2_L0:
                    break;
                case PP_TYPE.RECEIVE_S12F16_MAP_DATA_TYPE2:
                    break;
                case PP_TYPE.RECEIVE_S12F16_MAP_DATA_TYPE2_END:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_MID:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_IDTYP:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_FNLOC:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_ORLOC:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_RPSEL:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_REFP_X:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_REFP_Y:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_DUTMS:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_XDIES:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_YDIES:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_ROWCT:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_COLCT:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_PRDCT:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_BCEQU:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_NULBC:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_MLCL:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_STRP_X:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_STRP_Y:
                    break;
                case PP_TYPE.RECEIVE_MAP_ITEM_BINLT:
                    break;
                case PP_TYPE.GEM_ERROR_REPORT:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(MsgID), MsgID, null);
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

        public event Action<RemoteCommand> STARTLOTCommand;
        public event Action<RemoteCommand> PP_SELECTCommand;
        public event Action<RemoteCommand> STARTCommand;
        public event Action<RemoteCommand> STOPCommand;
        public event Action<RemoteCommand> RetrieveLotDataCommand;
        public event Action<RemoteCommand> LOTMANAGEMENTCommand;

        #endregion "RemoteCommand"
    }
}