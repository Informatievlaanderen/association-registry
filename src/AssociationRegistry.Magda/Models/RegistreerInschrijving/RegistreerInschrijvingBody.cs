namespace AssociationRegistry.Magda.Models.RegistreerInschrijving;

using Configuration;
using Repertorium.RegistreerInschrijving;
using System.Xml.Serialization;

[Serializable]
public class RegistreerInschrijvingBody
{
    [XmlElement(Namespace = "http://webservice.registreerinschrijvingdienst-02_01.repertorium-02_01.vip.vlaanderen.be")]
    public RegistreerInschrijving RegistreerInschrijving { get; set; } = null!;

    public static RegistreerInschrijvingBody CreateRequest(string kboNummer, Guid reference, MagdaOptionsSection magdaOptionsSection)
        => new()
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
                                Inschrijving = new InschrijvingType(),
                            },
                        },
                    },
                },
            },
        };
}
