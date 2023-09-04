namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe.Examples;

using Common;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class VoegVertegenwoordigerToeRequestExamples : IExamplesProvider<VoegVertegenwoordigerToeRequest>
{
    public VoegVertegenwoordigerToeRequest GetExamples()
        => new()
        {
            Vertegenwoordiger = new ToeTeVoegenVertegenwoordiger
            {
                Insz = "01234567890",
                Voornaam = "Conan",
                Achternaam = "The Barbarian",
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
