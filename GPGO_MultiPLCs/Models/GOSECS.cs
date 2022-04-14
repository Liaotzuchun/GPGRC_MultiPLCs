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
            //var str = SECSTool.ShowSECSIIMessage(AxQSWrapper, RawData);

            if (S == 7 && F is 4 or 24)
            {
                if (RawData is byte[] Tmp && Tmp[2] == 0)
                {
                    var s = new StreamReaderIni($"C:\\ITRIinit\\0\\ProcessJob\\{LastDownloadPPID}.pjb").Sections;
                    if (s?.Count > 0)
                    {
                        var A = s.First();

                        var recipe = new PLC_Recipe(LastDownloadPPID, "SECSGEM-HOST", UserLevel.Manager);

                        if (recipe.SetByDictionary(A.Value.ItemElements))
                        {
                            Tmp[2] = InsertPPEvent != null && !InsertPPEvent.Invoke(recipe) ? (byte)1 : (byte)0;
                        }
                        else
                        {
                            Tmp[2] = 1;
                        }
                    }
                }
            }
            else if (S == 2 && F == 16) //S2F16 New Equipment Constant Acknoledge
            {
                if (RawData is byte[] Tmp && Tmp[2] == 0)
                {
                    ECChangeEndEvent?.Invoke(SystemBytes);
                }
            }

            AxQSWrapper.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
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