namespace AssociationRegistry.Test.Admin.Api.Framework;

using AutoFixture;
using Common.AutoFixture;
using System.ComponentModel;

[Category("Validator")]
public abstract class ValidatorTest
{
    public Fixture Fixture { get; init; }

    public ValidatorTest()
    {
        Fixture = new Fixture().CustomizeAdminApi();
    }
}
