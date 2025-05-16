namespace AssociationRegistry.Test.Acm.Api.Framework;

using AutoFixture;
using Common.AutoFixture;
using Test.Framework.Customizations;

public static class AutoFixtureCustomizations
{
    public static Fixture CustomizeAcmApi(this Fixture fixture)
    {
        fixture.CustomizeDomain();

        return fixture;
    }
}
