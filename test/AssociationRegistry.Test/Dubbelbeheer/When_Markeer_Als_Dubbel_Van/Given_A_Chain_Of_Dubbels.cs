namespace AssociationRegistry.Test.Dubbelbeheer.When_Markeer_Als_Dubbel_Van;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using Xunit;

/// <summary>
///     A is dubbel van B en later wordt B dubbel van C.
///     Eindtoestand: A.isDubbelVan = B, B.isDubbelVan = C, B.corresponderendeVCodes = [A], C.corresponderendeVCodes = [B].
/// </summary>
public class Given_A_Chain_Of_Dubbels
{
    private const string A = "V0003001";
    private const string B = "V0003002";
    private const string C = "V0003003";

    private readonly Fixture _fixture = new Fixture().CustomizeDomain();

    private readonly VerenigingState _a;
    private readonly VerenigingState _b;
    private readonly VerenigingState _c;

    public Given_A_Chain_Of_Dubbels()
    {
        // A -> B
        _a = MarkeerAlsDubbelVan(Geregistreerd(A), B);
        _b = Geregistreerd(B).Apply(new VerenigingAanvaarddeDubbeleVereniging(B, A));

        // B -> C, terwijl B authentiek is voor A
        _b = MarkeerAlsDubbelVan(_b, C);
        _c = Geregistreerd(C).Apply(new VerenigingAanvaarddeDubbeleVereniging(C, B));
    }

    private VerenigingState Geregistreerd(string vCode)
    {
        var geregistreerd = _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = vCode,
        };

        return new VerenigingState().Apply(geregistreerd);
    }

    private static VerenigingState MarkeerAlsDubbelVan(VerenigingState state, string authentiekeVCode)
    {
        var vereniging = new Vereniging();
        vereniging.Hydrate(state);

        vereniging.MarkeerAlsDubbelVan(VCode.Create(authentiekeVCode));

        return vereniging.UncommittedEvents.Aggregate(state, (current, @event) => current.Apply((dynamic)@event));
    }

    [Fact]
    public void Then_A_Is_Dubbel_Van_B()
    {
        _a.VerenigingStatus.Should()
            .BeOfType<VerenigingStatus.StatusDubbel>()
            .Which.VCodeAuthentiekeVereniging.Should()
            .Be(VCode.Create(B));
    }

    [Fact]
    public void Then_B_Is_Dubbel_Van_C()
    {
        _b.VerenigingStatus.Should()
            .BeOfType<VerenigingStatus.StatusDubbel>()
            .Which.VCodeAuthentiekeVereniging.Should()
            .Be(VCode.Create(C));
    }

    [Fact]
    public void Then_B_Keeps_A_As_CorresponderendeVCode()
    {
        _b.CorresponderendeVCodes.Should().Equal(A);
    }

    [Fact]
    public void Then_C_Has_B_As_CorresponderendeVCode()
    {
        _c.CorresponderendeVCodes.Should().Equal(B);
    }
}
