namespace AssociationRegistry.Magda.Models;

using System;
using System.Xml.Serialization;
using Onderneming.GeefOndernemingVKBO;

[Serializable]
public class GeefOndernemingResponseBody
{
    [XmlElement(Namespace = "http://onderneming-02_00.vip.vlaanderen.be")]
    public GeefOndernemingVKBOResponse? GeefOndernemingResponse { get; set; }
}
