namespace AssociationRegistry.Public.Api.DetailVerenigingen;

using System;
using System.Collections.Immutable;
using Swashbuckle.AspNetCore.Filters;

public class DetailVerenigingResponseExamples : IExamplesProvider<DetailVerenigingResponse>
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public DetailVerenigingResponse GetExamples()
        => new(
            $"{_appSettings.BaseUrl}api/v1/contexten/detail-vereniging-context.json",
            new VerenigingDetail(
                "V123",
                "Voorbeeld Vereniging",
                "VV",
                "",
                "Vereniging",
                DateOnly.FromDateTime(new DateTime(2022,09,27)),
                null,
                new Locatie("2000", "Antwerpen"),
                null!,
                ImmutableArray<Locatie>.Empty,
                ImmutableArray<Activiteit>.Empty,
                ImmutableArray<ContactGegeven>.Empty,
                DateOnly.FromDateTime(new DateTime(2022,09,27))
            )); // TODO complete good example !
}
