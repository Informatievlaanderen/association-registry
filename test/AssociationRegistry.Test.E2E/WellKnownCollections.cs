namespace AssociationRegistry.Test.E2E;

using Framework.ApiSetup;
using When_Markeer_Als_Dubbel_Van;
using Xunit;

[CollectionDefinition(Name)]
public class FullBlownApiCollection : ICollectionFixture<FullBlownApiSetup>
{
    public const string Name = "full blown api";
}

[CollectionDefinition(Name)]
public class MarkeerAlsDubbelVanContextCollection : ICollectionFixture<MarkeerAlsDubbelVanContext>
{
    public const string Name = nameof(MarkeerAlsDubbelVanContext);
}
