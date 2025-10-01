namespace AssociationRegistry.Integrations.Magda.GeefOnderneming.Models;

using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using System.Xml.Serialization;

[Serializable]
public class GeefOndernemingBody
{
    [XmlElement(Namespace = "http://webservice.geefondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be")]
    public GeefOnderneming GeefOnderneming { get; set; } = null!;

    public static GeefOndernemingBody CreateRequest(string kboNummer, Guid reference, MagdaOptionsSection magdaOptionsSection)
        => new()
        {
            GeefOnderneming = new GeefOnderneming
            {
                Verzoek = new VerzoekType
                {
                    Context = new ContextType
                    {
                        Naam = "GeefOnderneming",
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
                                Taal = "nl",
                                Criteria = new CriteriaGeefOnderneming2_0Type
                                {
                                    Ondernemingsnummer = kboNummer,
                                    Basisgegevens = VlagEnumType.Item1,
                                    // Activiteiten = VlagEnumType.Item1,
                                    // Bankrekeningen = VlagEnumType.Item1,
                                    // Bijhuis = VlagEnumType.Item1,
                                    Functies = new CriteriaFunctiesType()
                                    {
                                        Aanduiding = VlagEnumType.Item1,
                                        Onderneming = VlagEnumType.Item1
                                    }
                                },
                            },
                        },
                    },
                },
            },
        };
}
