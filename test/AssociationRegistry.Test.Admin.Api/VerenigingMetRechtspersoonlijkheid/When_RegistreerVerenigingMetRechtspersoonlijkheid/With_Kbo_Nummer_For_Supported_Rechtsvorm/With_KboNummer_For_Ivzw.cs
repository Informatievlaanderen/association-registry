namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using Fixtures;
using Xunit;

public class RegistreerIVzwSetup: RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerIVzwSetup(EventsInDbScenariosFixture fixture): base(fixture, "0824992720")
    {

    }
}

public class With_KboNummer_For_Ivzw: With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerIVzwSetup>
{
    public With_KboNummer_For_Ivzw(EventsInDbScenariosFixture fixture, RegistreerIVzwSetup registreerVereniginMetRechtspersoonlijkheidSetup) : base(fixture, registreerVereniginMetRechtspersoonlijkheidSetup)
    {
    }
}
