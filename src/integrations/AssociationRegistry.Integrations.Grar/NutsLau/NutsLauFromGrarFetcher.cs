namespace AssociationRegistry.Integrations.Grar.NutsLau;

using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Integrations.Grar.Clients;
using Microsoft.Extensions.Logging;

public class NutsLauFromGrarFetcher : INutsLauFromGrarFetcher
{
    private readonly IGrarClient _client;
    private readonly IPostcodesFromGrarFetcher _postcodesFromGrarFetcher;
    private readonly ILogger<NutsLauFromGrarFetcher> _logger;

    public NutsLauFromGrarFetcher(IGrarClient client,
                                  IPostcodesFromGrarFetcher postcodesFromGrarFetcher,
                                  ILogger<NutsLauFromGrarFetcher> logger)
    {
        _client = client;
        _postcodesFromGrarFetcher = postcodesFromGrarFetcher;
        _logger = logger;
    }

    public async Task<PostalNutsLauInfo[]> GetFlemishAndBrusselsNutsAndLauByPostcode(CancellationToken cancellationToken)
    {
        var postcodes = await _postcodesFromGrarFetcher.FetchPostalCodes(cancellationToken);

        _logger.LogInformation($"PostcodesFromGrarFetcher returned {postcodes.Length} postcodes.");

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
                    Nuts3 = postInfo.Nuts,
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
                Nuts3 = "BE224",
                Lau = "71034",
            }
        },
        {
            "9950", new PostalNutsLauInfo()
            {
                Postcode = "9950",
                Gemeentenaam = "Lievegem",
                Nuts3 = "BE234",
                Lau = "44085",
            }
        },
        {
            "9771", new PostalNutsLauInfo()
            {
                Postcode = "9771",
                Gemeentenaam = "Kruisem",
                Nuts3 = "BE235",
                Lau = "45068",
            }
        },
        {
            "3080", new PostalNutsLauInfo()
            {
                Postcode = "3080",
                Gemeentenaam = "Tervuren",
                Nuts3 = "BE242",
                Lau = "24104",
            }
        },
        {
            "2070", new PostalNutsLauInfo()
            {
                Postcode = "2070",
                Gemeentenaam = "Beveren-Kruibeke-Zwijndrecht",
                Nuts3 = "BE236",
                Lau = "46030",
            }
        },
        {
            "3720", new PostalNutsLauInfo()
            {
                Postcode = "3720",
                Gemeentenaam = "Hasselt",
                Nuts3 = "BE224",
                Lau = "71072",
            }
        },
        {
            "3721", new PostalNutsLauInfo()
            {
                Postcode = "3721",
                Gemeentenaam = "Hasselt",
                Nuts3 = "BE224",
                Lau = "71072",
            }
        },
        {
            "3722", new PostalNutsLauInfo()
            {
                Postcode = "3722",
                Gemeentenaam = "Hasselt",
                Nuts3 = "BE224",
                Lau = "71072",
            }
        },
        {
            "3723", new PostalNutsLauInfo()
            {
                Postcode = "3723",
                Gemeentenaam = "Hasselt",
                Nuts3 = "BE224",
                Lau = "71072",
            }
        },
        {
            "3724", new PostalNutsLauInfo()
            {
                Postcode = "3724",
                Gemeentenaam = "Hasselt",
                Nuts3 = "BE224",
                Lau = "71072",
            }
        },
        {
            "9180", new PostalNutsLauInfo()
            {
                Postcode = "9180",
                Gemeentenaam = "Lokeren",
                Nuts3 = "BE236",
                Lau = "46029",
            }
        },
    };
}
