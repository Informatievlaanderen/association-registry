namespace AssociationRegistry.Test.Common.Framework;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Vereniging;

public class WerkingsgebiedenServiceMock : IWerkingsgebiedenService
{
    public static readonly Werkingsgebied[] Provincies =
    [
        Werkingsgebied.Hydrate("BE10", "Brussels Hoofdstedelijk Gewest"),
        Werkingsgebied.Hydrate("BE21", "Provincie Antwerpen"),
        Werkingsgebied.Hydrate("BE22", "Provincie Limburg"),
        Werkingsgebied.Hydrate("BE23", "Provincie Oost-Vlaanderen"),
        Werkingsgebied.Hydrate("BE24", "Provincie Vlaams-Brabant"),
        Werkingsgebied.Hydrate("BE25", "Provincie West-Vlaanderen"),
    ];

    public static readonly Werkingsgebied[] All = Provincies
        .Union(NutsLauInfoMock.All.Select(x => Werkingsgebied.Hydrate(x.Nuts3 + x.Lau, x.Gemeentenaam)).ToArray())
        .ToArray();

    public static Werkingsgebied[] AllWithNvt => All.Append(Werkingsgebied.NietVanToepassing).ToArray();

    public Task<Werkingsgebied> Create(string code, CancellationToken cancellationToken = default) =>
        Task.FromResult(Get(code));

    public Task<Werkingsgebied[]> CreateMany(
        IEnumerable<string> codes,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(codes.Select(Get).ToArray());

    public Task<IReadOnlyList<Werkingsgebied>> AllWithNVT(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Werkingsgebied>>(AllWithNvt);

    public static Werkingsgebied Get(string code)
    {
        var match = AllWithNvt.SingleOrDefault(w =>
            string.Equals(w.Code, code, StringComparison.InvariantCultureIgnoreCase)
        );

        return match ?? throw new WerkingsgebiedCodeIsNietGekend(code);
    }
}
