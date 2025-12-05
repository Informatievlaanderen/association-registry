namespace AssociationRegistry.Integrations.Magda.Persoon.Models.RegistreerInschrijving0200;

using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Integrations.Magda.Repertorium.RegistreerInschrijving0200;
using System.Xml.Serialization;

[Serializable]
public class RegistreerInschrijvingPersoonBody
{

    [XmlElement(Namespace = "http://webservice.registreerinschrijvingdienst-02_00.repertorium-02_00.vip.vlaanderen.be")]
    public RegistreerInschrijving RegistreerInschrijving { get; set; } = null!;

    public static RegistreerInschrijvingPersoonBody CreateRequest(string insz, Guid reference, MagdaOptionsSection magdaOptionsSection)
        => new()
        {
            RegistreerInschrijving = new RegistreerInschrijving
            {
                Verzoek = new VerzoekType
                {
                    Context = new ContextType
                    {
                        Naam = "RegistreerInschrijving",
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
                                Referte = reference.ToString("D"),
                                Hoedanigheid = "6001",
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
                            Referte = reference.ToString("D"),
                            Inhoud = new VraagInhoudType
                            {
                                Inschrijving = new InschrijvingType
                                {
                                    Hoedanigheid = "6001",
                                    Identificatie = magdaOptionsSection.Afzender,
                                    INSZ = insz,
                                    Periode = new PeriodeVerplichtBeginType
                                    {
                                        Begin = DateTime.Now.ToString("yyyy-MM-dd"),
                                        Einde = null,
                                    },
                                },
                            },
                        },
                    },
                },
            },
        };
}
