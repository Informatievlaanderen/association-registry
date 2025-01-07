namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Framework;
using Framework.Fixtures;

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
