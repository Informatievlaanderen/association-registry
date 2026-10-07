namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.Grar.NutsLau;

/// <summary>
///     Returns the given postcodes plus the configured extra postcodes. Without extras it behaves as a pass-through.
/// </summary>
public class FakePostcodesInSameGemeenteService : IPostcodesInSameGemeenteService
{
    private readonly string[] _extraPostcodes;

    public FakePostcodesInSameGemeenteService(params string[] extraPostcodes)
    {
        _extraPostcodes = extraPostcodes;
    }

    public Task<string[]> WithPostcodesOfSameGemeente(string[] postcodes) =>
        Task.FromResult(postcodes.Concat(_extraPostcodes).Distinct().ToArray());
}
