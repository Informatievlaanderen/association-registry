namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer;

using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Immutable;

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
                        [
                            new DuplicaatVereniging.HoofdactiviteitVerenigingsloket(Code: "CODE", Naam: "Beschrijving")
                        ],
                        [
                            new DuplicaatVereniging.Locatie(Locatietype: "Locatietype", IsPrimair: true, Adres: "Adresvoorstelling",
                                                            Naam: "Naam", Postcode: "Postcode", Gemeente: "Gemeente")
                        ]),
                }),
            _appSettings);
}
