namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;

/// <summary>The verenigingen of a relatieaanvraag, built from their events.</summary>
public static class Verenigingen
{
    public static readonly VCode KoepelA = VCode.Create("V0001001");
    public static readonly VCode KoepelB = VCode.Create("V0001002");
    public static readonly VCode VerenigingC = VCode.Create("V0001003");

    private static readonly Fixture Fixture = new Fixture().CustomizeAdminApi();

    public static VerenigingState Koepel(VCode vCode) =>
        KboVereniging(vCode).Apply(new VerenigingWerdGeidentificeerdAlsKoepel(vCode.Value));

    public static VerenigingState GeenKoepel(VCode vCode) =>
        KboVereniging(vCode).Apply(new VerenigingWerdGeidentificeerdAlsGeenKoepel(vCode.Value));

    public static VerenigingState KboVereniging(VCode vCode) =>
        new VerenigingState().Apply(
            Fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode.Value,
            }
        );

    public static VerenigingState Vzer(VCode vCode) =>
        new VerenigingState().Apply(
            Fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode.Value,
            }
        );

    public static VerenigingState MetLidmaatschap(
        this VerenigingState vereniging,
        VCode andereVereniging,
        DateOnly? van = null,
        DateOnly? tot = null
    ) =>
        vereniging.Apply(
            new LidmaatschapWerdToegevoegd(
                vereniging.VCode.Value,
                new Registratiedata.Lidmaatschap(
                    vereniging.Lidmaatschappen.Count + 1,
                    andereVereniging.Value,
                    "Andere vereniging",
                    van,
                    tot,
                    string.Empty,
                    string.Empty
                )
            )
        );

    public static VerenigingState AlsSubverenigingVan(this VerenigingState vereniging, VCode koepel) =>
        vereniging.Apply(
            new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                vereniging.VCode.Value,
                new Registratiedata.SubverenigingVan(koepel.Value, "Koepel", string.Empty, string.Empty)
            )
        );
}
