namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Swashbuckle.AspNetCore.Filters;


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
            potentialDuplicates: new PotentialDuplicatesFound(
                new[]
                {
                    new DuplicaatVereniging(
                        VCode: "V0001001",
                        new DuplicaatVereniging.VerenigingsType()
                        {
                            Code = Verenigingstype.FeitelijkeVereniging.Code,
                            Naam = Verenigingstype.FeitelijkeVereniging.Naam
                        },
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
            appSettings: _appSettings,
            new VerenigingstypeMapperV1());
}
