namespace AssociationRegistry.Test.Bankrekeningnummers.When_Creating_Titularissen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using FluentAssertions;
using Resources;
using Xunit;

public class TitularissenTests
{
    // ---------------------------------------------------------------
    // Create - invalid input
    // ---------------------------------------------------------------

    [Fact]
    public void Create_With_Null_Throws()
    {
        Assert.Throws<TitularisMagNietNullOfLeegZijn>(() => Titularissen.Create(null!));
    }

    [Fact]
    public void Create_With_Empty_Array_Throws()
    {
        Assert.Throws<TitularisMagNietNullOfLeegZijn>(() => Titularissen.Create([]));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Create_With_NullOrWhiteSpace_Element_Throws(string? element)
    {
        Assert.Throws<TitularisMagNietNullOfLeegZijn>(() => Titularissen.Create([element!]));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_With_NullOrWhiteSpace_Second_Element_Throws(string? element)
    {
        Assert.Throws<TitularisMagNietNullOfLeegZijn>(() => Titularissen.Create(["Frodo Baggins", element!]));
    }

    // ---------------------------------------------------------------
    // Create - valid input
    // ---------------------------------------------------------------

    [Fact]
    public void Create_With_Single_Titularissen_Returns_Value()
    {
        var titularissen = Titularissen.Create(["Frodo Baggins"]);

        Assert.Equal(["Frodo Baggins"], titularissen.Value);
    }

    [Fact]
    public void Create_With_Multiple_Titularissensen_Preserves_Order()
    {
        var titularissen = Titularissen.Create(["Frodo Baggins", "Samwise Gamgee"]);

        Assert.Equal(["Frodo Baggins", "Samwise Gamgee"], titularissen.Value);
    }

    [Fact]
    public void Create_Copies_Input_Array()
    {
        var input = new[] { "Frodo Baggins" };

        var titularissen = Titularissen.Create(input);
        input[0] = "Gollum";

        Assert.Equal("Frodo Baggins", titularissen.Value[0]);
    }

    // ---------------------------------------------------------------
    // Hydrate
    // ---------------------------------------------------------------

    [Fact]
    public void Hydrate_Does_Not_Validate()
    {
        // Hydrate replays historical events verbatim, even ones that would
        // no longer pass Create's validation.
        var titularissen = Titularissen.Hydrate([]);

        Assert.Empty(titularissen.Value);
    }

    [Fact]
    public void Hydrate_Preserves_Value()
    {
        var titularissen = Titularissen.Hydrate(["Frodo Baggins", ""]);

        Assert.Equal(["Frodo Baggins", ""], titularissen.Value);
    }

    [Fact]
    public void Hydrate_Copies_Input_Array()
    {
        var input = new[] { "Frodo Baggins" };

        var titularissen = Titularissen.Hydrate(input);
        input[0] = "Gollum";

        Assert.Equal("Frodo Baggins", titularissen.Value[0]);
    }

    // ---------------------------------------------------------------
    // Replace
    // ---------------------------------------------------------------

    [Fact]
    public void Replace_With_Null_Returns_Same_Instance()
    {
        var original = Titularissen.Create(["Frodo Baggins"]);

        var result = original.Replace(null);

        Assert.Same(original, result);
    }

    [Fact]
    public void Replace_With_Value_Returns_New_Instance_With_New_Value()
    {
        var original = Titularissen.Create(["Frodo Baggins"]);

        var result = original.Replace(["Samwise Gamgee"]);

        Assert.NotSame(original, result);
        Assert.Equal(["Samwise Gamgee"], result.Value);
    }

    [Fact]
    public void Replace_Does_Not_Mutate_Original()
    {
        var original = Titularissen.Create(["Frodo Baggins"]);

        _ = original.Replace(["Samwise Gamgee"]);

        Assert.Equal(["Frodo Baggins"], original.Value);
    }

    [Fact]
    public void Replace_With_Empty_Array_Throws()
    {
        var original = Titularissen.Create(["Frodo Baggins"]);

        Assert.Throws<TitularisMagNietNullOfLeegZijn>(() => original.Replace([]));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Replace_With_NullOrWhiteSpace_Element_Throws(string? element)
    {
        var original = Titularissen.Create(["Frodo Baggins"]);

        Assert.Throws<TitularisMagNietNullOfLeegZijn>(() => original.Replace([element!]));
    }

    // ---------------------------------------------------------------
    // Create - duplicates
    // ---------------------------------------------------------------

    [Fact]
    public void Create_With_Duplicate_Titularis_Throws()
    {
        var exception = Assert.Throws<TitularissenMoetenUniekZijn>(() =>
            Titularissen.Create(["Frodo Baggins", "Frodo Baggins"])
        );
        exception.Message.Should().Be(ExceptionMessages.TitularissenMoetenUniekZijn);
    }

    [Theory]
    [InlineData("frodo baggins")]
    [InlineData("FRODO BAGGINS")]
    [InlineData("Frodo BAGGINS")]
    public void Create_With_Duplicate_Titularis_Different_Casing_Throws(string duplicate)
    {
        var exception = Assert.Throws<TitularissenMoetenUniekZijn>(() =>
            Titularissen.Create(["Frodo Baggins", duplicate])
        );
        exception.Message.Should().Be(ExceptionMessages.TitularissenMoetenUniekZijn);
    }

    [Fact]
    public void Create_With_Distinct_Titularissen_Does_Not_Throw()
    {
        var titularissen = Titularissen.Create(["Frodo Baggins", "Samwise Gamgee"]);

        Assert.Equal(2, titularissen.Value.Length);
    }

    [Fact]
    public void Hydrate_With_Duplicate_Titularis_Does_Not_Validate()
    {
        var titularissen = Titularissen.Hydrate(["Frodo Baggins", "frodo baggins"]);

        Assert.Equal(2, titularissen.Value.Length);
    }

    [Fact]
    public void Replace_With_Duplicate_Titularis_Throws()
    {
        var original = Titularissen.Create(["Frodo Baggins"]);

        Assert.Throws<TitularissenMoetenUniekZijn>(() => original.Replace(["Samwise Gamgee", "samwise gamgee"]));
    }
}
