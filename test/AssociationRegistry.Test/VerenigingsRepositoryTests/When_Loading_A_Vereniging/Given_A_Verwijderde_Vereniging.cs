namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Resources;
using Vereniging;
using Xunit;

public class Given_A_Verwijderde_Vereniging
{
    private readonly VerenigingsRepository _repo;
    private readonly VCode _vCode;

    public Given_A_Verwijderde_Vereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        _vCode = fixture.Create<VCode>();

        var eventStoreMock = new EventStoreMock(
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = _vCode,  },
            fixture.Create<VerenigingWerdVerwijderd>() with{ VCode = _vCode});

        _repo = new VerenigingsRepository(eventStoreMock);
    }

    [Fact]
    public async Task Then_A_FeitelijkeVereniging_Is_Returned()
    {
        var exception = await
            Assert.ThrowsAsync<AssociationRegistry.Vereniging.Exceptions.VerenigingIsVerwijderd>(async () => await _repo.Load<Vereniging>(_vCode, expectedVersion: null)) ;

        exception.Message.Should().Be(ExceptionMessages.VerenigingIsVerwijderd);
    }
}
