namespace AssociationRegistry.Public.Api.WebApi.Hoofdactiviteiten;

using Swashbuckle.AspNetCore.Filters;

public class HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponseExamples : IExamplesProvider<
    HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse>
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
