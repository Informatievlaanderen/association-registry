namespace AssociationRegistry.Integrations.Magda.Models.RegistreerInschrijving;

using Hosts.Configuration.ConfigurationBindings;
using Repertorium.RegistreerInschrijving;
using System.Xml.Serialization;

[Serializable]
public class RegistreerInschrijvingBody
{
    private const string Onderneming = "OND";

    [XmlElement(Namespace = "http://webservice.registreerinschrijvingdienst-02_01.repertorium-02_01.vip.vlaanderen.be")]
    public RegistreerInschrijving RegistreerInschrijving { get; set; } = null!;

    public static RegistreerInschrijvingBody CreateRequest(string kboNummer, Guid reference, MagdaOptionsSection magdaOptionsSection)
    {
        return new RegistreerInschrijvingBody
        {
            RegistreerInschrijving = new RegistreerInschrijving
            {
                Verzoek = new VerzoekType
                {
                    Context = new ContextType
                    {
                        Naam = "RegistreerInschrijving",
                        Versie = "02.01.0000",
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
                                Hoedanigheid = magdaOptionsSection.Hoedanigheid,
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
                                Inschrijving = new InschrijvingType
                                {
                                    BetrokkenSubject = new BetrokkenSubjectType
                                    {
                                        Subjecten = new[]
                                        {
                                            new SubjectType
                                            {
                                                Sleutel = kboNummer,
                                                Type = Onderneming,
                                            },
                                        },
                                        Type = Onderneming,
                                    },
                                    Hoedanigheid = magdaOptionsSection.Hoedanigheid,
                                    Identificatie = magdaOptionsSection.Afzender,
                                },
                            },
                        },
                    },
                },
            },
        };
    }
}
