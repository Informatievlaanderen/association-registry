namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.List;

using AssociationRegistry.Grar.Contracts;

public class PostalInformationListOsloResponseBuilder
{
    private readonly List<PostalInformationListItemOsloResponse> _items = new();
    private Uri _volgende;

    public PostalInformationListOsloResponseBuilder WithPostInfo(string objectId, params string[] postnamen)
    {
        var postnaamList = new List<Postnaam>();

        foreach (var naam in postnamen)
        {
            postnaamList.Add(new Postnaam(new GeografischeNaam
            {
                Spelling = naam,
            }));
        }

        _items.Add(new PostalInformationListItemOsloResponse
        {
            Identificator = new PostinfoIdentificator { ObjectId = objectId, Id = "1", Naamruimte = "2", Versie = "3"},
            Postnamen = postnaamList,
            Detail = new Uri("https://example.com/detail"),
            PostInfoStatus = PostInfoStatus.Gerealiseerd
        });

        return this;
    }

    public PostalInformationListOsloResponseBuilder WithVolgende(string url)
    {
        _volgende = new Uri(url);
        return this;
    }

    public PostalInformationListOsloResponse Build()
    {
        return new PostalInformationListOsloResponse
        {
            Context = "https://example.com/context",
            PostInfoObjecten = _items,
            Volgende = _volgende
        };
    }
}
