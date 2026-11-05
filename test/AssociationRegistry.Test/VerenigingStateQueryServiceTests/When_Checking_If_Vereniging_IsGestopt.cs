namespace AssociationRegistry.Test.VerenigingStateQueryServiceTests;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using MartenDb.Store;
using Xunit;

/// <summary>
///     Een lidmaatschap bij een gestopte vereniging wordt bij de samenvoeging van een dubbele vereniging niet
///     overgenomen. Een vereniging met rechtspersoonlijkheid die gestopt werd in de KBO, telt ook als gestopt.
/// </summary>
public class When_Checking_If_Vereniging_IsGestopt
{
    private readonly Fixture _fixture = new Fixture().CustomizeDomain();

    [Fact]
    public async ValueTask Given_A_Vereniging_WerdGestopt()
    {
        var vCode = _fixture.Create<VCode>();

        var actual = await IsGestopt(
            nameof(Given_A_Vereniging_WerdGestopt),
            vCode,
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode,
            },
            _fixture.Create<VerenigingWerdGestopt>()
        );

        actual.Should().BeTrue();
    }

    [Fact]
    public async ValueTask Given_A_VerenigingMetRechtspersoonlijkheid_WerdGestoptInKbo()
    {
        var vCode = _fixture.Create<VCode>();

        var actual = await IsGestopt(
            nameof(Given_A_VerenigingMetRechtspersoonlijkheid_WerdGestoptInKbo),
            vCode,
            _fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode,
            },
            _fixture.Create<VerenigingWerdGestoptInKBO>()
        );

        actual.Should().BeTrue();
    }

    [Fact]
    public async ValueTask Given_A_Vereniging_IsNietGestopt()
    {
        var vCode = _fixture.Create<VCode>();

        var actual = await IsGestopt(
            nameof(Given_A_Vereniging_IsNietGestopt),
            vCode,
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode,
            }
        );

        actual.Should().BeFalse();
    }

    private static async Task<bool> IsGestopt(string schema, VCode vCode, params object[] events)
    {
        await using var store = await TestDocumentStoreFactory.CreateAsync(schema);
        await using var session = store.LightweightSession();

        session.Events.Append(vCode.ToString(), events);
        await session.SaveChangesAsync();

        return await new VerenigingStateQueryService(session).IsGestopt(vCode);
    }
}
