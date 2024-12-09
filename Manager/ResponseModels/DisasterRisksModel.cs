
namespace Manager.Model;

public class DisasterRisksModel
{
    public string RegionId { get; set; }
    public string RegionName { get; set; }
    public string DisasterTypes { get; set; }
    public string RiskScore { get; set; }
    public string RiskLevel { get; set; }
    public string RiskLevelName { get; set; }
    public bool AlertTriggered { get; set; } = false;
    public string AlertMessage { get; set; }
    public string Timestamp { get; set; }

}
