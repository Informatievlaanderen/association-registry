namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.When_Adding_Lidmaatschap.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class A_Valid_Request : ValidatorTest
{
    private readonly Fixture _fixture;
    private readonly VoegLidmaatschapToeRequestValidator _validator;

    public A_Valid_Request()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _validator = new VoegLidmaatschapToeRequestValidator();
    }

    [Fact]
    public void Has_no_validation_errors()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_van_null()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Van = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_tot_null()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Tot = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_identificatie_null()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Identificatie = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_identificatie_empty()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Identificatie = "";

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_beschrijving_null()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Beschrijving = null;

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_no_validation_errors_when_beschrijving_empty()
    {
        var request = _fixture.Create<VoegLidmaatschapToeRequest>();
        request.Beschrijving = "";

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
