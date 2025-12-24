namespace AssociationRegistry.KboMutations.SyncLambda.Messaging.Parsers;

using AssociationRegistry.KboMutations.CloudEvents;
using System.Text.Json;

public static class MagdaEnvelopeParser
{
    public static MagdaEnvelope Parse(string body)
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
        catch (ArgumentException)
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
