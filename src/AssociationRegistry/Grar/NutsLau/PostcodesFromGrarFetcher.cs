namespace AssociationRegistry.Grar.NutsLau;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models.PostalInfo;

public class PostcodesFromGrarFetcher:  IPostcodesFromGrarFetcher
{
    private readonly IGrarClient _client;

    public PostcodesFromGrarFetcher(IGrarClient client)
    {
        _client = client;
    }

    public async Task<string[]> FetchPostalCodes()
    {
        var postalInformationList = await _client.GetPostalInformationList("0", "100");

        if (postalInformationList is null)
            return Array.Empty<string>();

        var postcodes = postalInformationList.Postcodes.ToList();

        while (HasVolgende(postalInformationList))
        {
            postalInformationList =
                await _client.GetPostalInformationList(postalInformationList!.VolgendeOffset!, postalInformationList!.VolgendeLimit!);

            if (postalInformationList is not null)
                postcodes.AddRange(postalInformationList.Postcodes);
        }

        return postcodes.ToArray();
    }

    private static bool HasVolgende(PostcodesLijstResponse? postalInformationList)
        => postalInformationList?.VolgendeOffset is not null && postalInformationList?.VolgendeLimit is not null;
}

public interface IPostcodesFromGrarFetcher
{
    Task<string[]> FetchPostalCodes();
}
