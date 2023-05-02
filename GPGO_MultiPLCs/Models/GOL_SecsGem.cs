using System;
using GPMVVM.Models.SECS;
using GPMVVM.SECSGEM;

namespace GPGO_MultiPLCs.Models;

public class GOL_SecsGem : SecsGemEquipment
{
    public event Func<int, (string LotID, string PartID, int layer, int quantity), HCACKValule>? ADDLOT_Command;
    public event Func<int, HCACKValule>?                                                         CANCEL_Command;
    public event Func<int, HCACKValule>?                                                         START_Command;
    public event Func<int, HCACKValule>?                                                         STOP_Command;
    public event Func<int, string, HCACKValule>?                                                 PPSELECT_Command;

    public GOL_SecsGem(string id, string name, string version)
    {
        Start(id, name, version);

        RemoteCommand += r =>
                         {
                             if (r.CommandName == "ADDLOT")
                             {
                                 if (r.RemoteCommandParameters is { Count: < 5 })
                                 {
                                     return new RemoteCommandResponse(HCACKValule.ParameterInvalid);
                                 }

                                 if (r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o1) &&
                                     o1 is uint index                                               &&
                                     r.RemoteCommandParameters.TryGetValue("LotID", out var o2)     &&
                                     o2 is string lot                                               &&
                                     r.RemoteCommandParameters.TryGetValue("PartID", out var o3)    &&
                                     o3 is string part                                              &&
                                     r.RemoteCommandParameters.TryGetValue("Layer", out var o4)     &&
                                     o4 is uint layer                                               &&
                                     r.RemoteCommandParameters.TryGetValue("Quantity", out var o5)  &&
                                     o5 is uint quantity)
                                 {
                                     return new RemoteCommandResponse(ADDLOT_Command?.Invoke((int)index, (lot, part, (int)layer, (int)quantity)) ?? HCACKValule.ParameterInvalid);
                                 }

                                 return new RemoteCommandResponse(HCACKValule.CantPerform);
                             }
                             if (r.CommandName == "CANCEL")
                             {
                                 if (r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o) && o is uint index)
                                 {
                                     return new RemoteCommandResponse(CANCEL_Command?.Invoke((int)index) ?? HCACKValule.ParameterInvalid);
                                 }

                                 return new RemoteCommandResponse(HCACKValule.CantPerform);
                             }
                             if (r.CommandName == "PP_SELECT")
                             {
                                 return new RemoteCommandResponse(r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o1) &&
                                                                  o1 is uint index                                               &&
                                                                  r.RemoteCommandParameters.TryGetValue("PPID", out var o2)      &&
                                                                  o2 is string ppid ?
                                                                      PPSELECT_Command?.Invoke((int)index, ppid) ?? HCACKValule.ParameterInvalid :
                                                                      HCACKValule.CantPerform);
                             }
                             if (r.CommandName == "START")
                             {
                                 return new RemoteCommandResponse(r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o) && o is uint index ?
                                                                      START_Command?.Invoke((int)index) ?? HCACKValule.ParameterInvalid :
                                                                      HCACKValule.CantPerform);
                             }
                             if (r.CommandName == "STOP")
                             {
                                 return new RemoteCommandResponse(r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o) && o is uint index ?
                                                                      STOP_Command?.Invoke((int)index) ?? HCACKValule.ParameterInvalid :
                                                                      HCACKValule.CantPerform);
                             }
                             if (r.CommandName == "GO_LOCAL")
                             {
                                 return new RemoteCommandResponse(AxQGWrapper?.OnLineLocal() == 0 ? HCACKValule.Acknowledge : HCACKValule.CantPerform);
                             }
                             if (r.CommandName == "GO_REMOTE")
                             {
                                 return new RemoteCommandResponse(AxQGWrapper?.OnLineRemote() == 0 ? HCACKValule.Acknowledge : HCACKValule.CantPerform);
                             }

                             return new RemoteCommandResponse(HCACKValule.CmdNotExist);
                         };
    }
}