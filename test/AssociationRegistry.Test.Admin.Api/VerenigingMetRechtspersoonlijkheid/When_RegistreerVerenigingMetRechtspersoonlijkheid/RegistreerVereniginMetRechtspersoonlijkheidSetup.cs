namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Fixtures;
using Framework;

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
            .Replace("{{kboNummer}}", uitKboRequest.KboNummer);
}
