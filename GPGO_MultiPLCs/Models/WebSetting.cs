using MongodbConnect.DataClass;

namespace GPGO_MultiPLCs.Models;
public class WebSetting : MongodbDataBaseClass
{
    public string EquipmentID { get; set; }
    public string iMESURL { get; set; }
    public string CarrierAID { get; set; }
    public string CarrierBID { get; set; }
    public int AVGTime { get; set; }
    public int Timeout { get; set; }

    public bool UseHeart { get; set; }

    public int HeartTime { get; set; }
    public string HeartContent { get; set; }
    public string HeartPort { get; set; }
    public string HeartService { get; set; }
    public WebSetting(string equipmentID, string imesurl, string carrierAID, string carrierBID, int aVGTime, int timeOut, bool useheart, int heartime, string heartcontent, string heartport, string heartservice)
    {
        EquipmentID = equipmentID;
        iMESURL = imesurl;
        CarrierAID = carrierAID;
        CarrierBID = carrierBID;
        AVGTime = aVGTime;
        Timeout = timeOut;
        UseHeart = useheart;
        HeartTime = heartime;
        HeartContent = heartcontent;
        HeartPort = heartport;
        HeartService = heartservice;
    }

}