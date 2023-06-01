namespace AssociationRegistry.Test.When_Loading_A_Vereniging;

using AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_VCode
{
    private readonly VerenigingsRepository _repo;
    private readonly VCode _vCode;

    public Given_A_VCode()
    {
        var fixture = new Fixture().CustomizeAll();
        _vCode = fixture.Create<VCode>();
        var eventStoreMock = new EventStoreMock(
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with {VCode = _vCode});
        _repo = new VerenigingsRepository(eventStoreMock);
    }

    [Fact]
    public async Task Then_A_FeitelijkeVereniging_Is_Returned()
    {
        var feteitelijkeVerenging = await _repo.Load<Vereniging>(_vCode, null);
        feteitelijkeVerenging
            .Should()
            .NotBeNull()
            .And
            .BeOfType<Vereniging>();
    }

    [Fact]
    public async Task Then_It_Throws_A_UnsupportedOperationException()
    {
        var loadMethod = ()=> _repo.Load<VerenigingMetRechtspersoonlijkheid>(_vCode, null);
        await loadMethod.Should().ThrowAsync<UnsupportedOperationForVerenigingstype>();
    }
}
