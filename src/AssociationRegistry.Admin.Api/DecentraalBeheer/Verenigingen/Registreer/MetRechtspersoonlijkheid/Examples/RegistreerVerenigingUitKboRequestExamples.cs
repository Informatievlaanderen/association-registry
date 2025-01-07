namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class RegistreerVerenigingUitKboRequestExamples : IExamplesProvider<RegistreerVerenigingUitKboRequest>
{
    public RegistreerVerenigingUitKboRequest GetExamples()
        => new()
        {
            KboNummer = "0000000000",
        };
}
