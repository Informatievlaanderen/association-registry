namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AutoFixture;
using Events;

public static class WijzigErkenningTestHelperExtensions
{
    public static ErkenningWerdGewijzigd MapToExpectedEvent(this WijzigErkenningCommand source) =>
        new(
            ErkenningId: source.Erkenning.ErkenningId,
            Startdatum: source.Erkenning.StartDatum.Value,
            Einddatum: source.Erkenning.EindDatum.Value,
            Hernieuwingsdatum: source.Erkenning.Hernieuwingsdatum.Value,
            HernieuwingsUrl: source.Erkenning.HernieuwingsUrl,
            Status: ErkenningStatus
                .Bepaal(
                    ErkenningsPeriode.Create(
                        startdatum: source.Erkenning.StartDatum.Value,
                        einddatum: source.Erkenning.EindDatum.Value
                    ),
                    DateOnly.FromDateTime(DateTime.Now)
                )
                .Value,
            RedenVanWijziging: source.Erkenning.RedenVanWijziging
        );

    public static WijzigErkenningCommand CreateRandomizedForSameVCodeAndErkenningId(
        this WijzigErkenningCommand source,
        IFixture fixture
    ) =>
        fixture.Create<WijzigErkenningCommand>() with
        {
            Erkenning = fixture.Create<TeWijzigenErkenning>() with { ErkenningId = source.Erkenning.ErkenningId },
            VCode = source.VCode,
        };
}
