namespace AssociationRegistry.Test.Dubbelbeheer.When_Applying_DubbeleVerenigingWerdSamengevoegd;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AssociationRegistry.Vereniging.Bronnen;
using AutoFixture;
using FluentAssertions;
using Xunit;
using static AssociationRegistry.Events.SamenvoegingDubbeleVereniging;
using static AssociationRegistry.Test.Common.DubbelMerge.TestContactgegevens;

/// <summary>De overgenomen contactgegevens komen bij de bestaande, met hun nieuwe id en eventueel aangepast primair.</summary>
#pragma warning disable CS0618
public class Given_Contactgegevens
{
    private readonly VerenigingState _state;

    public Given_Contactgegevens()
    {
        var fixture = new Fixture().CustomizeDomain();

        var registratie = fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithContactgegevens([EmailRivendell]);

        var samengevoegd = EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            VCode.Create(registratie.VCode),
            fixture.Create<VCode>(),
            OvergenomenGegevensPerGroep<
                Registratiedata.Vertegenwoordiger,
                SamenvoegingDubbeleVereniging.Bankrekeningnummer
            >.WithOnly(OvergenomenBasisgegevens.Geen) with
            {
                Contactgegevens =
                [
                    ToegevoegdMetAanpassing(
                        "aangepast",
                        EmailFellowship,
                        EmailFellowship with
                        {
                            ContactgegevenId = 2,
                            IsPrimair = false,
                        }
                    ),
                ],
            },
            NietOvergenomenGegevensPerGroep<
                Registratiedata.Vertegenwoordiger,
                SamenvoegingDubbeleVereniging.Bankrekeningnummer
            >.WithOnly(NietOvergenomenBasisgegevens.Geen)
        );

        _state = new VerenigingState().Apply(registratie).Apply(samengevoegd);
    }

    [Fact]
    public void Then_The_Existing_Contactgegeven_Is_Kept() =>
        _state
            .Contactgegevens.Should()
            .ContainSingle(x => x.ContactgegevenId == EmailRivendell.ContactgegevenId && x.IsPrimair);

    [Fact]
    public void Then_The_Nieuwe_Waarde_Is_Added() =>
        _state
            .Contactgegevens.Should()
            .ContainEquivalentOf(
                Contactgegeven.Hydrate(
                    2,
                    Contactgegeventype.Email,
                    EmailFellowship.Waarde,
                    EmailFellowship.Beschrijving,
                    isPrimair: false,
                    Bron.Initiator
                )
            );
}
#pragma warning restore CS0618
