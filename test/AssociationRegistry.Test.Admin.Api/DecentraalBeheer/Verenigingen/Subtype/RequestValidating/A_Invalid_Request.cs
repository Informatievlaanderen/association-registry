namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Subtype;
using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Microsoft.AspNetCore.Http;
using Resources;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class A_Invalid_Request : ValidatorTest
{
    private readonly Fixture _fixture;
    private readonly WijzigSubtypeRequestValidator _requestValidator;

    public A_Invalid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _requestValidator = new WijzigSubtypeRequestValidator();
    }

    [Fact]
    public void Has_validation_errors_when_no_changes()
    {
        var request = new WijzigSubtypeRequest();

        var result = _requestValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Subtype)
              .WithVeldIsVerplichtErrorMessage(nameof(WijzigSubtypeRequest.Subtype));
    }

    [Fact]
    public void Has_validation_errors_when_Invalid_Subtype()
    {
        var request = new WijzigSubtypeRequest()
        {
            Subtype = "invalid",
        };

        var result = _requestValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Subtype)
              .WithErrorMessage("Het subtype moet een geldige waarde hebben.");
    }

    [Fact]
    public void Has_validation_errors_when_html_detected_in_identificatie()
    {
        var request = _fixture.Create<WijzigSubtypeRequest>();
        request.Identificatie = "<p>Something something</p>";

        var result = _requestValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Identificatie)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_validation_errors_when_html_detected_in_beschrijving()
    {
        var request = _fixture.Create<WijzigSubtypeRequest>();
        request.Beschrijving = "<p>Something something</p>";

        var result = _requestValidator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Beschrijving)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
