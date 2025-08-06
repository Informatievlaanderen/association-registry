namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;

public abstract class RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public readonly RegistreerVerenigingUitKboRequest UitKboRequest;
    public readonly HttpResponseMessage Response;

    public RegistreerVereniginMetRechtspersoonlijkheidSetup(EventsInDbScenariosFixture fixture, string kboNummer)
    {
        UitKboRequest = new RegistreerVerenigingUitKboRequest
        {
            KboNummer = kboNummer,
        };

        Response ??= fixture.DefaultClient.RegistreerKboVereniging(GetJsonBody(UitKboRequest)).GetAwaiter().GetResult();
    }

    private string GetJsonBody(RegistreerVerenigingUitKboRequest uitKboRequest)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_kboNummer")
          .Replace(oldValue: "{{kboNummer}}", uitKboRequest.KboNummer);
}
