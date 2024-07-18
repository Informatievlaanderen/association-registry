namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using DuplicateVerenigingDetection;
using Hosts.Configuration.ConfigurationBindings;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Immutable;
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
            hashedRequest: "AB-CD-12-23-43-98-36-A8",
            new PotentialDuplicatesFound(
                new[]
                {
                    new DuplicaatVereniging(
                        VCode: "V0001001",
                        new DuplicaatVereniging.VerenigingsType(
                            Verenigingstype.FeitelijkeVereniging.Code,
                            Verenigingstype.FeitelijkeVereniging.Naam),
                        Naam: "Naam",
                        KorteNaam: "Korte naam",
                        ImmutableArray.Create(
                            new DuplicaatVereniging.HoofdactiviteitVerenigingsloket(Code: "CODE", Naam: "Beschrijving")
                        ),
                        ImmutableArray.Create(
                            new DuplicaatVereniging.Locatie(Locatietype: "Locatietype", IsPrimair: true, Adres: "Adresvoorstelling",
                                                            Naam: "Naam", Postcode: "Postcode", Gemeente: "Gemeente")
                        )),
                }),
            _appSettings);
}
