namespace AssociationRegistry.Test.Middleware;

using Integrations.Magda.Models;
using Integrations.Magda.Models.GeefPersoon;
using Integrations.Magda.Persoon.GeefPersoon;

public class MagdaTestResponseFactory
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
    };    public static readonly ResponseEnvelope<GeefPersoonResponseBody> NietOverledenPersoon = new()
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
