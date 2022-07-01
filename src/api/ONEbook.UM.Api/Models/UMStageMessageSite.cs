using System.Xml.Serialization;

namespace ONEbook.UM.Api.Models;

[XmlRoot(ElementName = "UMStageMessageSite")]
public class UMStageMessageSite
{
    [XmlElement(ElementName = "VersionChkMessage")]
    public string VersionChkMessage { get; set; } = "";

    [XmlArrayItem(typeof(MessageSite), ElementName = "MessageSite")]
    [XmlArray("MessageSites")]
    public MessageSite[] MessageSites { get; set; }
}