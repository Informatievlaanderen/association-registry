namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer;

using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.DubbelDetectie;
using DecentraalBeheer.Vereniging.Mappers;
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
            potentialDuplicates: new PotentialDuplicatesFound(false, string.Empty, new DuplicaatVereniging(
                    VCode: "V0001001",
                    new DuplicaatVereniging.Types.Verenigingstype()
                    {
                        Code = DecentraalBeheer.Vereniging.Verenigingstype.VZER.Code,
                        Naam = DecentraalBeheer.Vereniging.Verenigingstype.VZER.Naam
                    },
                    Verenigingssubtype: VerenigingssubtypeCode.NietBepaald.Map<DuplicaatVereniging.Types.Verenigingssubtype>(),
                    Naam: "Naam",
                    KorteNaam: "Korte naam",
                    [
                        new DuplicaatVereniging.Types.HoofdactiviteitVerenigingsloket(Code: "CODE", Naam: "Beschrijving")
                    ],
                    [
                        new DuplicaatVereniging.Types.Locatie(Locatietype: "Locatietype", IsPrimair: true, Adres: "Adresvoorstelling",
                            Naam: "Naam", Postcode: "Postcode", Gemeente: "Gemeente")
                    ])),
            appSettings: _appSettings,
            new VerenigingstypeMapperV1());
}
