namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using AssociationRegistry.Test.Admin.Api.Fixtures;
using Xunit;

public class RegistreerVzwRegistreerSetup: RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerVzwRegistreerSetup(EventsInDbScenariosFixture fixture): base(fixture, "0554790609")
    {

    }
}

public class With_KboNummer_For_Vzw: With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerVzwRegistreerSetup>
{
    public With_KboNummer_For_Vzw(EventsInDbScenariosFixture fixture, RegistreerVzwRegistreerSetup registreerSetup) : base(fixture, registreerSetup)
    {
    }
}
