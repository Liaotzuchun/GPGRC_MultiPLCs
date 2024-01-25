using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GPGRC_MultiPLCs.Models;
[DataContract]
public class DataUpload
{
    [DataMember]
    public string macCode { get; set; }

    [DataMember]
    public string wipEntity { get; set; }

    [DataMember]
    public List<Item> items { get; set; } = new List<Item>();
}

[DataContract]
public class Item
{
    [DataMember]
    public string tagCode { get; set; }

    [DataMember]
    public string tagValue { get; set; }

    [DataMember]
    public string timeStamp { get; set; }
}

[DataContract]
public class WebServiceResponse
{
    [DataMember]
    public string errorCodek__BackingField { get; set; }

    [DataMember]
    public string errorMsgk__BackingField { get; set; }

    [DataMember]
    public string resultDatak__BackingField { get; set; }
}
