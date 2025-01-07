namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Lidmaatschap.RequestValidating;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Microsoft.AspNetCore.Http;
using Resources;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class A_Invalid_Request : ValidatorTest
{
    private readonly Fixture _fixture;
    private readonly VoegLidmaatschapToeRequestValidator _validator;

    public A_Invalid_Request()
    {
        _fixture = AdminApiAutoFixtureCustomizations.CustomizeAdminApi(new Fixture());
        _validator = new VoegLidmaatschapToeRequestValidator();
    }

    [Fact]
    public void Has_validation_errors_for_andereVereniging_when_empty()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.AndereVereniging = "";

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AndereVereniging)
              .WithVeldIsVerplichtErrorMessage(nameof(VoegLidmaatschapToeRequest.AndereVereniging));
    }

    [Fact]
    public void Has_validation_errors_for_andereVereniging_when_null()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.AndereVereniging = null;

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AndereVereniging)
              .WithVeldIsVerplichtErrorMessage(nameof(VoegLidmaatschapToeRequest.AndereVereniging));
    }

    [Fact]
    public void Has_validation_errors_when_tot_after_van()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Van = DateOnly.FromDateTime(DateTime.Now);
        request.Tot = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Tot)
              .WithErrorMessage(ValidationMessages.DatumTotMoetLaterZijnDanDatumVan);
    }

    [Fact]
    public void Has_no_validation_errors_when_tot_not_after_van()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Van = DateOnly.FromDateTime(DateTime.Now);
        request.Tot = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_validation_errors_when_html_detected_in_identificatie()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Identificatie = "<p>Something something</p>";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Identificatie)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_validation_errors_when_html_detected_in_beschrijving()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Beschrijving = "<p>Something something</p>";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Beschrijving)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
