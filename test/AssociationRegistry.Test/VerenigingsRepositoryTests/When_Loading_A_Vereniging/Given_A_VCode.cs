namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_Loading_A_Vereniging;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Test.Framework.Customizations;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_VCode
{
    private readonly VerenigingsRepository _repo;
    private readonly VCode _vCode;

    public Given_A_VCode()
    {
        var fixture = new Fixture().CustomizeDomain();
        _vCode = fixture.Create<VCode>();

        var eventStoreMock = new EventStoreMock(
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = _vCode });

        _repo = new VerenigingsRepository(eventStoreMock);
    }

    [Fact]
    public async Task Then_A_FeitelijkeVereniging_Is_Returned()
    {
        var feteitelijkeVerenging = await _repo.Load<Vereniging>(_vCode, expectedVersion: null);

        feteitelijkeVerenging
           .Should()
           .NotBeNull()
           .And
           .BeOfType<Vereniging>();
    }
}
