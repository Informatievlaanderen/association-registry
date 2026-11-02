namespace AssociationRegistry.Test.Dubbelbeheer.When_Applying_DubbeleVerenigingWerdSamengevoegd;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AutoFixture;
using FluentAssertions;
using Xunit;
using static AssociationRegistry.Events.SamenvoegingDubbeleVereniging;
using static AssociationRegistry.Test.Common.DubbelMerge.TestHoofdactiviteiten;
using static AssociationRegistry.Test.Common.DubbelMerge.TestWerkingsgebieden;

/// <summary>De overgenomen hoofdactiviteiten en werkingsgebieden komen bij de bestaande op de authentieke vereniging.</summary>
#pragma warning disable CS0618
public class Given_Hoofdactiviteiten_En_Werkingsgebieden
{
    private readonly Fixture _fixture = new Fixture().CustomizeDomain();

    [Fact]
    public void Then_The_Overgenomen_Hoofdactiviteiten_Are_Added()
    {
        var state = Apply(
            authentiekeHoofdactiviteiten: [Sport],
            authentiekeWerkingsgebieden: [],
            Overgenomen() with
            {
                Hoofdactiviteiten = [Toegevoegd("Hoofdactiviteit 'Jeugdwerk'", Jeugdwerk)],
            }
        );

        state.HoofdactiviteitenVerenigingsloket.Select(x => x.Code).Should().Equal(Sport.Code, Jeugdwerk.Code);
    }

    [Fact]
    public void Then_The_Overgenomen_Werkingsgebieden_Are_Added()
    {
        var state = Apply(
            authentiekeHoofdactiviteiten: [],
            authentiekeWerkingsgebieden: [Antwerpen],
            Overgenomen() with
            {
                Werkingsgebieden = [Toegevoegd("Werkingsgebied 'Provincie Vlaams-Brabant'", VlaamsBrabant)],
            }
        );

        state.Werkingsgebieden.Select(x => x.Code).Should().Equal(Antwerpen.Code, VlaamsBrabant.Code);
    }

    [Fact]
    public void Then_An_Overgenomen_NietVanToepassing_Makes_The_Werkingsgebieden_NietVanToepassing()
    {
        var state = Apply(
            authentiekeHoofdactiviteiten: [],
            authentiekeWerkingsgebieden: [],
            Overgenomen() with
            {
                Werkingsgebieden = [Toegevoegd("Werkingsgebied 'Niet van toepassing'", NietVanToepassing)],
            }
        );

        state.Werkingsgebieden.IsNietVanToepassing.Should().BeTrue();
    }

    private VerenigingState Apply(
        Registratiedata.HoofdactiviteitVerenigingsloket[] authentiekeHoofdactiviteiten,
        Registratiedata.Werkingsgebied[] authentiekeWerkingsgebieden,
        OvergenomenGegevensPerGroep<Registratiedata.Vertegenwoordiger, Registratiedata.Bankrekeningnummer> overgenomen
    )
    {
        var registratie = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithHoofdactiviteiten(authentiekeHoofdactiviteiten);

        var state = new VerenigingState().Apply(registratie);

        if (authentiekeWerkingsgebieden.Length > 0)
            state = state.Apply(new WerkingsgebiedenWerdenBepaald(registratie.VCode, authentiekeWerkingsgebieden));

        return state.Apply(
            EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
                VCode.Create(registratie.VCode),
                _fixture.Create<VCode>(),
                overgenomen,
                NietOvergenomenGegevensPerGroep<
                    Registratiedata.Vertegenwoordiger,
                    Registratiedata.Bankrekeningnummer
                >.WithOnly(NietOvergenomenBasisgegevens.Geen)
            )
        );
    }

    private static OvergenomenGegevensPerGroep<
        Registratiedata.Vertegenwoordiger,
        Registratiedata.Bankrekeningnummer
    > Overgenomen() =>
        OvergenomenGegevensPerGroep<Registratiedata.Vertegenwoordiger, Registratiedata.Bankrekeningnummer>.WithOnly(
            OvergenomenBasisgegevens.Geen
        );
}
#pragma warning restore CS0618
