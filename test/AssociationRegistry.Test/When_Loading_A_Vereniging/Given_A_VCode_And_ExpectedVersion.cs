namespace AssociationRegistry.Test.When_Loading_A_Vereniging;

using AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_VCode_And_ExpectedVersion
{
    private readonly VerenigingsRepository _repo;
    private readonly VCode _vCode;

    public Given_A_VCode_And_ExpectedVersion()
    {
        var fixture = new Fixture().CustomizeDomain();
        _vCode = fixture.Create<VCode>();

        var eventStoreMock = new EventStoreMock(
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = _vCode });

        var lockStoreMock = new LockStoreMock();
        _repo = new VerenigingsRepository(eventStoreMock, lockStoreMock);
    }

    [Fact]
    public async Task Then_A_FeitelijkeVereniging_Is_Returned()
    {
        var feteitelijkeVerenging = await _repo.Load<Vereniging>(_vCode, 1);

        feteitelijkeVerenging
           .Should()
           .NotBeNull()
           .And
           .BeOfType<Vereniging>();
    }

    [Fact]
    public async Task Then_It_Throws_A_UnexpectedAggregateVersionException()
    {
        var loadMethod = () => _repo.Load<Vereniging>(_vCode, 2);
        await loadMethod.Should().ThrowAsync<UnexpectedAggregateVersionException>();
    }
}
