namespace AssociationRegistry.Integrations.Magda.Persoon.Models;

using GeefPersoon;
using System.Xml.Serialization;

[Serializable]
public class GeefPersoonResponseBody
{
    [XmlElement(Namespace = "http://magda.vlaanderen.be/persoon/soap/geefpersoon/v02_02")]
    public GeefPersoonResponse? GeefPersoonResponse { get; set; }

    [XmlElement(
        "Fault",
        Namespace = "http://schemas.xmlsoap.org/soap/envelope/"
    )]
    public SoapFault? Fault { get; set; }
}
