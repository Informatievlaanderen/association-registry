namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;

using Common;
using Swashbuckle.AspNetCore.Filters;

public class VoegVertegenwoordigerToeRequestExamples : IExamplesProvider<VoegVertegenwoordigerToeRequest>
{
    public VoegVertegenwoordigerToeRequest GetExamples()
        => new()
        {
            Initiator = "OVO000001",
            Vertegenwoordiger = new ToeTeVoegenVertegenwoordiger
            {
                Insz = "yymmddxxxcc",
                PrimairContactpersoon = true,
                Roepnaam = "Conan",
                Rol = "Barbarian",
                Email = "conan@example.com",
                Telefoon = "0000112233",
                Mobiel = "9999887766",
                SocialMedia = "http://example.org",
            },
        };
}
