namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Removing_Lidmaatschap.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VerwijderLidmaatschap.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class A_Valid_Request : ValidatorTest
{
    private readonly Fixture _fixture;
    private readonly VerwijderLidmaatschapRequestValidator _validator;

    public A_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _validator = new VerwijderLidmaatschapRequestValidator();
    }

    [Fact]
    public void Has_no_validation_errors()
    {
        var request = _fixture.Create<VerwijderLidmaatschapRequest>();
        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
