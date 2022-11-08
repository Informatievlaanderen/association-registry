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
            $"{_appSettings.BaseUrl}v1/contexten/detail-vereniging-context.json",
            new VerenigingDetail(
                "V123",
                "Voorbeeld Vereniging",
                "VV",
                "De voorbeeld vereniging",
                "Vereniging",
                DateOnly.FromDateTime(new DateTime(2022, 09, 27)),
                null,
                new Locatie("2000", "Antwerpen"),
                new ContactPersoon(
                    "Joske",
                    "Vermeulen",
                    ImmutableArray.Create(
                        new ContactGegeven(
                            "email",
                            "joske@gmail.com")
                    )
                ),
                ImmutableArray.Create(
                    new Locatie("2000", "Antwerpen")
                ),
                ImmutableArray.Create(
                    new Activiteit(
                        "Sport",
                        new Uri($"{_appSettings.BaseUrl}v1/verenigingen/V12")
                    )
                ),
                ImmutableArray.Create(
                    new ContactGegeven("email", "info@vv.be")
                ),
                DateOnly.FromDateTime(
                    new DateTime(2022, 09, 27)
                )
            )
        );
}
