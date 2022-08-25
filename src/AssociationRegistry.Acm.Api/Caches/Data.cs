using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;

namespace AssociationRegistry.Acm.Api.Caches;

public class Data
{
    public Dictionary<string, ImmutableArray<Vereniging>> Verenigingen { get; set; } = null!;

    public static bool TryParse(Dictionary<string, Dictionary<string, string>> dictionary,
        out Dictionary<string, ImmutableArray<Vereniging>> verenigingen)
    {
        try
        {
            verenigingen = dictionary.Select(
                    kv =>
                        (kv.Key, kv.Value.Select(
                                x => new Vereniging(x.Key, x.Value))
                            .ToImmutableArray()
                        ))
                .ToDictionary(x => x.Key, x => x.Item2);
            return true;
        }
        catch
        {
            verenigingen = null!;
            return false;
        }
    }
}
