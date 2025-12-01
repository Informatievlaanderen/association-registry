namespace AssociationRegistry.Integrations.Magda.Models.GeefPersoon;

using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using Persoon.GeefPersoon;
using System.Xml.Serialization;

[Serializable]
public class GeefPersoonResponseBody
{
    [XmlElement(Namespace = "http://magda.vlaanderen.be/persoon/soap/geefpersoon/v02_02")]
    public GeefPersoonResponse? GeefPersoonResponse { get; set; }
}
