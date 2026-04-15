namespace AssociationRegistry.Integrations.Ipdc.Options;

using Framework;

public class IpdcOptions
{
    public string BaseUrl { get; set; }

    public void ThrowIfInValid()
    {
        Throw<ArgumentNullException>.IfNullOrWhiteSpace(BaseUrl, nameof(BaseUrl));
    }
}
