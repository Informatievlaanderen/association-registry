namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentValidation.TestHelper;
using Framework;
using Microsoft.AspNetCore.Http;
using Resources;
using Xunit;
using TeWijzigenBankrekeningnummer = AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer.RequestModels.TeWijzigenBankrekeningnummer;

public class WijzigBankrekeningnummerValidatorTest : ValidatorTest
{
    [Fact]
    public void With_Bankrekeningnummer_Null_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = null,
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer)
              .WithErrorMessage("'Bankrekeningnummer' is verplicht.");
    }

    [Fact]
    public void With_Titularis_Null_And_Doel_Null_Then_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
                Doel = null,
                Titularis = null,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer)
              .WithErrorMessage("Doel of Titularis moet ingevuld zijn.");
    }

    [Fact]
    public void With_Titularis_Empty_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
                Titularis = string.Empty,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer)
              .WithErrorMessage("Doel of Titularis moet ingevuld zijn.");
    }

    [Fact]
    public void With_Doel_Empty_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
                Doel = string.Empty,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer)
              .WithErrorMessage("Doel of Titularis moet ingevuld zijn.");
    }

    [Fact]
    public void With_Titularis_Null_Then_No_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
                Doel = "Lidgeld",
                Titularis = null,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Titularis);
    }

    [Fact]
    public void With_Titularis_Exceeds_Max_Length_Then_Has_validation_error()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
                Doel = "Lidgeld",
                Titularis = new string('A', Bankrekeningnummer.MaxLengthTitularis + 1),
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Titularis)
              .WithErrorMessage("Titularis mag niet langer dan 128 karakters zijn.");
    }

    [Fact]
    public void With_Doel_Exceeds_Max_Length_Then_Has_validation_error()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
                Titularis = "Frodo",
                Doel = new string('A', Bankrekeningnummer.MaxLengthDoel + 1),
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Doel)
              .WithErrorMessage("Doel mag niet langer dan 128 karakters zijn.");
    }

    [Fact]
    public void Has_Validation_Errors_When_Html_Detected_In_Doel()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
                Doel = "<p>Something something</p>",
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Doel)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_Validation_Errors_When_Html_Detected_In_Titularis()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest()
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer()
            {
                Titularis = "<p>Something something</p>",
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularis)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
