namespace AssociationRegistry.Integrations.Magda.Models.GeefOndernemingVKBO;

using Onderneming.GeefOndernemingVKBO;
using System.Xml.Serialization;

[Serializable]
public class GeefOndernemingVKBOResponseBody
{
    [XmlElement(Namespace = "http://webservice.geefondernemingvkbodienst-02_00.onderneming-02_00.vip.vlaanderen.be")]
    public GeefOndernemingVKBOResponse? GeefOndernemingVKBOResponse { get; set; }
}
