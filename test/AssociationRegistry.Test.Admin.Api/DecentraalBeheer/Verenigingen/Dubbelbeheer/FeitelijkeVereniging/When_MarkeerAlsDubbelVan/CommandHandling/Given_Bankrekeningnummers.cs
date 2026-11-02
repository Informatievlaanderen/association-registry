namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Xunit;

/// <summary>
///     Bankrekeningnummers worden enkel toegevoegd als het IBAN nog niet bestaat op de authentieke vereniging.
///     De authentieke vereniging heeft het bankrekeningnummer van Frodo (id 1). De dubbele vereniging heeft hetzelfde
///     IBAN, anders geschreven en met een ander doel en andere titularissen, en een nieuw bevestigd bankrekeningnummer.
/// </summary>
#pragma warning disable CS0618
public class Given_Bankrekeningnummers
{
    private static readonly Registratiedata.GegevensInitiator Gondor = new("OVO000202", "Gondor");

    private static readonly Registratiedata.Bankrekeningnummer BankrekeningnummerAuthentiek = new(
        BankrekeningnummerId: 1,
        Iban: "BE68539007547034",
        Doel: "Werking",
        Titularissen: ["Frodo Baggins"]
    );

    private static readonly Registratiedata.Bankrekeningnummer ZelfdeIbanBijDeDubbeleVereniging = new(
        BankrekeningnummerId: 1,
        Iban: "BE68 5390 0754 7034",
        Doel: "Lidgelden",
        Titularissen: ["Samwise Gamgee"]
    );

    private static readonly Registratiedata.Bankrekeningnummer NieuwBankrekeningnummer = new(
        BankrekeningnummerId: 2,
        Iban: "BE71096123456769",
        Doel: "Evenementen",
        Titularissen: ["Aragorn Elessar"]
    );

    private readonly Fixture _fixture = new Fixture().CustomizeDomain();
    private readonly MultipleVerenigingAggregateSessionMock _session;
    private readonly MarkeerAlsDubbelVanCommand _command;

    public Given_Bankrekeningnummers()
    {
        var dubbeleVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithHoofdactiviteiten([])
            .WithContactgegevens([])
            .WithLocaties([])
            .WithVertegenwoordigers([])
            .WithBankrekeningnummers([ZelfdeIbanBijDeDubbeleVereniging, NieuwBankrekeningnummer]);

        var authentiekeVereniging = _fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty)
            .WithBankrekeningnummers([BankrekeningnummerAuthentiek]);

        _session = new MultipleVerenigingAggregateSessionMock(
            new VerenigingState()
                .Apply(dubbeleVereniging)
                .Apply(
                    new AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
                        NieuwBankrekeningnummer.BankrekeningnummerId,
                        Gondor
                    )
                )
        );
        _session.WithVereniging(new VerenigingState().Apply(authentiekeVereniging));

        _command = new MarkeerAlsDubbelVanCommand(
            VCode.Create(dubbeleVereniging.VCode),
            VCode.Create(authentiekeVereniging.VCode)
        );
    }

    private async Task<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging> Handle()
    {
        await new MarkeerAlsDubbelVanCommandHandler(
            _session,
            new VerenigingsStateQueriesMock(),
            GeotagsServiceFactory.ReturnsUnchangedGeotags()
        ).Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>()));

        return _session
            .SavedInvocations[_command.VCodeAuthentiekeVereniging]
            .Single()
            .Vereniging.UncommittedEvents.OfType<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>()
            .Single();
    }

    [Fact]
    public async ValueTask Then_A_New_Iban_Is_Toegevoegd_With_Its_Titularissen_Doel_And_BevestigdDoor()
    {
        var samengevoegd = await Handle();

        var origineel = new SamenvoegingDubbeleVereniging.Bankrekeningnummer(
            NieuwBankrekeningnummer.BankrekeningnummerId,
            NieuwBankrekeningnummer.Iban,
            NieuwBankrekeningnummer.Doel,
            NieuwBankrekeningnummer.Titularissen,
            [Gondor]
        );

        samengevoegd
            .OvergenomenGegevens.Bankrekeningnummers.Should()
            .BeEquivalentTo([
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<SamenvoegingDubbeleVereniging.Bankrekeningnummer>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Bankrekeningnummer met id 2 werd toegevoegd vanuit de dubbele vereniging.",
                    origineel,
                    origineel with
                    {
                        BankrekeningnummerId = 2,
                    }
                ),
            ]);
    }

    [Fact]
    public async ValueTask Then_An_Existing_Iban_Is_Gededupliceerd_And_Not_Overwritten()
    {
        var samengevoegd = await Handle();

        samengevoegd
            .NietOvergenomenGegevens.Bankrekeningnummers.Should()
            .BeEquivalentTo([
                SamenvoegingDubbeleVereniging.Gededupliceerd(
                    "Bankrekeningnummer met id 1",
                    new SamenvoegingDubbeleVereniging.Bankrekeningnummer(
                        ZelfdeIbanBijDeDubbeleVereniging.BankrekeningnummerId,
                        ZelfdeIbanBijDeDubbeleVereniging.Iban,
                        ZelfdeIbanBijDeDubbeleVereniging.Doel,
                        ZelfdeIbanBijDeDubbeleVereniging.Titularissen,
                        []
                    )
                ),
            ]);
    }
}
#pragma warning restore CS0618
