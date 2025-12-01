namespace AssociationRegistry.Integrations.Magda.Models.RegistreerInschrijving0200;

using Repertorium.RegistreerInschrijving0200;
using System;
using System.Xml.Serialization;

[Serializable]
public class RegistreerInschrijvingResponseBody
{
    [XmlElement(Namespace = "http://webservice.registreerinschrijvingdienst-02_00.repertorium-02_00.vip.vlaanderen.be")]
    public RegistreerInschrijvingResponse? RegistreerInschrijvingResponse { get; set; }
}
