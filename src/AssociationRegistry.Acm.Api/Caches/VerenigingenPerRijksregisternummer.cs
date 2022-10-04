namespace AssociationRegistry.Acm.Api.Caches;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Api.VerenigingenPerRijksregisternummer;

public class VerenigingenPerRijksregisternummer
{
    private readonly Dictionary<string, ImmutableArray<Vereniging>> _data;

    private VerenigingenPerRijksregisternummer(Dictionary<string, ImmutableArray<Vereniging>> data)
    {
        _data = data;
    }

    public ImmutableArray<Vereniging> this[string rijksRegisterNummer]
        => _data.ContainsKey(rijksRegisterNummer) ? _data[rijksRegisterNummer] : GetByShortRrn(rijksRegisterNummer);

    private ImmutableArray<Vereniging> GetByShortRrn(string rijksRegisterNummer)
        => CalculateKey(rijksRegisterNummer) is { } key && _data.ContainsKey(key) ? _data[key] : ImmutableArray<Vereniging>.Empty;

    private static string? CalculateKey(string rijksregisternummer)
        => rijksregisternummer.Length < 4 ? null : rijksregisternummer[..4];

    public static VerenigingenPerRijksregisternummer Empty()
        => new(new Dictionary<string, ImmutableArray<Vereniging>>());

    public static VerenigingenPerRijksregisternummer FromVerenigingenAsDictionary(VerenigingenAsDictionary dictionary)
        => new(
            dictionary.Select(
                    kv =>
                        (kv.Key, kv.Value.Select(
                                x => new Vereniging(x.Key, x.Value))
                            .ToImmutableArray()
                        ))
                .ToDictionary(x => x.Key, x => x.Item2));
}
