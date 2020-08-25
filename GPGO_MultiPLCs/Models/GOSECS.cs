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
    #region "Event 事件 參數宣告們"
    /// <summary> TerminalMessageArgs</summary>
    public class TerminalMessageArgs : EventArgs
    {
        /// <summary> TerminalMessageArgs</summary>
        public TerminalMessageArgs(String message)
        {
            _Message = message;
        }
        /// <summary> _Message</summary>
        private readonly String _Message;
        /// <summary> Message</summary>
        public String Message
        {
            get { return _Message; }
        }
    }
    /// <summary> InsertPPCommandArgs</summary>
    public class InsertPPCommandArgs : EventArgs
    {
        /// <summary> InsertPPCommandArgs</summary>
        public InsertPPCommandArgs(RecipeBase<PLC_Recipe> P)
        {
            _ProcessProgram = P;
        }
        /// <summary> 加入的PP</summary>
        private readonly RecipeBase<PLC_Recipe> _ProcessProgram;
        /// <summary> 加入的PP</summary>
        public RecipeBase<PLC_Recipe> ProcessProgram
        {
            get { return _ProcessProgram; }
        }
    }
    /// <summary> DeletePPCommandArgs</summary>
    public class DeletePPCommandArgs : EventArgs
    {
        /// <summary> DeletePPCommandArgs</summary>
        public DeletePPCommandArgs(String P)
        {
            _ProcessProgram = P;
        }
        /// <summary> 刪除的PP Name</summary>
        private readonly String _ProcessProgram;
        /// <summary> 刪除的PP Name</summary>
        public String ProcessProgram
        {
            get { return _ProcessProgram; }
        }
    }
    /// <summary> ECChange訊號結束 </summary>
    public class ECChangeEndArgs : EventArgs
    {
        public int SystemBytes;
        /// <summary> ECChange訊號結束</summary>
        public ECChangeEndArgs(int S)
        {
            SystemBytes = S;
        }
    }
    /// <summary> ECChange</summary>
    public class ECChangeArgs : EventArgs
    {
        /// <summary> ECChange</summary>
        public ECChangeArgs(String eCID, String value)
        {
            _ECID = eCID;
            _Value = value;
        }
        /// <summary> ECID</summary>
        private readonly String _ECID;
        /// <summary> ECID</summary>
        public String ECID
        {
            get { return _ECID; }
        }
        /// <summary> Value</summary>
        private readonly String _Value;
        /// <summary> Value</summary>
        public String Value
        {
            get { return _Value; }
        }
    }
    #endregion "Event 事件 參數宣告們"
    public class GOSECS : GP_GEM
    {
        #region "GSESECS 目前除了基底外使用的變數 再想辦法拿掉"
        /// <summary>
        /// 前一個Download的PP
        /// </summary>
        private String LastDownloadPPID;
        #endregion "GSESECS 目前除了基底外使用的變數 再想辦法拿掉"
        #region "SECS條件初始化區"
        /// <summary> 使用在Coater的GEM</summary>
        public GOSECS(SECSParameter SECSParameter) : base(SECSParameter)
        {
        }
        #endregion "SECS條件初始化區"
        #region "覆寫基底函數區"
        /// <summary> When SECS driver receive Remote Command. </summary>
        public override RemoteCommandResponse RemoteCommandControl(RemoteCommand RemoteCommand)
        {
            HCACKValule HCACK = HCACKValule.Acknowledge;
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
            if (S == 7 && F == 4)//S7F24 Not Formatted Process Program Data
            {
                byte[] Tmp = RawData as byte[];
                if (Tmp[2] == 0)
                {
                    StreamReaderIni A = new StreamReaderIni($"C:\\ITRIinit\\0\\ProcessJob\\{LastDownloadPPID}.pjb");
                    RecipeBase<PLC_Recipe> t = null;
                    if (!OnRaisInsertPPCommandEvent(new InsertPPCommandArgs(t)))
                    {
                        Tmp[2] = 1;
                    }
                    else
                        Tmp[2] = 0;
                }
                AxQSWrapper.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
            }
            if (S == 7 && F == 24)//S7F24 Formatted Process Program Data
            {
                byte[] Tmp = RawData as byte[];
                if (Tmp[2] == 0)
                {
                    StreamReaderIni A = new StreamReaderIni($"C:\\ITRIinit\\0\\ProcessJob\\{LastDownloadPPID}.pjb");
                    RecipeBase<PLC_Recipe> t = null;
                    if (!OnRaisInsertPPCommandEvent(new InsertPPCommandArgs(t)))
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
            else if (S == 2 && F == 16)//S2F16 New Equipment Constant Acknoledge
            {
                byte[] Tmp = RawData as byte[];
                if (Tmp[2] == 0)
                {
                    OnRaiseECChangeEndEvent(new ECChangeEndArgs(SystemBytes));
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
                case PP_TYPE.PP_DELETE://1 S7F17 若有多個PPID被刪除時，QuickGem會回報多次PP_DELETE
                    OnRaisDeletePPCommandEvent(new DeletePPCommandArgs(PPID));
                    break;
                case PP_TYPE.PP_UPLOAD://2 S7F5,S7F25 Process program is uploaded
                    break;
                case PP_TYPE.PP_DOWNLOAD://3 S7F3 Unformatted process program is downloaded
                    LastDownloadPPID = PPID;
                    break;
                case PP_TYPE.PP_DOWNLOAD_S7F23:
                    //GPCore.StreamReaderIni A = new GPCore.StreamReaderIni($"D:\\ITRIinit\\ProcessJob\\{PPID}.pjb");
                    LastDownloadPPID = PPID;
                    break;
                case PP_TYPE.PP_DELETE_END://9
                    break;
                //Group3
                case PP_TYPE.RECEIVE_NEW_EC:
                    OnRaiseECChangeEvent(new ECChangeArgs(PPID.Split(',')[0], PPID.Split(',')[1]));
                    break;
                    //Group5 S2F41 RemoteCommand
                    //執行起來有那麼一點困難
                    //case PP_TYPE.RECEIVE_S2F41_RCMD://73
                    //    break;
                    //case PP_TYPE.RECEIVE_S2F41_CPNAME://74
                    //    break;
                    //case PP_TYPE.RECEIVE_S2F41_CPVAL://75
                    //    break;
                    //case PP_TYPE.RECEIVE_S2F41_RCMD_END://76
                    //    break;
                    //case PP_TYPE.RECEIVE_S2F41_ERROR_RCMD://77
                    //    break;
            }
        }

        /// <summary> 
        /// Reported event to indicate the received terminal message from remote side
        /// </summary>
        public override void GEM_TerminalMsgReceive(string Message)
        {
            OnRaiseTerminalMessageEvent(new TerminalMessageArgs(Message));
        }
        #endregion "覆寫基底函數區"
        #region "EventCollection"
        /// <summary>
        /// 收到Terminal訊息
        /// </summary>
        /// <param name="sender"> 觸發的Object</param>
        /// <param name="Message"> 訊息本人</param>
        public delegate void TerminalMessageArgsHandler(object sender, TerminalMessageArgs Message);
        public event TerminalMessageArgsHandler TerminalMessageEvent;
        /// <summary> 收到新的Terminal委派</summary>
        protected void OnRaiseTerminalMessageEvent(TerminalMessageArgs e)
        {
            TerminalMessageEvent?.Invoke(this, e);
        }
        /// <summary>
        /// Host 下載 PP事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="InsertPPCommand"></param>
        public delegate bool InsertPPCommandArgsHandler(object sender, InsertPPCommandArgs InsertPPCommand);
        public event InsertPPCommandArgsHandler InsertPPEvent;
        /// <summary> Host 下載 PP事件</summary>
        protected bool OnRaisInsertPPCommandEvent(InsertPPCommandArgs e)
        {
            if (InsertPPEvent == null)
            {
                return true;
            }
            else
            {
                return InsertPPEvent.Invoke(this, e);
            }
        }
        /// <summary>
        /// Host 刪除 PP事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="DeletePPCommand"></param>
        public delegate void DeletePPCommandArgsHandler(object sender, DeletePPCommandArgs DeletePPCommand);
        public event DeletePPCommandArgsHandler DeletePPEvent;
        /// <summary>  Host 刪除 PP事件</summary>
        protected void OnRaisDeletePPCommandEvent(DeletePPCommandArgs e)
        {
            DeletePPEvent?.Invoke(this, e);
        }
        /// <summary>
        /// Host下達改變EC事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ECChangeEndCommand"></param>
        public delegate void ECChangeEndArgsHandler(object sender, ECChangeEndArgs ECChangeEndCommand);
        public event ECChangeEndArgsHandler ECChangeEndEvent;
        /// <summary> Host下達改變EC事件</summary>
        protected void OnRaiseECChangeEndEvent(ECChangeEndArgs e)
        {
            ECChangeEndEvent.Invoke(this, e);
        }

        /// <summary>
        /// Host下達改變EC事件(EC參數)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ECChangeCommand"></param>
        public delegate void ECChangeArgsHandler(object sender, ECChangeArgs ECChangeCommand);
        public event ECChangeArgsHandler ECChangeEvent;
        /// <summary> 收到新的PPSelect委派</summary>
        protected void OnRaiseECChangeEvent(ECChangeArgs e)
        {
            ECChangeEvent?.Invoke(this, e);
        }
        #endregion "EventCollection"
        #region "RemoteCommand"
        public Action<RemoteCommand> STARTLOTCommand;
        public Action<RemoteCommand> PP_SELECTCommand;
        public Action<RemoteCommand> STARTCommand;
        public Action<RemoteCommand> STOPCommand;
        public Action<RemoteCommand> RetrieveLotDataCommand;
        public Action<RemoteCommand> LOTMANAGEMENTCommand;
        #endregion "RemoteCommand"
    }
}
