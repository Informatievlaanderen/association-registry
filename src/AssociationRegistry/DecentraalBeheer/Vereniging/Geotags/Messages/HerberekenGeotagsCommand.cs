namespace AssociationRegistry.DecentraalBeheer.Vereniging.Geotags.Messages;

public record HerberekenGeotagsCommand
{
    public VCode VCode { get; }

    public HerberekenGeotagsCommand(string vCode)
    {
        VCode = VCode.Create(vCode);
    }
}
