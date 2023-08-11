namespace AssociationRegistry.Admin.Api.Hoofdactiviteiten.Examples;

using ResponseModels;
using Swashbuckle.AspNetCore.Filters;

public class HoofdactiviteitenHoofdactiviteitenVerenigingsloketResponseExamples : IExamplesProvider<HoofdactiviteitenVerenigingsloketResponse>
{
    public HoofdactiviteitenVerenigingsloketResponse GetExamples()
        => new()
        {
            HoofdactiviteitenVerenigingsloket =
                new[]
                {
                    new HoofdactiviteitVerenigingsloketResponse
                    {
                        Code = "BLA",
                        Beschrijving = "Bijzonder Lange Afkortingen",
                    },
                },
        };
}
