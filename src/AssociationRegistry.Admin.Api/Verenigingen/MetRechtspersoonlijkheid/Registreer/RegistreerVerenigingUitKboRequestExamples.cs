namespace AssociationRegistry.Admin.Api.Verenigingen.MetRechtspersoonlijkheid.Registreer;

using Swashbuckle.AspNetCore.Filters;

public class RegistreerVerenigingUitKboRequestExamples : IExamplesProvider<RegistreerVerenigingUitKboRequest>
{
    public RegistreerVerenigingUitKboRequest GetExamples()
        => new()
        {
            KboNummer = "0000000000",
        };
}
