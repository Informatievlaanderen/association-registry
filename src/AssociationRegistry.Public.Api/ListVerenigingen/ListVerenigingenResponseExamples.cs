namespace AssociationRegistry.Public.Api.ListVerenigingen;

using System;
using System.Collections.Immutable;
using Swashbuckle.AspNetCore.Filters;

public class ListVerenigingenResponseExamples : IExamplesProvider<ListVerenigingenResponse>
{
    private readonly AppSettings _appSettings;

    public ListVerenigingenResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public ListVerenigingenResponse GetExamples()
        => new(
            $"{_appSettings.BaseUrl}v1/contexten/list-verenigingen-context.json",
            ImmutableArray.Create(
                new ListVerenigingenResponseItem(
                    "V001",
                    "Vereniging1",
                    "V1",
                    new Locatie("2000", "Antwerpen"),
                    ImmutableArray.Create(
                        new Activiteit(
                            "Sport",
                            new Uri($"{_appSettings.BaseUrl}v1/verenigingen/V001")
                        )
                    ),
                    ImmutableArray.Create(
                        new Link(
                            $"{_appSettings.BaseUrl}v1/verenigingen/V001",
                            "GET",
                            "Detail")
                    )
                ),
                new ListVerenigingenResponseItem(
                    "V002",
                    "Vereniging2",
                    "V2",
                    new Locatie("1000", "Brussel"),
                    ImmutableArray.Create(
                        new Activiteit(
                            "Sport",
                            new Uri($"{_appSettings.BaseUrl}v1/verenigingen/V001")
                        )
                    ),
                    ImmutableArray.Create(
                        new Link(
                            $"{_appSettings.BaseUrl}v1/verenigingen/V002",
                            "GET",
                            "Detail")
                    )
                ),
                new ListVerenigingenResponseItem(
                    "V003",
                    "Vereniging3",
                    "V3",
                    new Locatie("3000", "Leuven"),
                    ImmutableArray.Create(
                        new Activiteit(
                            "Kaartclub",
                            new Uri($"{_appSettings.BaseUrl}v1/verenigingen/V001")
                        )
                    ),
                    ImmutableArray.Create(
                        new Link(
                            $"{_appSettings.BaseUrl}v1/verenigingen/V003",
                            "GET",
                            "Detail")
                    )
                )
            ),
            new Metadata(new Pagination(3, 0, 10))
        );
}
