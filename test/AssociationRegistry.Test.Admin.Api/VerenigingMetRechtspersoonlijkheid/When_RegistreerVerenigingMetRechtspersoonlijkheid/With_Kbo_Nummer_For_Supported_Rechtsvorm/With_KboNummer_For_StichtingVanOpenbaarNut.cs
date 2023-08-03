namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using Fixtures;
using Xunit;

public class RegistreerStichtingVanOpenbaarNutSetup: RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerStichtingVanOpenbaarNutSetup(EventsInDbScenariosFixture fixture): base(fixture, "0468831484")
    {

    }
}

public class With_KboNummer_For_StichtingVanOpenbaarNut: With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerStichtingVanOpenbaarNutSetup>
{
    public With_KboNummer_For_StichtingVanOpenbaarNut(EventsInDbScenariosFixture fixture, RegistreerStichtingVanOpenbaarNutSetup registreerVereniginMetRechtspersoonlijkheidSetup) : base(fixture, registreerVereniginMetRechtspersoonlijkheidSetup)
    {
    }
}
