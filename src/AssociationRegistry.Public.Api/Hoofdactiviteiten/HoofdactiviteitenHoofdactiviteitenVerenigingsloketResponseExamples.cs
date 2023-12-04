namespace AssociationRegistry.Public.Api.Hoofdactiviteiten;

using Swashbuckle.AspNetCore.Filters;

public class HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponseExamples : IExamplesProvider<HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse>
{
    public HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse GetExamples()
        => new()
        {
            HoofdactiviteitenVerenigingsloket =
                new[]
                {
                    new HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse.HoofdactiviteitVerenigingsloket
                    {
                        Code = "BLA",
                        Naam = "Bijzonder Lange Afkortingen",
                    },
                },
        };
}
