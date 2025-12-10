namespace AssociationRegistry.Integrations.Magda.Persoon.Models;

using System.Xml.Serialization;

[Serializable]
[XmlRoot("Fault", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class SoapFault
{
    [XmlElement("faultcode", Namespace = "")]
    public string? FaultCode { get; set; }

    [XmlElement("faultstring", Namespace = "")]
    public string? FaultString { get; set; }

    [XmlElement("detail", Namespace = "")]
    public SoapFaultDetail? Detail { get; set; }
}

[Serializable]
public class SoapFaultDetail
{
    [XmlElement("message", Namespace = "")]
    public string? Message { get; set; }
}
