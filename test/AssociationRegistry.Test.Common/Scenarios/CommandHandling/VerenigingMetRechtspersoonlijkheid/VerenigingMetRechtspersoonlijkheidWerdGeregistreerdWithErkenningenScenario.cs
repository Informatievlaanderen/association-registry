namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerdInVerleden;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerdInHuidig;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerdInToekomst;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerdTeActiveren;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        var today = DateOnly.FromDateTime(DateTime.Now);
        var einddatum = today.AddDays(-fixture.Create<int>());
        var hernieuwingsdatum = einddatum.AddDays(-fixture.Create<int>());
        var startdatum = hernieuwingsdatum.AddDays(-fixture.Create<int>());

        ErkenningWerdGeregistreerdInVerleden = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = startdatum,
            Hernieuwingsdatum = hernieuwingsdatum,
            Einddatum = einddatum,
            Status = ErkenningStatus.VerlopenValue,
        };

        var huidigeErkenningStartDatum = ErkenningWerdGeregistreerdInVerleden.Einddatum.Value;
        var hernieuwingsDatumVoorHuidigeErkenning = today.AddDays(fixture.Create<int>());
        var eindDatumVoorHuidigeErkenning = hernieuwingsDatumVoorHuidigeErkenning.AddDays(fixture.Create<int>());

        ErkenningWerdGeregistreerdInHuidig = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = huidigeErkenningStartDatum,
            Einddatum = eindDatumVoorHuidigeErkenning,
            Hernieuwingsdatum = hernieuwingsDatumVoorHuidigeErkenning,
            IpdcProduct = ErkenningWerdGeregistreerdInVerleden.IpdcProduct,
            GeregistreerdDoor = ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor,
            Status = ErkenningStatus.ActiefValue,
        };

        var teActiverenErkenningStartDatum = ErkenningWerdGeregistreerdInVerleden.Einddatum.Value;
        var hernieuwingsDatumVoorteActiveren = today.AddDays(fixture.Create<int>());
        var eindDatumVoorteActiveren = hernieuwingsDatumVoorteActiveren.AddDays(fixture.Create<int>());

        ErkenningWerdGeregistreerdTeActiveren = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = teActiverenErkenningStartDatum,
            Einddatum = eindDatumVoorteActiveren,
            Hernieuwingsdatum = hernieuwingsDatumVoorteActiveren,
            IpdcProduct = ErkenningWerdGeregistreerdInVerleden.IpdcProduct,
            GeregistreerdDoor = ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor,
            Status = ErkenningStatus.InAanvraagValue,
        };

        var toekomstErkenningStartDatum = ErkenningWerdGeregistreerdInHuidig.Einddatum.Value;
        var hernieuwingsDatumVoorToekomstErkenning = toekomstErkenningStartDatum.AddDays(fixture.Create<int>());
        var eindDatumVoorToekomstErkenning = hernieuwingsDatumVoorToekomstErkenning.AddDays(fixture.Create<int>());

        ErkenningWerdGeregistreerdInToekomst = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = toekomstErkenningStartDatum,
            Einddatum = eindDatumVoorToekomstErkenning,
            Hernieuwingsdatum = hernieuwingsDatumVoorToekomstErkenning,
            IpdcProduct = ErkenningWerdGeregistreerdInVerleden.IpdcProduct,
            GeregistreerdDoor = ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor,
            Status = ErkenningStatus.InAanvraagValue,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerdInVerleden,
            ErkenningWerdGeregistreerdInHuidig,
            ErkenningWerdGeregistreerdInToekomst,
            ErkenningWerdGeregistreerdTeActiveren,
        };
}
