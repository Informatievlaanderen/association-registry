namespace AssociationRegistry.Test.Grar.NutsLau;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.NutsLau;

public class NutsLauFromGrarFetcher : INutsLauFromGrarFetcher
{
    private readonly IGrarClient _client;


    public NutsLauFromGrarFetcher(IGrarClient client)
    {
        _client = client;
    }

    public async Task<PostalNutsLauInfo[]> GetFlemishNutsAndLauByPostcode(string[] postcodes)
    {
        var nutsLauInfos = new List<PostalNutsLauInfo>();

        foreach (var postcode in postcodes)
        {
            if (!Postcode.IsVlaamsePostcode(postcode))
                continue;

            var postInfo = await _client.GetPostalNutsLauInformation(postcode);
            if (postInfo is not null)
                nutsLauInfos.Add(new PostalNutsLauInfo(postInfo.Postcode, postInfo.Gemeentenaam, postInfo.Nuts, postInfo.Lau));
        }

        return nutsLauInfos.ToArray();
    }
}

public interface INutsLauFromGrarFetcher
{
    Task<PostalNutsLauInfo[]> GetFlemishNutsAndLauByPostcode(string[] postalInformationList);
}

public record PostalNutsLauInfo(
    string Postcode,
    string Gemeentenaam,
    string Nuts,
    string Lau);
