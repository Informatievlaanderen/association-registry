namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Lidmaatschap.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
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
    private readonly WijzigLidmaatschapRequestValidator _validator;

    public A_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _validator = new WijzigLidmaatschapRequestValidator();
    }

    [Fact]
    public void Has_no_validation_errors()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_van_null()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Van = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_tot_null()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Tot = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_identificatie_null()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Identificatie = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_identificatie_empty()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Identificatie = "";

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_beschrijving_null()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Beschrijving = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_beschrijving_empty()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Beschrijving = "";

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
