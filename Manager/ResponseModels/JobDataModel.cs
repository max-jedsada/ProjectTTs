using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Manager.ResponseModels;

public class Feature
{
    public string? type { get; set; }
    public Properties properties { get; set; } = new Properties();
    public Geometry geometry { get; set; } = new Geometry();
    public string? id { get; set; }
}

public class Geometry
{
    public string? type { get; set; }
    public List<decimal?> coordinates { get; set; } = new List<decimal?>();
}

public class Metadata
{
    public long generated { get; set; }
    public string? url { get; set; }
    public string? title { get; set; }
    public decimal? status { get; set; }
    public string? api { get; set; }
    public decimal? count { get; set; }
}

public class Properties
{
    public decimal? mag { get; set; }
    public string? place { get; set; }
    public object time { get; set; }
    public object updated { get; set; }
    public object tz { get; set; }
    public string? url { get; set; }
    public string? detail { get; set; }
    public object felt { get; set; }
    public object cdi { get; set; }
    public object mmi { get; set; }
    public object alert { get; set; }
    public string? status { get; set; }
    public decimal? tsunami { get; set; }
    public decimal? sig { get; set; }
    public string? net { get; set; }
    public string? code { get; set; }
    public string? ids { get; set; }
    public string? sources { get; set; }
    public string? types { get; set; }
    public decimal? nst { get; set; }
    public decimal? dmin { get; set; }
    public decimal? rms { get; set; }
    public decimal? gap { get; set; }
    public string? magType { get; set; }
    public string? type { get; set; }
    public string? title { get; set; }

    public string? Level { get; set; }
    public string? RegionId { get; set; }
    public string? RegionName { get; set; }
    public string? LocationCoordinates { get; set; }
    public string? DisasterTypes { get; set; }

    public bool AlertTriggered { get; set; } = false;
    public string? AlertMessage { get; set; }
    public string? Timestamp { get; set; }

}

public class JobDataModel
{
    public string? type { get; set; }
    public Metadata metadata { get; set; } = new Metadata();
    public List<Feature> features { get; set; } = new List<Feature>();
    public List<decimal?> bbox { get; set; } = new List<decimal?>();
}

public class SettingModel
{
    public string? RegionId { get; set; }
    public List<LocationCoordinates> LocationCoordinates { get; set; } = new List<LocationCoordinates>();
    public List<string?> DisasterTypes { get; set; }
}

public class LocationCoordinates
{
    public decimal? Lat { get; set; }
    public decimal? Long { get; set; }
}

public class SettingAlertModel
{
    public string? RegionId { get; set; }
    public string? DisasterTypes { get; set; }
    public string? ThresholdScore { get; set; }
}
