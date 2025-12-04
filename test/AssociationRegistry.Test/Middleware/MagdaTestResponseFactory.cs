namespace AssociationRegistry.Test.Middleware;

using ImTools;
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
using UitzonderingTypeType = Integrations.Magda.Repertorium.RegistreerInschrijving0200.UitzonderingTypeType;

public static class MagdaTestResponseFactory
{
    public static class GeefPersoonResponses
    {
        public static ResponseEnvelope<GeefPersoonResponseBody> OverledenPersoon => new()
        {
            Body = new GeefPersoonResponseBody()
            {
                GeefPersoonResponse = new GeefPersoonResponse()
                {
                    Repliek = new RepliekType()
                    {
                        Context = CreatePersoonContext(),
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

        public static ResponseEnvelope<GeefPersoonResponseBody> NietOverledenPersoon => new()
        {
            Body = new GeefPersoonResponseBody()
            {
                GeefPersoonResponse = new GeefPersoonResponse()
                {
                    Repliek = new RepliekType()
                    {
                        Context = CreatePersoonContext(),
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

        public static ResponseEnvelope<GeefPersoonResponseBody> Fout(string foutCode, Integrations.Magda.Persoon.GeefPersoon.UitzonderingTypeType uitzonderingTypeType) => new()
        {
            Body = new GeefPersoonResponseBody()
            {
                GeefPersoonResponse = new GeefPersoonResponse()
                {
                    Repliek = new RepliekType()
                    {
                        Context = CreatePersoonContext(),
                        Antwoorden = new AntwoordenType()
                        {
                            Antwoord = new AntwoordType()
                            {
                                Uitzonderingen = new Integrations.Magda.Persoon.GeefPersoon.UitzonderingType[]
                                {
                                    new Integrations.Magda.Persoon.GeefPersoon.UitzonderingType()
                                    {
                                        Type = uitzonderingTypeType,
                                        Identificatie = foutCode,
                                    }
                                }
                            },
                        },
                    },
                },
            },
        };

        private static Integrations.Magda.Persoon.GeefPersoon.ContextType CreatePersoonContext()
            => new()
            {
                Bericht = new Integrations.Magda.Persoon.GeefPersoon.BerichtType()
                {
                    Ontvanger = new Integrations.Magda.Persoon.GeefPersoon.OntvangerAdresType()
                    {
                        Referte = Guid.NewGuid().ToString(),
                    }
                }
            };
    }

    public static class RegistreerInschrijvingPersoon
    {
        public static ResponseEnvelope<RegistreerInschrijvingResponseBody> WelGeslaagd => new()
        {
            Body = new RegistreerInschrijvingResponseBody
            {
                RegistreerInschrijvingResponse = new RegistreerInschrijvingResponse()
                {
                    Repliek = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.RepliekType()
                    {
                        Context = CreateInschrijvingContext(),
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


        public static ResponseEnvelope<RegistreerInschrijvingResponseBody> NietGeslaagd(string foutCode, UitzonderingTypeType uitzonderingTypeType) => new()
        {
            Body = new RegistreerInschrijvingResponseBody
            {
                RegistreerInschrijvingResponse = new RegistreerInschrijvingResponse()
                {
                    Repliek = new Integrations.Magda.Repertorium.RegistreerInschrijving0200.RepliekType()
                    {
                        Context = CreateInschrijvingContext(),
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
                                        Type = uitzonderingTypeType,
                                    }
                                }
                            },
                        },
                    },
                },
            },
        };

        private static ContextType CreateInschrijvingContext()
            => new()
            {
                Bericht = new BerichtType()
                {
                    Ontvanger = new OntvangerAdresType()
                    {
                        Referte = Guid.NewGuid().ToString(),
                    }
                }
            };
    }
}
