namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;

public class SequentialVCodeService : IVCodeService
{
    private static int vCode = 0;

    public Task<string> GetNext()
        => Task.FromResult($"V{++vCode:000000}");

    /// <summary>
    /// only for testing
    /// </summary>
    public static string GetLast()
        => $"V{vCode:000000}";
}
