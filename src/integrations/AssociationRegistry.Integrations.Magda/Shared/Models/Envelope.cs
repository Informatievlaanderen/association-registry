namespace AssociationRegistry.Integrations.Magda.Shared.Models;

using System.Xml.Serialization;

[XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
[Serializable]
public class Envelope<T>
{
    [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public Header? Header { get; set; }

    [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public T? Body { get; set; }
}

[XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
[Serializable]
public class ResponseEnvelope<T>
{
    [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public Header? Header { get; set; }

    [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public T? Body { get; set; }
}
