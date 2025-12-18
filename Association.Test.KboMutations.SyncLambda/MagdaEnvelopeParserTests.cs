namespace Association.Test.KboMutations.SyncLambda;

using AssociationRegistry.KboMutations.SyncLambda;
using System.Text.Json;

public class MagdaEnvelopeParserTests
{
    [Fact]
    public void Parse_WhenOnlyKboNummerPresent_ReturnsSyncKbo()
    {
        // Arrange
        var json = """{ "KboNummer": "0123456789" }""";

        // Act
        var result = MagdaEnvelopeParser.Parse(json);

        // Assert
        Assert.Equal(MagdaMessageType.SyncKbo, result.Type);
        Assert.Equal("0123456789", result.KboNummer);
        Assert.Null(result.InszMessage);
    }

    [Fact]
    public void Parse_WhenOnlyInszPresent_ReturnsSyncKsz_WithOverledenDefaultFalse()
    {
        // Arrange
        var json = """{ "Insz": "06321184845" }""";

        // Act
        var result = MagdaEnvelopeParser.Parse(json);

        // Assert
        Assert.Equal(MagdaMessageType.SyncKsz, result.Type);
        Assert.Null(result.KboNummer);
        Assert.NotNull(result.InszMessage);
        Assert.Equal("06321184845", result.InszMessage!.Insz);
        Assert.False(result.InszMessage.Overleden);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Parse_WhenInszAndOverledenPresent_ReturnsSyncKsz_WithOverledenValue(bool overleden)
    {
        // Arrange
        var json = $$"""{"Insz": "06321184845","Overleden": {{overleden.ToString().ToLowerInvariant()}} }""";

        // Act
        var result = MagdaEnvelopeParser.Parse(json);

        // Assert
        Assert.Equal(MagdaMessageType.SyncKsz, result.Type);
        Assert.Null(result.KboNummer);
        Assert.NotNull(result.InszMessage);
        Assert.Equal("06321184845", result.InszMessage!.Insz);
        Assert.Equal(overleden, result.InszMessage.Overleden);
    }

    [Fact]
    public void Parse_WhenInszPresentAndOverledenIsNonBoolean_DefaultsOverledenFalse()
    {
        // Arrange
        // Overleden is a string -> GetBool should return null -> default false
        var json = """{ "Insz": "06321184845", "Overleden": "false" }""";

        // Act
        var result = MagdaEnvelopeParser.Parse(json);

        // Assert
        Assert.Equal(MagdaMessageType.SyncKsz, result.Type);
        Assert.NotNull(result.InszMessage);
        Assert.False(result.InszMessage!.Overleden);
    }

    [Fact]
    public void Parse_WhenKboNummerAndInszPresent_ReturnsUnknown_WithBothValues()
    {
        // Arrange
        var json = """{ "KboNummer": "0123456789", "Insz": "06321184845", "Overleden": true}""";

        // Act
        var result = MagdaEnvelopeParser.Parse(json);

        // Assert
        Assert.Equal(MagdaMessageType.Unknown, result.Type);
        Assert.Equal("0123456789", result.KboNummer);
        Assert.NotNull(result.InszMessage);
        Assert.Equal("06321184845", result.InszMessage!.Insz);
        Assert.True(result.InszMessage.Overleden);
    }

    [Fact]
    public void Parse_WhenNeitherKboNummerNorInszPresent_ReturnsUnknown_WithNulls()
    {
        // Arrange
        var json = """{ "SomethingElse": "x" }""";

        // Act
        var result = MagdaEnvelopeParser.Parse(json);

        // Assert
        Assert.Equal(MagdaMessageType.Unknown, result.Type);
        Assert.Null(result.KboNummer);
        Assert.Null(result.InszMessage);
    }

    [Fact]
    public void Parse_WhenKboNummerIsWhitespaceAndInszPresent_TreatsKboAsMissing_AndReturnsSyncKsz()
    {
        // Arrange
        var json = """{ "KboNummer": "   ", "Insz": "06321184845" }""";

        // Act
        var result = MagdaEnvelopeParser.Parse(json);

        // Assert
        Assert.Equal(MagdaMessageType.SyncKsz, result.Type);
        Assert.Null(result.KboNummer); // note: parser returns the raw string; this asserts intent
        Assert.NotNull(result.InszMessage);
        Assert.Equal("06321184845", result.InszMessage!.Insz);
    }

    [Fact]
    public void Parse_WhenBodyIsInvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = """{ "Insz": "06321184845" """;

        // Act + Assert
        Assert.Throws<JsonException>(() => MagdaEnvelopeParser.Parse(invalidJson));
    }
}

