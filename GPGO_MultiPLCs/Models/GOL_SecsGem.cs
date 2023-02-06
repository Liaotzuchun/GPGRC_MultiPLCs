using System;
using System.Collections.Generic;
using System.Linq;
using GPMVVM.PooledCollections;
using GPMVVM.SECSGEM;

namespace GPGO_MultiPLCs.Models;

public class GOL_SecsGem : SECSGEM_Equipment
{
    public event Func<int, (string LotID, string PartID, int layer, IList<string> Panels), HCACKValule>? AddLOT;
    public event Func<int, HCACKValule>?                                                                 CANCEL;
    public event Func<int, HCACKValule>?                                                                 Start;
    public event Func<int, HCACKValule>?                                                                 Stop;
    public event Func<int, string, HCACKValule>?                                                         SetRecipe;
    public event Func<string, HCACKValule>?                                                              RetrieveLotData;

    public GOL_SecsGem(string id) : base(id, "GP_GO") => RemoteCommand += r =>
                                                                          {
                                                                              if (r.RCMD == "ADDLOT")
                                                                              {
                                                                                  if (r.RemoteCommandParameter is { Count: < 5 })
                                                                                  {
                                                                                      return HCACKValule.ParameterInvalid;
                                                                                  }

                                                                                  if (r.RemoteCommandParameter?[0]?.CPVAL?.ObjectData is int[] { Length: > 0 } indexes &&
                                                                                      r.RemoteCommandParameter[1]?.CPVAL?.ObjectData is string lot                     &&
                                                                                      r.RemoteCommandParameter[2]?.CPVAL?.ObjectData is string part                    &&
                                                                                      r.RemoteCommandParameter[3]?.CPVAL?.ObjectData is int[] layers                   &&
                                                                                      r.RemoteCommandParameter[4]?.CPVAL is SECSMessageBranches { SECSMessageObjects: { } objs })
                                                                                  {
                                                                                      var       i      = indexes[0];
                                                                                      var       l      = layers[0];
                                                                                      using var panels = objs.Select(x => x?.ObjectData?.ToString() ?? string.Empty).ToPooledList();

                                                                                      return AddLOT?.Invoke(i, (lot, part, l, panels)) ?? HCACKValule.ParameterInvalid;
                                                                                  }

                                                                                  return HCACKValule.CantPerform;
                                                                              }
                                                                              if (r.RCMD == "CANCEL")
                                                                              {
                                                                                  if (r.RemoteCommandParameter is { Count: < 1 })
                                                                                  {
                                                                                      return HCACKValule.ParameterInvalid;
                                                                                  }

                                                                                  if (r.RemoteCommandParameter?[0]?.CPVAL?.ObjectData is int[] { Length: > 0 } indexes)
                                                                                  {
                                                                                      var i = indexes[0];

                                                                                      return CANCEL?.Invoke(i) ?? HCACKValule.ParameterInvalid;
                                                                                  }

                                                                                  return HCACKValule.CantPerform;
                                                                              }
                                                                              if (r.RCMD == "PP_SELECT")
                                                                              {
                                                                                  return r.RemoteCommandParameter is { Count: < 2 } ?
                                                                                             HCACKValule.ParameterInvalid :
                                                                                             r.RemoteCommandParameter?[0]?.CPVAL?.ObjectData is int[] o1 &&
                                                                                             r.RemoteCommandParameter?[1]?.CPVAL?.ObjectData is int[] o2 &&
                                                                                             int.TryParse(o1[0].ToString(), out var i) ?
                                                                                                 SetRecipe?.Invoke(i, o2.ToString()) ?? HCACKValule.ParameterInvalid :
                                                                                                 HCACKValule.CantPerform;
                                                                              }
                                                                              if (r.RCMD == "START")
                                                                              {
                                                                                  return r.RemoteCommandParameter is { Count: < 1 } ?
                                                                                             HCACKValule.ParameterInvalid :
                                                                                             r.RemoteCommandParameter?[0]?.CPVAL?.ObjectData is int[] o && int.TryParse(o[0].ToString(), out var i) ? Start?.Invoke(i) ?? HCACKValule.ParameterInvalid : HCACKValule.CantPerform;
                                                                              }
                                                                              if (r.RCMD == "STOP")
                                                                              {
                                                                                  return r.RemoteCommandParameter is { Count: < 1 } ?
                                                                                             HCACKValule.ParameterInvalid :
                                                                                             r.RemoteCommandParameter?[0]?.CPVAL?.ObjectData is int[] o && int.TryParse(o[0].ToString(), out var i) ?
                                                                                                 Stop?.Invoke(i) ?? HCACKValule.ParameterInvalid :
                                                                                                 HCACKValule.CantPerform;
                                                                              }
                                                                              if (r.RCMD == "RETRIEVELOTDATA")
                                                                              {
                                                                                  return r.RemoteCommandParameter is { Count: < 1 } ?
                                                                                             HCACKValule.ParameterInvalid :
                                                                                             r.RemoteCommandParameter?[0]?.CPVAL?.ObjectData is string lotid && !string.IsNullOrEmpty(lotid) ?
                                                                                                 RetrieveLotData?.Invoke(lotid) ?? HCACKValule.ParameterInvalid :
                                                                                                 HCACKValule.CantPerform;
                                                                              }
                                                                              if (r.RCMD == "GO_LOCAL")
                                                                              {
                                                                                  return SecsGem?.AxQGWrapper.OnLineLocal() == 0 ? HCACKValule.CantPerform : HCACKValule.Acknowledge;
                                                                              }
                                                                              if (r.RCMD == "GO_REMOTE")
                                                                              {
                                                                                  return SecsGem?.AxQGWrapper.OnLineRemote() == 0 ? HCACKValule.CantPerform : HCACKValule.Acknowledge;
                                                                              }

                                                                              return HCACKValule.CmdNotExist;
                                                                          };
}