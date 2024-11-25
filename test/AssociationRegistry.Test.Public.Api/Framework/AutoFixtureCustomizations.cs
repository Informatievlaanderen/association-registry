namespace AssociationRegistry.Test.Public.Api.Framework;

using AutoFixture;
using Common.AutoFixture;
using Test.Framework.Customizations;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizePublicApi(this Fixture fixture)
    {
        fixture.CustomizeDomain();

        fixture.CustomizeTestEvent(typeof(TestEvent<>));

        return fixture;
    }
}
