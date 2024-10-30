﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Lidmaatschap.RequestValidating;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class A_Invalid_Request : ValidatorTest
{
    private readonly Fixture _fixture;
    private readonly WijzigLidmaatschapRequestValidator _validator;

    public A_Invalid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _validator = new WijzigLidmaatschapRequestValidator();
    }

    [Fact]
    public void Has_validation_errors_for_lidmaatschapId_when_zero()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.LidmaatschapId = 0;

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.LidmaatschapId)
              .WithErrorMessage(ValidationMessages.VeldIsVerplicht);
    }

    [Fact]
    public void Has_validation_errors_when_tot_after_van()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Van = DateOnly.FromDateTime(DateTime.Now);
        request.Tot = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Tot)
              .WithErrorMessage(ValidationMessages.DatumTotMoetLaterZijnDanDatumVan);
    }

    [Fact]
    public void Has_no_validation_errors_when_tot_not_after_van()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Van = DateOnly.FromDateTime(DateTime.Now);
        request.Tot = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_validation_errors_when_html_detected_in_identificatie()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Identificatie = "<p>Something something</p>";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Identificatie)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_validation_errors_when_html_detected_in_beschrijving()
    {
        var request = _fixture.Create<WijzigLidmaatschapRequest>();
        request.Beschrijving = "<p>Something something</p>";

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Beschrijving)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
