namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Maak_Subvereniging.RequestValidating;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.Subvereniging.MaakSubvereniging.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Resources;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class A_Invalid_Request : ValidatorTest
{
    private readonly Fixture _fixture;
    private readonly MaakSubverenigingRequestValidator _validator;

    public A_Invalid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _validator = new MaakSubverenigingRequestValidator();
    }

    [Fact]
    public void Has_validation_errors_for_andereVereniging_when_empty()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        request.AndereVereniging = "";

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AndereVereniging)
              .WithErrorMessage(ValidationMessages.VeldIsVerplicht);
    }

    [Fact]
    public void Has_validation_errors_for_andereVereniging_when_null()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        request.AndereVereniging = null;

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AndereVereniging)
              .WithErrorMessage(ValidationMessages.VeldIsVerplicht);
    }

    [Fact]
    public void Has_validation_errors_when_html_detected_in_identificatie()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        request.Identificatie = "<p>Something something</p>";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Identificatie)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_validation_errors_when_html_detected_in_beschrijving()
    {
        var request = _fixture.Create<MaakSubverenigingRequest>();
        request.Beschrijving = "<p>Something something</p>";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Beschrijving)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
