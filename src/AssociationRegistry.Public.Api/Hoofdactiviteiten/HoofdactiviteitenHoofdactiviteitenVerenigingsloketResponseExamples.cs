namespace AssociationRegistry.Public.Api.Hoofdactiviteiten;

using Swashbuckle.AspNetCore.Filters;

public class HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponseExamples : IExamplesProvider<HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse>
{
    public HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse GetExamples()
        => new(
            new[]
            {
                new HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponse.HoofdactiviteitVerenigingsloket("BLA", "Bijzonder Lange Afkortingen"),
            });
}
