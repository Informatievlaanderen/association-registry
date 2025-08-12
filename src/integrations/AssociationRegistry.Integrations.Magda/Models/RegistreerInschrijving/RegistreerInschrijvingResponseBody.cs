namespace AssociationRegistry.Integrations.Magda.Models.RegistreerInschrijving;

using AssociationRegistry.Integrations.Magda.Repertorium.RegistreerInschrijving;
using System;
using System.Xml.Serialization;

[Serializable]
public class RegistreerInschrijvingResponseBody
{
    [XmlElement(Namespace = "http://webservice.registreerinschrijvingdienst-02_01.repertorium-02_01.vip.vlaanderen.be")]
    public RegistreerInschrijvingResponse? RegistreerInschrijvingResponse { get; set; }
}
