namespace AssociationRegistry.Integrations.Magda.Persoon.Models.RegistreerInschrijving0200;

using AssociationRegistry.Integrations.Magda.Repertorium.RegistreerInschrijving0200;
using System;
using System.Xml.Serialization;

[Serializable]
public class RegistreerInschrijvingResponseBody
{
    [XmlElement(Namespace = "http://webservice.registreerinschrijvingdienst-02_00.repertorium-02_00.vip.vlaanderen.be")]
    public RegistreerInschrijvingResponse? RegistreerInschrijvingResponse { get; set; }

    [XmlElement(
        "Fault",
        Namespace = "http://schemas.xmlsoap.org/soap/envelope/"
    )]
    public SoapFault? Fault { get; set; }
}

[Serializable]
[XmlRoot("Fault", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class SoapFault
{
    [XmlElement("faultcode")]
    public string? FaultCode { get; set; }

    [XmlElement("faultstring")]
    public string? FaultString { get; set; }

    [XmlElement("detail")]
    public SoapFaultDetail? Detail { get; set; }
}

[Serializable]
public class SoapFaultDetail
{
    [XmlElement("message")]
    public string? Message { get; set; }
}


