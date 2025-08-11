namespace AssociationRegistry.Test.Grar.GrarClient.When_Getting_PostInfo.NutsLauInfo;

using Contracts.Contracts;

public class PostalInformationOsloResponseBuilder
{
    private string _postcode = "default-id";
    private string _lau = "default-id";
    private string _gemeenteNaam = "DefaultGemeente";
    private string _nutsCode = "DefaultNutsCode";

    public PostalInformationOsloResponseBuilder withPostcode(string postcode)
    {
        _postcode = postcode;
        return this;
    }

    public PostalInformationOsloResponseBuilder WithLau(string objectId)
    {
        _lau = objectId;
        return this;
    }

    public PostalInformationOsloResponseBuilder WithGemeente(string gemeentenaam)
    {
        _gemeenteNaam = gemeentenaam;
        return this;
    }

    public PostalInformationOsloResponseBuilder WithNuts(string nutsCode)
    {
        _nutsCode = nutsCode;
        return this;
    }

    public PostalInformationOsloResponse Build()
        => new()
        {
            Context = "DefaultContext",
            Identificator = new PostinfoIdentificator
            {
                ObjectId = _postcode,
                Id = "id",
                Naamruimte = "naamruimte",
                Versie = "1"
            },
            Gemeente = new PostinfoDetailGemeente
            {
                ObjectId = _lau,
                Detail = "https://example.com/gemeente-detail",
                Gemeentenaam = new Gemeentenaam
                {
                    GeografischeNaam = new GeografischeNaam
                    {
                        Spelling = _gemeenteNaam
                    }
                }
            },
            Postnamen = new List<Postnaam>
            {
                new Postnaam(new GeografischeNaam { Spelling = _gemeenteNaam })
            },
            PostInfoStatus = PostInfoStatus.Gerealiseerd,
            Nuts3Code = _nutsCode,
        };

    public PostalInformationOsloResponse BuildWithMinimalFields(string postcode)
        => new()
        {
            Identificator = new PostinfoIdentificator
            {
                ObjectId = postcode,
                Id = "id",
                Naamruimte = "naamruimte",
                Versie = "1"
            },
            Postnamen = new List<Postnaam>
            {
                new Postnaam(new GeografischeNaam { Spelling = _gemeenteNaam })
            },
            PostInfoStatus = PostInfoStatus.Gerealiseerd,
        };
}
