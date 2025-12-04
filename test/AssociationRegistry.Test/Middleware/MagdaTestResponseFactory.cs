namespace AssociationRegistry.Test.Middleware;

using Integrations.Magda.Persoon.GeefPersoon;
using Integrations.Magda.Persoon.Models;
using Integrations.Magda.Persoon.Models.RegistreerInschrijving0200;
using Integrations.Magda.Repertorium.RegistreerInschrijving0200;
using Integrations.Magda.Shared.Models;
using AntwoordenType = Integrations.Magda.Persoon.GeefPersoon.AntwoordenType;
using AntwoordInhoudType = Integrations.Magda.Persoon.GeefPersoon.AntwoordInhoudType;
using AntwoordType = Integrations.Magda.Persoon.GeefPersoon.AntwoordType;
using BerichtType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.BerichtType;
using ContextType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.ContextType;
using OntvangerAdresType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.OntvangerAdresType;
using RepliekType = Integrations.Magda.Persoon.GeefPersoon.RepliekType;
using UitzonderingType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.UitzonderingType;

public static class MagdaTestResponseFactory
{
    public static class GeefPersoonResponses
    {
        public static readonly ResponseEnvelope<GeefPersoonResponseBody> OverledenPersoon = new()
        {
            Body = new GeefPersoonResponseBody()
            {
                GeefPersoonResponse = new GeefPersoonResponse()
                {
                    Repliek = new RepliekType()
                    {
                        Antwoorden = new AntwoordenType()
                        {
                            Antwoord = new AntwoordType()
                            {
                                Inhoud = new AntwoordInhoudType()
                                {
                                    Persoon = new PersoonType()
                                    {
                                        Overlijden = new OverlijdenType()
                                        {
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            },
        };

        public static readonly ResponseEnvelope<GeefPersoonResponseBody> NietOverledenPersoon = new()
        {
            Body = new GeefPersoonResponseBody()
            {
                GeefPersoonResponse = new GeefPersoonResponse()
                {
                    Repliek = new RepliekType()
                    {
                        Antwoorden = new AntwoordenType()
                        {
                            Antwoord = new AntwoordType()
                            {
                                Inhoud = new AntwoordInhoudType()
                                {
                                    Persoon = new PersoonType(),
                                },
                            },
                        },
                    },
                },
            },
        };
    }

    public static class RegistreerInschrijvingPersoonResponses
    {
        public static readonly ResponseEnvelope<RegistreerInschrijvingResponseBody> WelGeslaagd = new()
        {
            Body = new RegistreerInschrijvingResponseBody
            {
                RegistreerInschrijvingResponse = new RegistreerInschrijvingResponse()
                {
                    Repliek = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.RepliekType()
                    {
                        Context = CreateContext(),
                        Antwoorden = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.AntwoordenType()
                        {
                            Antwoord = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.AntwoordType()
                            {
                                Inhoud = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.AntwoordInhoudType()
                                {
                                    Resultaat = new ResultaatCodeType()
                                    {
                                        Value = ResultaatEnumType.Item1,
                                        Beschrijving = "Wel geslaagd",
                                    },
                                },
                            },
                        },
                    },
                },
            },
        };


        public static ResponseEnvelope<RegistreerInschrijvingResponseBody> NietGeslaagd(string foutCode) => new()
        {
            Body = new RegistreerInschrijvingResponseBody
            {
                RegistreerInschrijvingResponse = new RegistreerInschrijvingResponse()
                {
                    Repliek = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.RepliekType()
                    {
                        Context = CreateContext(),
                        Antwoorden = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.AntwoordenType()
                        {
                            Antwoord = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.AntwoordType()
                            {
                                Inhoud = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.AntwoordInhoudType()
                                {
                                    Resultaat = new ResultaatCodeType()
                                    {
                                        Value = ResultaatEnumType.Item0,
                                        Beschrijving = "Niet geslaagd",
                                    },
                                },
                                Uitzonderingen = new []
                                {
                                    new UitzonderingType()
                                    {
                                        Identificatie = foutCode,
                                    }
                                }
                            },
                        },
                    },
                },
            },
        };

        private static ContextType CreateContext()
            => new()
            {
                Bericht = new BerichtType()
                {
                    Ontvanger = new OntvangerAdresType()
                    {
                        Referte = "something",
                    }
                }
            };
    }
}
