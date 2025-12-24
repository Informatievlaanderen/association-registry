namespace AssociationRegistry.Test.MagdaSync.KboSync;

using AssociationRegistry.KboMutations.CloudEvents;
using AssociationRegistry.KboMutations.SyncLambda.Messaging;
using AssociationRegistry.KboMutations.SyncLambda.Messaging.Parsers;
using System.Diagnostics;
using Xunit;

/// <summary>
/// Tests for parsing CloudEvents messages (new format with distributed tracing)
/// </summary>
public class SyncEnvelopeParser_CloudEventTests
{
    [Fact]
    public void Parse_CloudEvent_WhenOnlyKboNummerPresent_ReturnsSyncKbo()
    {
        // Arrange
        var cloudEventJson = CloudEventBuilder.KboSyncOrganisationQueued()
                                              .WithData(new { KboNummer = "0123456789" })
                                              .BuildAsJson();

        // Act
        var result = SyncEnvelopeParser.Parse(cloudEventJson);

        // Assert
        Assert.Equal(SyncMessageType.SyncKbo, result.Type);
        Assert.Equal("0123456789", result.KboNummer);
        Assert.Null(result.InszMessage);
    }

    [Fact]
    public void Parse_CloudEvent_WhenOnlyInszPresent_ReturnsSyncKsz_WithOverledenDefaultFalse()
    {
        // Arrange
        var cloudEventJson = CloudEventBuilder.KboSyncPersonQueued()
                                              .WithData(new { Insz = "06321184845" })
                                              .BuildAsJson();

        // Act
        var result = SyncEnvelopeParser.Parse(cloudEventJson);

        // Assert
        Assert.Equal(SyncMessageType.SyncKsz, result.Type);
        Assert.Null(result.KboNummer);
        Assert.NotNull(result.InszMessage);
        Assert.Equal("06321184845", result.InszMessage!.Insz);
        Assert.False(result.InszMessage.Overleden);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Parse_CloudEvent_WhenInszAndOverledenPresent_ReturnsSyncKsz_WithOverledenValue(bool overleden)
    {
        // Arrange
        var cloudEventJson = CloudEventBuilder.KboSyncPersonQueued()
                                              .WithData(new { Insz = "06321184845", Overleden = overleden })
                                              .BuildAsJson();

        // Act
        var result = SyncEnvelopeParser.Parse(cloudEventJson);

        // Assert
        Assert.Equal(SyncMessageType.SyncKsz, result.Type);
        Assert.Null(result.KboNummer);
        Assert.NotNull(result.InszMessage);
        Assert.Equal("06321184845", result.InszMessage!.Insz);
        Assert.Equal(overleden, result.InszMessage.Overleden);
    }

    [Fact]
    public void Parse_CloudEvent_ExtractsSourceFileName()
    {
        // Arrange
        var cloudEventJson = CloudEventBuilder.KboSyncOrganisationQueued()
                                              .WithData(new { KboNummer = "0123456789" })
                                              .FromFile("pub_mut_klanten-ondernemingen.csv")
                                              .BuildAsJson();

        // Act
        var result = SyncEnvelopeParser.Parse(cloudEventJson);

        // Assert
        Assert.Equal("pub_mut_klanten-ondernemingen.csv", result.SourceFileName);
    }

    [Fact]
    public void Parse_CloudEvent_ExtractsTraceContext()
    {
        // Arrange
        var activitySource = new ActivitySource("test");
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);

        using var activity = activitySource.StartActivity("test-activity");

        var cloudEventJson = CloudEventBuilder.KboSyncOrganisationQueued()
                                              .WithData(new { KboNummer = "0123456789" })
                                              .BuildAsJson();

        // Act
        var result = SyncEnvelopeParser.Parse(cloudEventJson);

        // Assert
        Assert.NotNull(result.ParentTraceContext);
        Assert.Equal(activity!.TraceId, result.ParentTraceContext!.Value.TraceId);
    }

    [Fact]
    public void Parse_CloudEvent_WhenNoActivity_TraceContextIsNull()
    {
        // Arrange - no active Activity
        var cloudEventJson = CloudEventBuilder.KboSyncOrganisationQueued()
                                              .WithData(new { KboNummer = "0123456789" })
                                              .BuildAsJson();

        // Act
        var result = SyncEnvelopeParser.Parse(cloudEventJson);

        // Assert
        Assert.Null(result.ParentTraceContext);
    }

    [Fact]
    public void Parse_CloudEvent_WhenKboNummerAndInszPresent_ReturnsUnknown()
    {
        // Arrange
        var cloudEventJson = CloudEventBuilder.KboSyncOrganisationQueued()
                                              .WithData(new { KboNummer = "0123456789", Insz = "06321184845", Overleden = true })
                                              .BuildAsJson();

        // Act
        var result = SyncEnvelopeParser.Parse(cloudEventJson);

        // Assert
        Assert.Equal(SyncMessageType.Unknown, result.Type);
        Assert.Equal("0123456789", result.KboNummer);
        Assert.NotNull(result.InszMessage);
        Assert.Equal("06321184845", result.InszMessage!.Insz);
        Assert.True(result.InszMessage.Overleden);
    }

    [Fact]
    public void Parse_CloudEvent_FunctionQueuedType_ParsesCorrectly()
    {
        // Arrange
        var cloudEventJson = CloudEventBuilder.KboSyncFunctionQueued()
                                              .WithData(new { KboNummer = "9876543210" })
                                              .BuildAsJson();

        // Act
        var result = SyncEnvelopeParser.Parse(cloudEventJson);

        // Assert
        Assert.Equal(SyncMessageType.SyncKbo, result.Type);
        Assert.Equal("9876543210", result.KboNummer);
    }
}
