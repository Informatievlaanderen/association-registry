namespace AssociationRegistry.Magda.Models.GeefOndernemingVKBO;

using Hosts.Configuration.ConfigurationBindings;
using Onderneming.GeefOndernemingVKBO;
using System.Xml.Serialization;

[Serializable]
public class GeefOndernemingVKBOBody
{
    [XmlElement(Namespace = "http://webservice.geefondernemingvkbodienst-02_00.onderneming-02_00.vip.vlaanderen.be")]
    public GeefOndernemingVKBO GeefOnderneming { get; set; } = null!;

    public static GeefOndernemingVKBOBody CreateRequest(string kboNummer, Guid reference, MagdaOptionsSection magdaOptionsSection)
        => new()
        {
            GeefOnderneming = new GeefOndernemingVKBO
            {
                Verzoek = new VerzoekType
                {
                    Context = new ContextType
                    {
                        Naam = "GeefOndernemingVKBO",
                        Versie = "02.00.0000",
                        Bericht = new BerichtType
                        {
                            Type = BerichtTypeType.VRAAG,
                            Tijdstip = new TijdstipType
                            {
                                Datum = DateTime.Now.ToString("yyyy-MM-dd"),
                                Tijd = DateTime.Now.ToString("HH:mm:ss.000"),
                            },
                            Afzender = new AfzenderAdresType
                            {
                                Identificatie = magdaOptionsSection.Afzender,
                                Referte = reference.ToString(),
                            },
                            Ontvanger = new OntvangerAdresType
                            {
                                Identificatie = magdaOptionsSection.Ontvanger,
                            },
                        },
                    },
                    Vragen = new VragenType
                    {
                        Vraag = new VraagType
                        {
                            Referte = reference.ToString(),
                            Inhoud = new VraagInhoudType
                            {
                                Ondernemingsnummer = kboNummer,
                            },
                        },
                    },
                },
            },
        };
}
