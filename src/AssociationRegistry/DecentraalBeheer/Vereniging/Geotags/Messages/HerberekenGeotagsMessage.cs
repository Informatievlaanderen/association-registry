namespace AssociationRegistry.DecentraalBeheer.Vereniging.Geotags.Messages;

public class HerberekenGeotagsMessage
{
    public string StreamKey { get; }

    public HerberekenGeotagsMessage(string streamKey)
    {
        StreamKey = streamKey;
    }
}

public static class StartBewaartermijnMessageExtensions
{
    public static HerberekenGeotagsCommand ToCommand(this HerberekenGeotagsMessage message) => new(message.StreamKey);
}
