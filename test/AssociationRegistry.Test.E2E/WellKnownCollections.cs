namespace AssociationRegistry.Test.E2E;

using Framework.ApiSetup;
using Xunit;

[CollectionDefinition(Name)]
public class FullBlownApiCollection : ICollectionFixture<FullBlownApiSetup>
{
    public const string Name = "full blown api";
}
