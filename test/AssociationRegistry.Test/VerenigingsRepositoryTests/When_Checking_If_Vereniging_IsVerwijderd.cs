namespace AssociationRegistry.Test.VerenigingsRepositoryTests;

using AssociationRegistry.EventStore;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using MartenDb.Store;
using Moq;
using Vereniging;
using Xunit;

public class When_Checking_If_Vereniging_IsVerwijderd
{
    private readonly Fixture _fixture;

    public When_Checking_If_Vereniging_IsVerwijderd()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public async ValueTask Given_A_Vereniging_IsVerwijderd()
    {
        await using var store = await TestDocumentStoreFactory.CreateAsync("VerenigingIsVerwijderd");
        await using var session = store.LightweightSession();
        var vCode = _fixture.Create<VCode>();

        session.Events.Append(
            vCode.ToString(),
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode,
            },
            _fixture.Create<VerenigingWerdVerwijderd>() with
            {
                VCode = vCode,
            }
        );

        await session.SaveChangesAsync();

        var sut = new VerenigingStateQueryService(session);
        var actual = await sut.IsVerwijderd(VCode.Create(vCode));

        actual.Should().BeTrue();
    }

    [Fact]
    public async ValueTask Given_A_Vereniging_IsNietVerwijderd()
    {
        await using var store = await TestDocumentStoreFactory.CreateAsync("VerenigingIsNietVerwijderd");
        await using var session = store.LightweightSession();
        var vCode = _fixture.Create<VCode>();

        session.Events.Append(
            vCode.ToString(),
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode,
            }
        );

        await session.SaveChangesAsync();

        var sut = new VerenigingStateQueryService(session);
        var actual = await sut.IsVerwijderd(VCode.Create(vCode));

        actual.Should().BeFalse();
    }
}
