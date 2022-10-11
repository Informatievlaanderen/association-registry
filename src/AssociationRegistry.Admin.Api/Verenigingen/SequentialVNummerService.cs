namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;

public class SequentialVNummerService : IVNummerService
{
    private static int V_NUMMER = 0;

    public async Task<string> GetNext()
        => $"V{++V_NUMMER:000000}";

    public string GetLast()
        => $"V{V_NUMMER:000000}";
}
