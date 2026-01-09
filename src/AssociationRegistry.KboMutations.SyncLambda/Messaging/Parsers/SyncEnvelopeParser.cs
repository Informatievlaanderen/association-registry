namespace AssociationRegistry.KboMutations.SyncLambda.Messaging.Parsers;

using Contracts.CloudEvents;
using System.Text.Json;

public static class SyncEnvelopeParser
{
    public static SyncEnvelope Parse(string body)
    {
        var parser = TryParseAsCloudEvent(body) ?? ParseAsPlainJson(body);
        return parser.ToEnvelope();
    }

    private static IMessageParser? TryParseAsCloudEvent(string body)
    {
        try
        {
            var cloudEvent = CloudEventExtensions.FromJson(body);
            return cloudEvent != null ? new CloudEventMessageParser(cloudEvent) : null;
        }
        catch
        {
            // Not a CloudEvent, fallback to plain JSON
            return null;
        }
    }

    private static IMessageParser ParseAsPlainJson(string body)
    {
        var doc = JsonDocument.Parse(body);
        return new PlainJsonMessageParser(doc);
    }
}
