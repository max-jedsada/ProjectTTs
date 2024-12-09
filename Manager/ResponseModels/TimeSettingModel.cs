
namespace Manager.Model;

public class TimeSettingModel
{
    public int? Id { get; set; }

    public int? AirportId { get; set; }
    public bool? IsAllAirport { get; set; }

    public int TimeOpenCheckinDom { get; set; }
    public int TimeCloseCheckinDom { get; set; }
    public int TimeOpenCheckinInter { get; set; }
    public int TimeCloseCheckinInter { get; set; }
    public int TimeOpenBelt { get; set; }
    public int TimeCloseBelt { get; set; }
    public int TimeOpenGate { get; set; }
    public int TimeOCloseGate { get; set; }

}
