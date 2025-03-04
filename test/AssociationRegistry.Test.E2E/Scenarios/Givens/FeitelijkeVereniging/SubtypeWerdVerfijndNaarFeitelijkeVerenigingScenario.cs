namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Events;
using EventStore;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

public class SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario : Framework.TestClasses.IScenario
{
    public string NaamVereniging { get; set; }
    public SubtypeWerdVerfijndNaarFeitelijkeVereniging SubtypeWerdVerfijndNaarFeitelijkeVereniging { get; set; }

    public SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var vzer = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        var subtypeWerdVerfijndNaarFeitelijkeVereniging = new SubtypeWerdVerfijndNaarFeitelijkeVereniging(
            VCode: vzer.VCode,
            Subtype: new Registratiedata.Subtype(Subtype.FeitelijkeVereniging.Code,Subtype.FeitelijkeVereniging.Naam));

        SubtypeWerdVerfijndNaarFeitelijkeVereniging = new SubtypeWerdVerfijndNaarFeitelijkeVereniging(
            VCode: vzer.VCode,
            Subtype: new Registratiedata.Subtype(Subtype.NogNietBepaald.Code,Subtype.NogNietBepaald.Naam));

        return
        [
            new(vzer.VCode, [vzer, subtypeWerdVerfijndNaarFeitelijkeVereniging, SubtypeWerdVerfijndNaarFeitelijkeVereniging]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;
}
