namespace AssociationRegistry.Magda.Models;

using System.Xml.Serialization;
using Onderneming.GeefOndernemingVKBO;

[Serializable]
public class GeefOndernemingBody
{
    [XmlElement(Namespace = "http://webservice.geefondernemingvkbodienst-02_00.onderneming-02_00.vip.vlaanderen.be")]
    public GeefOndernemingVKBO GeefOnderneming { get; set; } = null!;
}
