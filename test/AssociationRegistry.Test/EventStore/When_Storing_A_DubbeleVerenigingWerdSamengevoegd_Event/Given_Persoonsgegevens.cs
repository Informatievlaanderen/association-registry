namespace AssociationRegistry.Test.When_Storing_A_DubbeleVerenigingWerdSamengevoegd_Event;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.DubbelMerge;
using FluentAssertions;
using Xunit;
using static AssociationRegistry.Test.Common.DubbelMerge.TestBankrekeningnummers;
using static AssociationRegistry.Test.Common.DubbelMerge.TestVertegenwoordigers;

#pragma warning disable CS0618
public class Given_Persoonsgegevens
{
    private const string AuthentiekeVCode = "V0006002";
    private const string DubbeleVCode = "V0006001";

    // OrigineleWaarde en NieuweWaarde krijgen elk een eigen RefId. Ze hebben hier bewust andere persoonsgegevens
    // (Frodo/Samwise), zodat de test faalt als de upcaster de verkeerde RefId gebruikt.
    // Van de niet-overgenomen Aragorn en Bilbo wordt enkel het id bewaard, zonder persoonsgegevens.
    private static DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging MergedEvent() =>
        new DubbelMergeEventBuilder(AuthentiekeVCode, DubbeleVCode)
            .WithDataInAllOtherGroups()
            .WithOvergenomenVertegenwoordiger(Frodo, Samwise)
            .WithNietOvergenomenVertegenwoordiger(Aragorn.VertegenwoordigerId)
            .WithOvergenomenBankrekeningnummer(BankrekeningnummerFrodo, BankrekeningnummerSamwise)
            .WithNietOvergenomenBankrekeningnummer(BankrekeningnummerBilbo.BankrekeningnummerId)
            .Build();

    [Fact]
    public async ValueTask Then_The_Persisted_Event_Contains_No_Persoonsgegevens()
    {
        var store = await InsertMergedEventInStore();

        var json = await store.GetPersistedJson(AuthentiekeVCode);

        string[] allPersoonsgegevens =
        [
            .. Frodo.PersoonsgegevensValues(),
            .. Samwise.PersoonsgegevensValues(),
            .. Aragorn.PersoonsgegevensValues(),
            .. BankrekeningnummerFrodo.PersoonsgegevensValues(),
            .. BankrekeningnummerSamwise.PersoonsgegevensValues(),
            .. BankrekeningnummerBilbo.PersoonsgegevensValues(),
        ];

        json.Should().NotContainAny(allPersoonsgegevens);
    }

    [Fact]
    public async ValueTask Then_Only_The_Persoonsgegevens_Of_The_Overgenomen_Vertegenwoordigers_Are_Stored_Under_The_Authentieke_Vereniging()
    {
        var store = await InsertMergedEventInStore();

        var documents = await store.GetVertegenwoordigerPersoonsgegevens();

        documents
            .Should()
            .BeEquivalentTo(
                new[] { Frodo.ToDocument(AuthentiekeVCode), Samwise.ToDocument(AuthentiekeVCode) },
                options => options.Excluding(x => x.RefId)
            );
        documents.Select(x => x.RefId).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async ValueTask Then_Only_The_Persoonsgegevens_Of_The_Overgenomen_Bankrekeningnummers_Are_Stored_Under_The_Authentieke_Vereniging()
    {
        var store = await InsertMergedEventInStore();

        var documents = await store.GetBankrekeningnummerPersoonsgegevens();

        documents
            .Should()
            .BeEquivalentTo(
                new[]
                {
                    BankrekeningnummerFrodo.ToDocument(AuthentiekeVCode),
                    BankrekeningnummerSamwise.ToDocument(AuthentiekeVCode),
                },
                options => options.Excluding(x => x.RefId)
            );
        documents.Select(x => x.RefId).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async ValueTask Then_It_Is_Upcasted_With_The_Persoonsgegevens()
    {
        var store = await InsertMergedEventInStore();

        var @event = await store.GetUpcastedEvent<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>(
            AuthentiekeVCode
        );

        @event.Should().BeEquivalentTo(MergedEvent(), options => options.WithStrictOrdering());
    }

    [Fact]
    public async ValueTask Then_It_Is_Upcasted_As_Anonymized_When_The_Persoonsgegevens_Were_Deleted()
    {
        var store = await InsertMergedEventInStore();
        await store.DeletePersoonsgegevens(AuthentiekeVCode);

        var @event = await store.GetUpcastedEvent<DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging>(
            AuthentiekeVCode
        );

        var expected = new DubbelMergeEventBuilder(AuthentiekeVCode, DubbeleVCode)
            .WithDataInAllOtherGroups()
            .WithOvergenomenVertegenwoordiger(Frodo.Anonymized(), Samwise.Anonymized())
            .WithNietOvergenomenVertegenwoordiger(Aragorn.VertegenwoordigerId)
            .WithOvergenomenBankrekeningnummer(
                BankrekeningnummerFrodo.Anonymized(),
                BankrekeningnummerSamwise.Anonymized()
            )
            .WithNietOvergenomenBankrekeningnummer(BankrekeningnummerBilbo.BankrekeningnummerId)
            .Build();

        @event.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    private static async Task<PersoonsgegevensEventStore> InsertMergedEventInStore()
    {
        var store = await PersoonsgegevensEventStore.Create(nameof(Given_Persoonsgegevens));
        await store.Save(AuthentiekeVCode, MergedEvent());

        return store;
    }
}
#pragma warning restore CS0618
