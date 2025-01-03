namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AssociationRegistry.EventStore;
using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Resources;
using Vereniging;
using Xunit;

public class Given_A_Dubbele_Vereniging
{
    private readonly VerenigingsRepository _repo;
    private readonly VCode _vCode;

    public Given_A_Dubbele_Vereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        _vCode = fixture.Create<VCode>();

        var eventStoreMock = new EventStoreMock(
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = _vCode,  },
            fixture.Create<VerenigingWerdGemarkeerdAlsDubbelVan>() with{ VCode = _vCode, VCodeAuthentiekeVereniging = fixture.Create<VCode>()});

        _repo = new VerenigingsRepository(eventStoreMock);
    }

    [Fact]
    public async Task Then_Throws_A_VerenigingIsDubbelException()
    {
        var exception = await
            Assert.ThrowsAsync<AssociationRegistry.Vereniging.Exceptions.VerenigingIsDubbel>(async () => await _repo.Load<Vereniging>(_vCode, expectedVersion: null)) ;

        exception.Message.Should().Be(ExceptionMessages.VerenigingIsDubbel);
    }
}
