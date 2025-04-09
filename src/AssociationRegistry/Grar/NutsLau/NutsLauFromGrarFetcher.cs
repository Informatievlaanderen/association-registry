namespace AssociationRegistry.Grar.NutsLau;

using Clients;
using Marten.Schema;

public class NutsLauFromGrarFetcher : INutsLauFromGrarFetcher
{
    private readonly IGrarClient _client;


    public NutsLauFromGrarFetcher(IGrarClient client)
    {
        _client = client;
    }

    public async Task<PostalNutsLauInfo[]> GetFlemishNutsAndLauByPostcode(string[] postcodes, CancellationToken cancellationToken)
    {
        var nutsLauInfos = new List<PostalNutsLauInfo>();

        foreach (var postcode in postcodes)
        {
            if (Postcode.IsWaalsePostcode(postcode))
                continue;

            var postInfo = await _client.GetPostalNutsLauInformation(postcode, cancellationToken);

            if (postInfo is not null)
                nutsLauInfos.Add(new PostalNutsLauInfo()
                {
                    Postcode = postInfo.Postcode,
                    Gemeentenaam = postInfo.Gemeentenaam,
                    Nuts = postInfo.Nuts,
                    Lau = postInfo.Lau,
                });
        }

        return nutsLauInfos.ToArray();
    }
}

public interface INutsLauFromGrarFetcher
{
    Task<PostalNutsLauInfo[]> GetFlemishNutsAndLauByPostcode(string[] postalInformationList, CancellationToken cancellationToken);
}

public record PostalNutsLauInfo
{
    [Identity]
    public string Postcode { get; set; }
    public string Gemeentenaam { get; set; }
    public string Nuts { get; set; }
    public string Lau { get; set; }
};
