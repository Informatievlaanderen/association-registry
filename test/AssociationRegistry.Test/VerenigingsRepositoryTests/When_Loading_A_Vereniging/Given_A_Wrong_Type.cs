namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AssociationRegistry.EventStore;
using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_A_Wrong_Type
{
    private readonly VerenigingsRepository _repo;
    private readonly VCode _vCode;

    public Given_A_Wrong_Type()
    {
        var fixture = new Fixture().CustomizeDomain();
        _vCode = fixture.Create<VCode>();

        var eventStoreMock = new EventStoreMock(
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = _vCode });

        _repo = new VerenigingsRepository(eventStoreMock);
    }


    [Fact]
    public async Task Then_It_Throws_A_UnsupportedOperationException()
    {
        var loadMethod = () => _repo.Load<VerenigingMetRechtspersoonlijkheid>(_vCode, expectedVersion: null);
        await loadMethod.Should().ThrowAsync<ActieIsNietToegestaanVoorVerenigingstype>();
    }
}
