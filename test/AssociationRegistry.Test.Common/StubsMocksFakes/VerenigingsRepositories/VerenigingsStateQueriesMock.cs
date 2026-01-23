namespace AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;

using AssociationRegistry.Framework;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using DecentraalBeheer.Vereniging;
using FluentAssertions;

public class VerenigingsStateQueriesMock : IVerenigingStateQueryService
{
    private VerenigingState? _verenigingToLoad;
    private readonly bool _expectedLoadingDubbel;
    private bool _actualLoadingDubbel;
    private bool _expectedLoadingVerwijderd;
    private bool _actualLoadingVerwijderd;

    // ReSharper disable once NotAccessedPositionalProperty.Local
    // Anders kan er niet gecompared worden.
    private record InvocationLoad(string key, Type Type);

    private readonly List<InvocationLoad> _invocationsLoad = new();

    public VerenigingsStateQueriesMock(
        VerenigingState? verenigingToLoad = null,
        bool expectedLoadingDubbel = false,
        bool expectedLoadingVerwijderd = false
    )
    {
        _verenigingToLoad = verenigingToLoad;
        _expectedLoadingDubbel = expectedLoadingDubbel;
        _expectedLoadingVerwijderd = expectedLoadingVerwijderd;
    }

    public async Task<TVereniging> Load<TVereniging>(
        VCode vCode,
        CommandMetadata metadata,
        bool allowVerwijderdeVereniging = false,
        bool allowDubbeleVereniging = false
    )
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        _actualLoadingDubbel = allowDubbeleVereniging;
        _actualLoadingVerwijderd = allowVerwijderdeVereniging;
        _invocationsLoad.Add(new InvocationLoad(key: vCode, typeof(TVereniging)));
        var vereniging = new TVereniging();
        vereniging.Hydrate(_verenigingToLoad!);

        return await Task.FromResult(vereniging);
    }

    public async Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata)
    {
        _invocationsLoad.Add(new InvocationLoad(key: kboNummer, typeof(VerenigingMetRechtspersoonlijkheid)));

        if (_verenigingToLoad is null)
            throw new AggregateNotFoundException(identifier: kboNummer, typeof(VerenigingMetRechtspersoonlijkheid));

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        vereniging.Hydrate(_verenigingToLoad);

        return await Task.FromResult(vereniging);
    }

    public Task<bool> IsVerwijderd(VCode vCode) => Task.FromResult(false);

    public Task<bool> IsDubbel(VCode vCode) => Task.FromResult(false);

    public async Task<bool> Exists(VCode vCode) => true;

    public async Task<bool> Exists(KboNummer kboNummer) => true;

    public async Task<VCode?> GetOptionalVCodeFor(KboNummer kboNummer) =>
        _verenigingToLoad?.KboNummer == kboNummer ? _verenigingToLoad.VCode : null;

    public async Task<IReadOnlyList<VCode>> FilterVzerOnly(IEnumerable<VCode> vCodes) => vCodes.ToList();

    public void ShouldHaveLoaded<TVereniging>(params string[] keys)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        _invocationsLoad
            .Should()
            .BeEquivalentTo(
                keys.Select(key => new InvocationLoad(key: key, typeof(TVereniging))),
                config: options => options.WithStrictOrdering()
            );
    }

    // public void ShouldHaveSavedAtLeast(params IEvent[] expectedEvents)
    // {
    //     SaveInvocations.Should().HaveCount(1);
    //
    //     var actual = SaveInvocations[0].Vereniging.UncommittedEvents.ToArray();
    //
    //     actual.Should().Contain(expectedEvents);
    //
    //     AssertLoadingDubbel();
    // }

    public void AssertLoadingDubbel()
    {
        _actualLoadingDubbel.Should().Be(_expectedLoadingDubbel);
    }

    public void AssertLoadingVerwijderd()
    {
        _actualLoadingVerwijderd.Should().Be(_expectedLoadingVerwijderd);
    }
}
