namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Maak_Subvereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Subvereniging.MaakSubvereniging.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class A_Valid_Request : ValidatorTest
{
    private readonly Fixture _fixture;
    private readonly MaakSubverenigingRequestValidator _validator;

    public A_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _validator = new MaakSubverenigingRequestValidator();
    }

    [Fact]
    public void Has_no_validation_errors()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_identificatie_null()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        request.Identificatie = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_identificatie_empty()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        request.Identificatie = "";

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_beschrijving_null()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        request.Beschrijving = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_beschrijving_empty()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        request.Beschrijving = "";

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
