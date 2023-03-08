using System;
using GPMVVM.Models.SECS;
using GPMVVM.SECSGEM;

namespace GPGO_MultiPLCs.Models;

public class GOL_SecsGem : SECSGEM_Equipment
{
    public event Func<int, (string LotID, string PartID, int layer, int quantity), HCACKValule>? AddLOT;
    public event Func<int, HCACKValule>?                                                         CANCEL;
    public event Func<int, HCACKValule>?                                                         Start;
    public event Func<int, HCACKValule>?                                                         Stop;
    public event Func<int, string, HCACKValule>?                                                 SetRecipe;
    public event Func<string, HCACKValule>?                                                      RetrieveLotData;

    public GOL_SecsGem(string id, string name, string version) : base(id, name, version) => RemoteCommand += r =>
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
                                                                                                                         return new RemoteCommandResponse(AddLOT?.Invoke((int)index, (lot, part, (int)layer, (int)quantity)) ?? HCACKValule.ParameterInvalid);
                                                                                                                     }

                                                                                                                     return new RemoteCommandResponse(HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "CANCEL")
                                                                                                                 {
                                                                                                                     if (r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o) && o is uint index)
                                                                                                                     {
                                                                                                                         return new RemoteCommandResponse(CANCEL?.Invoke((int)index) ?? HCACKValule.ParameterInvalid);
                                                                                                                     }

                                                                                                                     return new RemoteCommandResponse(HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "PP_SELECT")
                                                                                                                 {
                                                                                                                     return new RemoteCommandResponse(r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o1) &&
                                                                                                                                                      o1 is uint index                                               &&
                                                                                                                                                      r.RemoteCommandParameters.TryGetValue("PPID", out var o2)      &&
                                                                                                                                                      o2 is string ppid ?
                                                                                                                                                          SetRecipe?.Invoke((int)index, ppid) ?? HCACKValule.ParameterInvalid :
                                                                                                                                                          HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "START")
                                                                                                                 {
                                                                                                                     return new RemoteCommandResponse(r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o) && o is uint index ?
                                                                                                                                                          Start?.Invoke((int)index) ?? HCACKValule.ParameterInvalid :
                                                                                                                                                          HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "STOP")
                                                                                                                 {
                                                                                                                     return new RemoteCommandResponse(r.RemoteCommandParameters.TryGetValue("OvenIndex", out var o) && o is uint index ?
                                                                                                                                                          Stop?.Invoke((int)index) ?? HCACKValule.ParameterInvalid :
                                                                                                                                                          HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "RETRIEVELOTDATA")
                                                                                                                 {
                                                                                                                     return new RemoteCommandResponse(r.RemoteCommandParameters.TryGetValue("LOTID", out var o) && o is string lotid && !string.IsNullOrEmpty(lotid) ?
                                                                                                                                                          RetrieveLotData?.Invoke(lotid) ?? HCACKValule.ParameterInvalid :
                                                                                                                                                          HCACKValule.CantPerform);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "GO_LOCAL")
                                                                                                                 {
                                                                                                                     return new RemoteCommandResponse(SecsGem?.AxQGWrapper.OnLineLocal() == 0 ? HCACKValule.CantPerform : HCACKValule.Acknowledge);
                                                                                                                 }
                                                                                                                 if (r.CommandName == "GO_REMOTE")
                                                                                                                 {
                                                                                                                     new RemoteCommandResponse(SecsGem?.AxQGWrapper.OnLineRemote() == 0 ? HCACKValule.CantPerform : HCACKValule.Acknowledge);
                                                                                                                 }

                                                                                                                 return new RemoteCommandResponse(HCACKValule.CmdNotExist);
                                                                                                             };
}