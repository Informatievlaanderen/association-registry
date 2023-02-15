namespace AssociationRegistry.Public.Api.Hoofdactiviteiten;

using Swashbuckle.AspNetCore.Filters;

public class HoofdactiviteitenResponseExamples : IExamplesProvider<HoofdactiviteitenResponse>
{
    public HoofdactiviteitenResponse GetExamples()
        => new(
            new[]
            {
                new HoofdactiviteitenResponse.Hoofdactiviteit("BLA", "Bijzonder Lange Afkortingen"),
            });
}
