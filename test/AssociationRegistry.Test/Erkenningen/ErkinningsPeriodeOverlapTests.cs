namespace AssociationRegistry.Test.Erkenningen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Xunit;

public class ErkenningsPeriodeOverlapTests
{
    private readonly DateOnly _baseDateOnly = new(2025, 01, 10);

    /// <summary>
    /// bestaande ligt volledig vóór toeTeVoegen → geen overlap
    /// [----bestaande----]      [----toeTeVoegen----]
    /// </summary>
    [Fact]
    public void Given_Non_Overlapping_Periods_When_Bestaande_Before_ToeTeVoegen_Then_Returns_False()
    {
        var bestaande = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(5));
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly.AddDays(10), _baseDateOnly.AddDays(15));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeFalse();
    }

    /// <summary>
    /// bestaande ligt volledig na toeTeVoegen → geen overlap
    /// [----toeTeVoegen----]      [----bestaande----]
    /// </summary>
    [Fact]
    public void Given_Non_Overlapping_Periods_When_Bestaande_After_ToeTeVoegen_Then_Returns_False()
    {
        var bestaande = ErkenningsPeriode.Create(_baseDateOnly.AddDays(10), _baseDateOnly.AddDays(15));
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(5));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeFalse();
    }

    /// <summary>
    /// toeTeVoegen overlapt begin van bestaande
    ///     [----bestaande----]
    /// [----toeTeVoegen----]
    /// </summary>
    [Fact]
    public void Given_Overlap_At_Start_Then_Returns_True()
    {
        var bestaande = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly.AddDays(-5), _baseDateOnly.AddDays(5));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeTrue();
    }

    /// <summary>
    /// toeTeVoegen overlapt einde van bestaande
    /// [----bestaande----]
    ///         [----toeTeVoegen----]
    /// </summary>
    [Fact]
    public void Given_Overlap_At_End_Then_Returns_True()
    {
        var bestaande = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly.AddDays(5), _baseDateOnly.AddDays(15));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeTrue();
    }

    /// <summary>
    /// toeTeVoegen ligt volledig binnen bestaande
    /// [------bestaande------]
    ///    [--toeTeVoegen--]
    /// </summary>
    [Fact]
    public void Given_ToeTeVoegen_Inside_Bestaande_Then_Returns_True()
    {
        var bestaande = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(20));
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly.AddDays(5), _baseDateOnly.AddDays(10));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeTrue();
    }

    /// <summary>
    /// bestaande ligt volledig binnen toeTeVoegen
    ///    [--bestaande--]
    /// [------toeTeVoegen------]
    /// </summary>
    [Fact]
    public void Given_Bestaande_Inside_ToeTeVoegen_Then_Returns_True()
    {
        var bestaande = ErkenningsPeriode.Create(_baseDateOnly.AddDays(5), _baseDateOnly.AddDays(10));
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(20));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeTrue();
    }

    /// <summary>
    /// grenzen raken elkaar exact → overlap (inclusief)
    /// [----bestaande----]
    ///                   [----toeTeVoegen----]
    /// </summary>
    [Fact]
    public void Given_Touching_At_Boundary_Then_Returns_True()
    {
        var bestaande = ErkenningsPeriode.Create(_baseDateOnly, _baseDateOnly.AddDays(10));
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly.AddDays(10), _baseDateOnly.AddDays(20));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeTrue();
    }

    /// <summary>
    /// bestaande heeft geen startdatum (−∞) → overlap met toeTeVoegen in het verleden
    /// </summary>
    [Fact]
    public void Given_Bestaande_Start_Is_Null_Then_Treated_As_MinValue()
    {
        var bestaande = ErkenningsPeriode.Create(null, _baseDateOnly.AddDays(10));
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly.AddDays(-100), _baseDateOnly.AddDays(-50));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeTrue();
    }

    /// <summary>
    /// bestaande heeft geen einddatum (+∞) → overlap met toeTeVoegen in de toekomst
    /// </summary>
    [Fact]
    public void Given_Bestaande_End_Is_Null_Then_Treated_As_MaxValue()
    {
        var bestaande = ErkenningsPeriode.Create(_baseDateOnly, null);
        var toeTeVoegen = ErkenningsPeriode.Create(_baseDateOnly.AddDays(100), _baseDateOnly.AddDays(200));

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeTrue();
    }

    /// <summary>
    /// beide periodes open (−∞, +∞) → altijd overlap
    /// </summary>
    [Fact]
    public void Given_Both_Open_Intervals_Then_Always_Overlap()
    {
        var bestaande = ErkenningsPeriode.Create(null, null);
        var toeTeVoegen = ErkenningsPeriode.Create(null, null);

        var result = bestaande.OverlapsWith(toeTeVoegen);

        result.Should().BeTrue();
    }
}
