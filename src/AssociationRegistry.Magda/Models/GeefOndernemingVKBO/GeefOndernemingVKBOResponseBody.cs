namespace AssociationRegistry.Magda.Models.GeefOndernemingVKBO;

using System;
using System.Xml.Serialization;
using AssociationRegistry.Magda.Onderneming.GeefOndernemingVKBO;

[Serializable]
public class GeefOndernemingVKBOResponseBody
{
    [XmlElement(Namespace = "http://webservice.geefondernemingvkbodienst-02_00.onderneming-02_00.vip.vlaanderen.be")]
    public GeefOndernemingVKBOResponse? GeefOndernemingVKBOResponse { get; set; }
}
