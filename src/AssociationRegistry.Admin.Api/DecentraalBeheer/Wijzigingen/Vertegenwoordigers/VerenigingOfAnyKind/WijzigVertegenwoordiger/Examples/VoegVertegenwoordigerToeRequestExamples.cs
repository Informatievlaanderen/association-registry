namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigVertegenwoordigerRequestExamples : IExamplesProvider<WijzigVertegenwoordigerRequest>
{
    public WijzigVertegenwoordigerRequest GetExamples()
        => new()
        {
            Vertegenwoordiger = new TeWijzigenVertegenwoordiger
            {
                IsPrimair = true,
                Roepnaam = "Conan",
                Rol = "Barbarian",
                Email = "conan@example.com",
                Telefoon = "0000112233",
                Mobiel = "9999887766",
                SocialMedia = "http://example.org",
            },
        };
}
