namespace AssociationRegistry.Grar.NutsLau;

using Models.PostalInfo;

public class PostcodesFromGrarFetcher:  IPostcodesFromGrarFetcher
{
    private readonly IGrarClient _client;

    public PostcodesFromGrarFetcher(IGrarClient client)
    {
        _client = client;
    }

    public async Task<string[]> FetchPostalCodes(CancellationToken cancellationToken)
    {
        var postalInformationList = await _client.GetPostalInformationList("0", "100", cancellationToken);

        if (!postalInformationList.Postcodes.Any())
            return [];

        var postcodes = postalInformationList.Postcodes.ToList();

        while (HasVolgende(postalInformationList))
        {
            postalInformationList =
                await _client.GetPostalInformationList(postalInformationList!.VolgendeOffset!, postalInformationList!.VolgendeLimit!, cancellationToken);

            if (postalInformationList.Postcodes.Any())
                postcodes.AddRange(postalInformationList.Postcodes);
        }

        return postcodes.ToArray();
    }

    private static bool HasVolgende(PostcodesLijstResponse? postalInformationList)
        => postalInformationList?.VolgendeOffset is not null && postalInformationList?.VolgendeLimit is not null;
}
