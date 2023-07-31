using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCC_Reference2;

namespace GPGO_MultiPLCs.Models;
internal class SCC_Service : IMacIntfWS
{
    //SCC_Reference.WebServiceResponse IMacIntfWS.macIntf(string methodInvoke, string input)
    //{
    //    return new SCC_Reference.WebServiceResponse();
    //}
    //Task<SCC_Reference.WebServiceResponse> IMacIntfWS.macIntfAsync(string methodInvoke, string input)
    //{
    //    return Task.Factory.StartNew(_ =>
    //    {
    //        return new SCC_Reference.WebServiceResponse();
    //    },TaskCreationOptions.LongRunning);
    //}
    public macIntfResponse macIntf(macIntfRequest request)
    {
        return new macIntfResponse();
    }
    //public Task<macIntfResponse> macIntfAsync(macIntfRequest request)  
    //{
    //    return Task.Factory.StartNew(_ =>
    //    {
    //        return new macIntfResponse();
    //    },TaskCreationOptions.LongRunning);
    //}
}
