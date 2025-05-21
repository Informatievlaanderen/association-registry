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

    public async Task<PostalNutsLauInfo[]> GetFlemishAndBrusselsNutsAndLauByPostcode(
        string[] postcodes,
        CancellationToken cancellationToken)
    {
        var nutsLauInfos = new List<PostalNutsLauInfo>();

        foreach (var postcode in postcodes)
        {
            if (Postcode.IsWaalsePostcode(postcode))
                continue;

            if (Uitzonderingen.TryGetValue(postcode, out var overriddenInfo))
            {
                nutsLauInfos.Add(overriddenInfo);
                continue;
            }

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

    public static readonly Dictionary<string, PostalNutsLauInfo> Uitzonderingen = new()
    {
        {
            "3970", new PostalNutsLauInfo()
            {
                Postcode = "3970",
                Gemeentenaam = "Leopoldsburg",
                Nuts = "BE224",
                Lau = "71034",
            }
        },
        {
            "9950", new PostalNutsLauInfo()
            {
                Postcode = "9950",
                Gemeentenaam = "Lievegem",
                Nuts = "BE234",
                Lau = "44085",
            }
        },
        {
            "9771", new PostalNutsLauInfo()
            {
                Postcode = "9771",
                Gemeentenaam = "Kruisem",
                Nuts = "BE235",
                Lau = "45068",
            }
        },
        {
            "3080", new PostalNutsLauInfo()
            {
                Postcode = "3080",
                Gemeentenaam = "Tervuren",
                Nuts = "BE242",
                Lau = "24104",
            }
        },
        {
            "2070", new PostalNutsLauInfo()
            {
                Postcode = "2070",
                Gemeentenaam = "Beveren-Kruibeke-Zwijndrecht",
                Nuts = "BE236",
                Lau = "46030",
            }
        },
        {
            "3720", new PostalNutsLauInfo()
            {
                Postcode = "3720",
                Gemeentenaam = "Hasselt",
                Nuts = "BE224",
                Lau = "71072",
            }
        },
        {
            "3721", new PostalNutsLauInfo()
            {
                Postcode = "3721",
                Gemeentenaam = "Hasselt",
                Nuts = "BE224",
                Lau = "71072",
            }
        },
        {
            "3722", new PostalNutsLauInfo()
            {
                Postcode = "3722",
                Gemeentenaam = "Hasselt",
                Nuts = "BE224",
                Lau = "71072",
            }
        },
        {
            "3723", new PostalNutsLauInfo()
            {
                Postcode = "3723",
                Gemeentenaam = "Hasselt",
                Nuts = "BE224",
                Lau = "71072",
            }
        },
        {
            "3724", new PostalNutsLauInfo()
            {
                Postcode = "3724",
                Gemeentenaam = "Hasselt",
                Nuts = "BE224",
                Lau = "71072",
            }
        },
        {
            "9180", new PostalNutsLauInfo()
            {
                Postcode = "9180",
                Gemeentenaam = "Lokeren",
                Nuts = "BE236",
                Lau = "46029",
            }
        },
    };
}

public interface INutsLauFromGrarFetcher
{
    Task<PostalNutsLauInfo[]> GetFlemishAndBrusselsNutsAndLauByPostcode(
        string[] postalInformationList,
        CancellationToken cancellationToken);
}

public record PostalNutsLauInfo
{
    [Identity]
    public string Postcode { get; set; }
    public string Gemeentenaam { get; set; }
    public string Nuts { get; set; }
    public string Lau { get; set; }
    public string Werkingsgebied => $"{Nuts}{Lau}";
    public string Nuts2 => Nuts[..4];
};
