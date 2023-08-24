using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.Interfaces;
[ServiceContract]
public interface IGPOvenServerService
{
    [OperationContract]
    WebServiceResponse macIntf(string methodInvoke, string input);
    [OperationContract]
    Task<WebServiceResponse> macIntfAsync(string methodInvoke, string input);
}