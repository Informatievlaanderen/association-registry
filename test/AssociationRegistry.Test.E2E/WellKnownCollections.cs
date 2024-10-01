namespace AssociationRegistry.Test.E2E;

using Framework.ApiSetup;
using When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;
using When_Registreer_FeitelijkeVereniging;
using When_Stop_Vereniging;
using When_Wijzig_Locatie;
using Xunit;

[CollectionDefinition(Name)]
public class RegistreerVerenigingAdminCollection2 : ICollectionFixture<FullBlownApiSetup>
{
    public const string Name = "registreerfeitelijkeverenigingadmin2";
}

[CollectionDefinition(Name)]
public class RegistreerVerenigingAdminCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingContext<AdminApiSetup>>
{
    public const string Name = "registreerfeitelijkeverenigingadmin";
}

[CollectionDefinition(Name)]
public class RegistreerVerenigingPublicCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingContext<PublicApiSetup>>
{
    public const string Name = "registreerfeitelijkeverenigingpublic";
}

[CollectionDefinition(Name)]
public class RegistreerVerenigingWithPotentialDuplicatesAdminCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext<AdminApiSetup>>
{
    public const string Name = "registreerfeitelijkeverenigingwithpotentialduplicatesadmin";
}

[CollectionDefinition(Name)]
public class RegistreerVerenigingWithPotentialDuplicatesPublicCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext<PublicApiSetup>>
{
    public const string Name =
        "registreerfeitelijkeverenigingwithpotentialduplicatespublic";
}

[CollectionDefinition(Name)]
public class StopVerenigingPublicCollection : ICollectionFixture<StopVerenigingContext<PublicApiSetup>>
{
    public const string Name = "stopverenigingpublic";
}

[CollectionDefinition(Name)]
public class WijzigLocatieAdminCollection : ICollectionFixture<WijzigLocatieContext<AdminApiSetup>>
{
    public const string Name = "wijziglocatieadmin";
}
