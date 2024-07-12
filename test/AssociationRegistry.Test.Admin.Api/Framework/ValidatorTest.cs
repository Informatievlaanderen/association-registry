namespace AssociationRegistry.Test.Admin.Api.Framework;

using AutoFixture;
using Xunit.Categories;

[UnitTest]
[Category("Validator")]
public abstract class ValidatorTest
{
    public Fixture Fixture { get; init; }

    public ValidatorTest()
    {
        Fixture = new Fixture().CustomizeAdminApi();
    }
}
