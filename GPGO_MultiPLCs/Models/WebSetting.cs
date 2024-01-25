using MongodbConnect.DataClass;

namespace GPGRC_MultiPLCs.Models;
public class WebSetting : MongodbDataBaseClass
{
    public string EquipmentID { get; set; }
    public string iMESURL { get; set; }
    public string CallCarrierID { get; set; }
    public string OutCarrierID { get; set; }
    public string NGCarrierID { get; set; }
    public int AVGTime { get; set; }
    public int Timeout { get; set; }

    public bool UseHeart { get; set; }

    public int HeartTime { get; set; }
    public string HeartContent { get; set; }
    public int HeartPort { get; set; }
    public string HeartService { get; set; }
    public WebSetting(string equipmentID, string imesurl, string callcarrierID, string outcarrierID, string ngcarrierID, int aVGTime, int timeOut, bool useheart, int heartime, string heartcontent, int heartport, string heartservice)
    {
        EquipmentID = equipmentID;
        iMESURL = imesurl;
        CallCarrierID = callcarrierID;
        OutCarrierID = outcarrierID;
        NGCarrierID = ngcarrierID;
        AVGTime = aVGTime;
        Timeout = timeOut;
        UseHeart = useheart;
        HeartTime = heartime;
        HeartContent = heartcontent;
        HeartPort = heartport;
        HeartService = heartservice;
    }

}