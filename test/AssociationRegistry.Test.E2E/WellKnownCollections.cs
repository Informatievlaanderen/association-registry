namespace AssociationRegistry.Test.E2E;

using Framework.TestClasses;
using When_Corrigeer_Markeer_Als_Dubbel_Van;
using When_Markeer_Als_Dubbel_Van;
using When_Wijzig_Locatie;
using When_Verwijder_Lidmaatschap;
using When_Registreer_FeitelijkeVereniging_With_Duplicates_With_Gemeentenaam_In_Verenigingsnaam;
using When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;
using When_Registreer_FeitelijkeVereniging;
using When_Registreer_VerenigingMetRechtsperoonslijkheid;
using When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;
using When_Stop_Vereniging;
using When_Voeg_Lidmaatschap_Toe;
using When_Wijzig_Basisgegevens_Kbo;
using When_Wijzig_Basisgegevens;
using When_Wijzig_Lidmaatschap;
using Xunit;

[CollectionDefinition(WellKnownCollections.MarkeerAlsDubbelVan)]
public class MarkeerAlsDubbelVanCollection : ICollectionFixture<MarkeerAlsDubbelVanContext>
{
}

[CollectionDefinition(WellKnownCollections.CorrigeerMarkeringAlsDubbelVan)]
public class CorrigeerMarkeringAlsDubbelVanCollection : ICollectionFixture<CorrigeerMarkeringAlsDubbelVanContext>
{
}

[CollectionDefinition(WellKnownCollections.VoegLidmaatschapToe)]
public class VoegLidmaatschapToeCollection : ICollectionFixture<VoegLidmaatschapToeContext>
{
}

[CollectionDefinition(WellKnownCollections.WijzigLocatie)]
public class WijzigLocatieCollection : ICollectionFixture<WijzigLocatieContext>
{
}

[CollectionDefinition(WellKnownCollections.WijzigLidmaatschap)]
public class WijzigLidmaatschapCollection : ICollectionFixture<WijzigLidmaatschapContext>
{
}

[CollectionDefinition(WellKnownCollections.VerwijderLidmaatschap)]
public class VerwijderLidmaatschapCollection : ICollectionFixture<VerwijderLidmaatschapContext>
{
}

[CollectionDefinition(WellKnownCollections.WijzigBasisgegevens)]
public class WijzigBasisgegevensCollection : ICollectionFixture<WijzigBasisgegevensTestContext>
{
}

[CollectionDefinition(WellKnownCollections.WijzigBasisgegevensKbo)]
public class WijzigBasisgegevensKboCollection : ICollectionFixture<WijzigBasisgegevensKboTestContext>
{
}

[CollectionDefinition(WellKnownCollections.RegistreerFeitelijkeVereniging)]
public class RegistreerFeitelijkeVerenigingCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingTestContext>
{
}

[CollectionDefinition(WellKnownCollections.RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaam)]
public class RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext>
{
}

[CollectionDefinition(WellKnownCollections.RegistreerFeitelijkeVerenigingWithPotentialDuplicates)]
public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesCollection : ICollectionFixture<RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext>
{
}

[CollectionDefinition(WellKnownCollections.RegistreerVerenigingMetRechtsperoonlijkheid)]
public class RegistreerVerenigingMetRechtsperoonlijkheidCollection : ICollectionFixture<RegistreerVerenigingMetRechtsperoonlijkheidTestContext>
{
}

[CollectionDefinition(WellKnownCollections.RegistreerVerenigingZonderEigenRechtspersoonlijkheid)]
public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection : ICollectionFixture<RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext>
{
}

[CollectionDefinition(WellKnownCollections.StopVereniging)]
public class StopVerenigingCollection : ICollectionFixture<StopVerenigingContext>
{
}

[CollectionDefinition(WellKnownCollections.GenericSharedContext)]
public class GenericSharedContextCollection : ICollectionFixture<GenericSharedContext>
{
}

public static class WellKnownCollections
{
    public const string MarkeerAlsDubbelVan = nameof(MarkeerAlsDubbelVanCollection);
    public const string CorrigeerMarkeringAlsDubbelVan = nameof(CorrigeerMarkeringAlsDubbelVanCollection);
    public const string VoegLidmaatschapToe = nameof(VoegLidmaatschapToeCollection);
    public const string WijzigLocatie = nameof(WijzigLocatieCollection);
    public const string WijzigLidmaatschap = nameof(WijzigLidmaatschapCollection);
    public const string VerwijderLidmaatschap = nameof(VerwijderLidmaatschapCollection);
    public const string WijzigBasisgegevens = nameof(WijzigBasisgegevensCollection);
    public const string WijzigBasisgegevensKbo = nameof(WijzigBasisgegevensKboCollection);
    public const string RegistreerFeitelijkeVereniging = nameof(RegistreerFeitelijkeVerenigingCollection);
    public const string RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaam = nameof(RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamCollection);
    public const string RegistreerFeitelijkeVerenigingWithPotentialDuplicates = nameof(RegistreerFeitelijkeVerenigingWithPotentialDuplicatesCollection);
    public const string RegistreerVerenigingMetRechtsperoonlijkheid = nameof(RegistreerVerenigingMetRechtsperoonlijkheidCollection);
    public const string RegistreerVerenigingZonderEigenRechtspersoonlijkheid = nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection);
    public const string StopVereniging = nameof(StopVerenigingCollection);
    public const string GenericSharedContext = nameof(GenericSharedContextCollection);
}
