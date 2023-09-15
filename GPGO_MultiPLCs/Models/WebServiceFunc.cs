using System.Xml.Serialization;

namespace GPGO_MultiPLCs.Models;
public class WebServiceFunc
{
    public class CallAgv
    {
        [XmlAttribute]
        public string? macCode;
        [XmlAttribute]
        public string? wipEntity;
        [XmlAttribute]
        public string? berthCode;
    }
}
