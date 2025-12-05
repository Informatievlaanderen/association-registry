namespace AssociationRegistry.Integrations.Magda.Persoon.Models;

using Hosts.Configuration.ConfigurationBindings;
using GeefPersoon;
using System;
using System.Xml.Serialization;

[Serializable]
public class GeefPersoonBody
{
    [XmlElement(Namespace = "http://magda.vlaanderen.be/persoon/soap/geefpersoon/v02_02")]
    public GeefPersoon GeefPersoon { get; set; } = null!;

    public static GeefPersoonBody CreateRequest(
        string insz,
        Guid reference,
        MagdaOptionsSection magdaOptionsSection)
        => new()
        {
            GeefPersoon = new GeefPersoon
            {
                Verzoek = new VerzoekType
                {
                    Context = new ContextType
                    {
                        Naam = "GeefPersoon",
                        Versie = "02.02.0000",
                        Bericht = new BerichtType
                        {
                            Type = BerichtTypeType.VRAAG,
                            Tijdstip = new TijdstipType
                            {
                                Datum = DateTime.Now.ToString("yyyy-MM-dd"),
                                Tijd  = DateTime.Now.ToString("HH:mm:ss.000"),
                            },
                            Afzender = new AfzenderAdresType
                            {
                                Identificatie = magdaOptionsSection.Afzender,
                                Referte       = reference.ToString("D"),
                                Hoedanigheid  = "6001",
                            },
                            Ontvanger = new OntvangerAdresType
                            {
                                Identificatie = magdaOptionsSection.Ontvanger,
                                Naam          = "VIP",
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
                                Bron    = "KSZ",
                                Taal    = TaalCodeType.nl,
                                Criteria = new CriteriaGeefPersoonType
                                {
                                    INSZ = insz,
                                    OpvragingenKSZ = new OpvragingenKSZType()
                                    {
                                        BasisPersoonsgegevens = VlagCodeType.Item1,
                                        BasisPersoonsgegevensSpecified = true,
                                    }
                                },
                            },
                        },
                    },
                },
            },
        };
}
