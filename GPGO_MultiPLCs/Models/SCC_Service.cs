using System;
using System.Threading.Tasks;
using SCC_ServerSideRef;

namespace GPGRC_MultiPLCs.Models;
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

    public Task<macIntfResponse> macIntfAsync(macIntfRequest request) => throw new NotImplementedException();
    //public Task<macIntfResponse> macIntfAsync(macIntfRequest request)  
    //{
    //    return Task.Factory.StartNew(_ =>
    //    {
    //        return new macIntfResponse();
    //    },TaskCreationOptions.LongRunning);
    //}
}
