namespace AssociationRegistry.Integrations.Magda.Persoon.Models.RegistreerUitschrijving;

using AssociationRegistry.Integrations.Magda.Repertorium.RegistreerUitschrijving;
using System.Xml.Serialization;

[Serializable]
public class RegistreerUitschrijvingResponseBody
{
    [XmlElement(Namespace = "http://webservice.registreeruitschrijvingdienst-02_01.repertorium-02_01.vip.vlaanderen.be")]
    public RegistreerUitschrijvingResponse? RegistreerUitschrijvingResponse { get; set; }
}
