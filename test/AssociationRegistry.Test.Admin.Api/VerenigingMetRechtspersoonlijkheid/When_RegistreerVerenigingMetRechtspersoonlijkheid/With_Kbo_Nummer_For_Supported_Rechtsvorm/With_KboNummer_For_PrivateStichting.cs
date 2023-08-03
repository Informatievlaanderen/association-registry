namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using AssociationRegistry.Test.Admin.Api.Fixtures;
using Xunit;

public class RegistreerPrivateStichtingSetup: RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerPrivateStichtingSetup(EventsInDbScenariosFixture fixture): base(fixture, "0546572531")
    {

    }
}

public class With_KboNummer_For_PrivateStichting: With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerPrivateStichtingSetup>
{
    public With_KboNummer_For_PrivateStichting(EventsInDbScenariosFixture fixture, RegistreerPrivateStichtingSetup registreerSetup) : base(fixture, registreerSetup)
    {
    }
}
