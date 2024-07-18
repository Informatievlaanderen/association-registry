namespace AssociationRegistry.Magda.Models.RegistreerUitschrijving;

using Hosts.Configuration.ConfigurationBindings;
using Repertorium.RegistreerUitschrijving;
using System.Xml.Serialization;

[Serializable]
public class RegistreerUitschrijvingBody
{
    [XmlElement(Namespace = "http://webservice.registreeruitschrijvingdienst-02_01.repertorium-02_01.vip.vlaanderen.be")]
    public RegistreerUitschrijving RegistreerUitschrijving { get; set; } = null!;

    public static RegistreerUitschrijvingBody CreateRequest(string kboNummer, Guid reference, MagdaOptionsSection magdaOptionsSection)
        => new()
        {
            RegistreerUitschrijving = new RegistreerUitschrijving
            {
                Verzoek = new VerzoekType
                {
                    Context = new ContextType
                    {
                        Naam = "RegistreerUitschrijving",
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
                                Uitschrijving = new UitschrijvingType
                                {
                                    Identificatie = "",
                                    Hoedanigheid = "",
                                },
                            },
                        },
                    },
                },
            },
        };
}
