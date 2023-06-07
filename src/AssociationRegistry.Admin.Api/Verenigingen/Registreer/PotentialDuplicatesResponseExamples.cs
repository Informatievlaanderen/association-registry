namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System.Collections.Immutable;
using DuplicateVerenigingDetection;
using Infrastructure.ConfigurationBindings;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;

public class PotentialDuplicatesResponseExamples : IExamplesProvider<PotentialDuplicatesResponse>
{
    private readonly AppSettings _appSettings;

    public PotentialDuplicatesResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public PotentialDuplicatesResponse GetExamples()
        => new(
            "AB-CD-12-23-43-98-36-A8",
            new PotentialDuplicatesFound(
                new[]
                {
                    new DuplicaatVereniging(
                        "V0001001",
                        new DuplicaatVereniging.VerenigingsType(
                                Verenigingstype.FeitelijkeVereniging.Code,
                                Verenigingstype.FeitelijkeVereniging.Beschrijving),
                        "Naam",
                        "Korte naam",
                        ImmutableArray.Create(
                            new DuplicaatVereniging.HoofdactiviteitVerenigingsloket("CODE", "Beschrijving")
                        ),
                        "Doelgroep",
                        ImmutableArray.Create(
                            new DuplicaatVereniging.Locatie("LocatieType", Hoofdlocatie: true, "Adres", "Naam", "Postcode", "Gemeente")
                        ),
                        ImmutableArray.Create(
                            new DuplicaatVereniging.Activiteit(Id: 1, "Categorie")
                        )),
                }),
            _appSettings);
}
