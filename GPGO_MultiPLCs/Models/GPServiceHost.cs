using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GPGO_MultiPLCs.Interfaces;

namespace GPGO_MultiPLCs.Models;
public class GPServiceHost : IGPOvenServerService
{
    public WebServiceResponse macIntf(string methodInvoke, string input)
    {
        Debug.WriteLine($"method:{methodInvoke}\ninput:{input}");
        return new WebServiceResponse();
    }
    public Task<WebServiceResponse> macIntfAsync(string methodInvoke, string input)
    {
        return Task.Factory.StartNew(_ =>
        {
            return new WebServiceResponse();
        }, TaskCreationOptions.LongRunning);
    }
}

