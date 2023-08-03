namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using Fixtures;
using Xunit;

public class RegistreerOnbekendTypeRegistreerSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerOnbekendTypeRegistreerSetup(EventsInDbScenariosFixture fixture) : base(fixture, "0563634435")
    {
    }
}

public class With_KboNummer_For_OnbekendType : With_KboNummer_For_Unsupported_Vereniging, IClassFixture<RegistreerOnbekendTypeRegistreerSetup>
{
    public With_KboNummer_For_OnbekendType(EventsInDbScenariosFixture fixture, RegistreerOnbekendTypeRegistreerSetup registreerSetup) : base(fixture, registreerSetup)
    {
    }
}
