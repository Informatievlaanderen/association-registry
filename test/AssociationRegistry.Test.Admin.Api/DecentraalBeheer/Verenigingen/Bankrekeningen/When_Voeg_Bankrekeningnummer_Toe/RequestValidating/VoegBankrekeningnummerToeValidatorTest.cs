namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using FluentValidation.TestHelper;
using Framework;
using Microsoft.AspNetCore.Http;
using Resources;
using Xunit;

public class VoegBankrekeningnummerToeValidatorTest : ValidatorTest
{
    [Fact]
    public void With_Bankrekeningnummer_Null_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = null,
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer)
              .WithErrorMessage("'Bankrekeningnummer' is verplicht.");
    }

    [Fact]
    public void With_IBAN_Empty_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = string.Empty,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Iban)
              .WithErrorMessage("'iban' mag niet leeg zijn.");
    }

    [Fact]
    public void With_IBAN_Null_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = null,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Iban)
              .WithErrorMessage("'iban' is verplicht.");
    }

    [Theory]
    [InlineData("DE68539007547034")]    // wrong country code
    [InlineData("be68539007547034")]    // lowercase country code
    [InlineData("BE6853900754703")]     // too short (13 digits)
    [InlineData("BE685390075470345")]   // too long (15 digits)
    [InlineData("BE000000000")]         // too short
    [InlineData("BE00000000000000")]    // checksum invalid (all zeros)
    [InlineData("BE12ABC456789012")]    // contains letters
    [InlineData("BE12-3456-7890-12")]   // invalid characters
    [InlineData("XBE68539007547034")]   // does not start with BE
    [InlineData("BE 68539007547034")]   // contains space
    public void With_IBAN_Format_Incorrect_Then_Has_ValidationError(string iban)
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = iban,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Iban)
              .WithErrorMessage("Het opgegeven 'IBAN' is geen geldig Belgisch IBAN.");
    }

    [Fact]
    public void With_Valid_Bankrekeningnummer_Then_Has_No_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = "BE68539007547034",
                Doel = "Lidgeld",
                Titularis = "Frodo Baggins",
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer);
    }

    [Fact]
    public void With_Titularis_Empty_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = "BE68539007547034",
                Titularis = string.Empty,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Titularis)
              .WithErrorMessage("'titularis' mag niet leeg zijn.");
    }

    [Fact]
    public void With_Titularis_Null_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = "BE68539007547034",
                Titularis = null,
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Titularis)
              .WithErrorMessage("'titularis' is verplicht.");
    }

    [Fact]
    public void With_Titularis_Exceeds_Max_Lenth_Then_Has_validation_error()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = "BE68539007547034",
                Titularis = new string('A', Bankrekeningnummer.MaxLengthTitularis + 1),
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer.Titularis)
              .WithErrorMessage("Titularis mag niet langer dan 128 karakters zijn.");
    }

    [Fact]
    public void With_Doel_Exceeds_Max_Lenth_Then_Has_validation_error()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = "BE68539007547034",
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
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = "BE68539007547034",
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
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer()
            {
                Iban = "BE68539007547034",
                Titularis = "<p>Something something</p>",
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularis)
              .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
