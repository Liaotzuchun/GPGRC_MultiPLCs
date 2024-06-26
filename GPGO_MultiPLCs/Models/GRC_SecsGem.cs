using System;
using GPMVVM.Models.SECS;
using GPMVVM.SECSGEM;

namespace GPGRC_MultiPLCs.Models;

public class GRC_SecsGem : SecsGemEquipment
{
    public event Func< (string LotID, string PartID, int quantity), HCACKValule>? ADDLOT_Command;
    public event Func<int, HCACKValule>?                                                         CANCEL_Command;
    public event Func<int, HCACKValule>?                                                         START_Command;
    public event Func<int, HCACKValule>?                                                         STOP_Command;
    public event Func<(string PPID, string LotID, int quantity), HCACKValule>?                                                 PPSELECT_Command;

    public GRC_SecsGem(string id, string name, string version) : base(id, name, version) => RemoteCommand += r =>
                                                                                                                {
                                                                                                                 //if (r.CommandName == "ADDLOT")
                                                                                                                 //{
                                                                                                                 //    if (r.RemoteCommandParameters.TryGetValue("LotID", out var o2) &&
                                                                                                                 //        o2 is string lot &&
                                                                                                                 //        r.RemoteCommandParameters.TryGetValue("PartID", out var o3) &&
                                                                                                                 //        o3 is string part &&
                                                                                                                 //        r.RemoteCommandParameters.TryGetValue("Quantity", out var o5) &&
                                                                                                                 //        o5 is uint quantity)
                                                                                                                 //    {
                                                                                                                 //        return new RemoteCommandResponse(ADDLOT_Command?.Invoke( (lot, part,(int)quantity)) ?? HCACKValule.ParameterInvalid);
                                                                                                                 //    }

                                                                                                                 //    return new RemoteCommandResponse(HCACKValule.CantPerform);
                                                                                                                 //}
                                                                                                                 if (r.CommandName == "PP_SELECT")
                                                                                                                 {
                                                                                                                     if (r.RemoteCommandParameters is { Count: < 3 })
                                                                                                                     {
                                                                                                                         return new RemoteCommandResponse(HCACKValule.ParameterInvalid);
                                                                                                                     }
                                                                                                                     if (r.RemoteCommandParameters.TryGetValue("PPID", out var o1) &&
                                                                                                                         o1 is string ppid &&
                                                                                                                         r.RemoteCommandParameters.TryGetValue("LotID", out var o2) &&
                                                                                                                         o2 is string lot &&
                                                                                                                         r.RemoteCommandParameters.TryGetValue("Quantity", out var o3) &&
                                                                                                                         o3 is int quantity)
                                                                                                                     {
                                                                                                                         var result = PPSELECT_Command?.Invoke( (ppid, lot,(int)quantity));
                                                                                                                         return new RemoteCommandResponse(result ?? HCACKValule.ParameterInvalid);
                                                                                                                     }

                                                                                                                     return new RemoteCommandResponse(HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "STOP")
                                                                                                                 {
                                                                                                                     return new RemoteCommandResponse(STOP_Command?.Invoke(0) ?? HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "GO_LOCAL")
                                                                                                                 {
                                                                                                                     return new RemoteCommandResponse(Remote(false) ? HCACKValule.Acknowledge : HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "GO_REMOTE")
                                                                                                                 {
                                                                                                                     return new RemoteCommandResponse(Remote(true) ? HCACKValule.Acknowledge : HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 //if (r.CommandName == "START")
                                                                                                                 //{
                                                                                                                 //    return new RemoteCommandResponse(r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o) && o is uint index ?
                                                                                                                 //                                         START_Command?.Invoke((int)index) ?? HCACKValule.ParameterInvalid :
                                                                                                                 //                                         HCACKValule.CantPerform);
                                                                                                                 //}
                                                                                                                 //if (r.CommandName == "CANCEL")
                                                                                                                 //{
                                                                                                                 //    if (r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o) && o is uint index)
                                                                                                                 //    {
                                                                                                                 //        return new RemoteCommandResponse(CANCEL_Command?.Invoke((int)index) ?? HCACKValule.ParameterInvalid);
                                                                                                                 //    }

                                                                                                                 //    return new RemoteCommandResponse(HCACKValule.CantPerform);
                                                                                                                 //}

                                                                                                                 return new RemoteCommandResponse(HCACKValule.CmdNotExist);
                                                                                                             };
}