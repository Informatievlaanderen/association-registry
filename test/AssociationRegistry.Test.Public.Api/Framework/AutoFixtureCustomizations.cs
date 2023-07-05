namespace AssociationRegistry.Test.Public.Api.Framework;

using AutoFixture;
using Test.Framework.Customizations;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAll(this Fixture fixture)
    {
        fixture.CustomizeDomain();

        fixture.CustomizeTestEvent(typeof(TestEvent<>));

        return fixture;
    }
}
