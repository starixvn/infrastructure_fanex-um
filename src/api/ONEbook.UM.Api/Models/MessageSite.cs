using System.Xml.Serialization;
using ONEbook.UM.Api.Constants;

namespace ONEbook.UM.Api.Models;

public class MessageSite
{
    [XmlElement(ElementName = "SiteId")]
    public int SiteId { get; set; }

    [XmlElement(ElementName = "SiteName")]
    public string SiteName { get; set; }

    [XmlElement(ElementName = "StartTime")]
    public DateTime StartTime { get; set; }

    [XmlElement(ElementName = "EndTime")]
    public DateTime EndTime { get; set; }

    [XmlElement(ElementName = "Stage")]
    public UMStage Stage { get; set; }
}