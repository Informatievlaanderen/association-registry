namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Voeg_Bankrekeningnummer_Toe.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using FluentValidation.TestHelper;
using Framework;
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
              .WithErrorMessage("'IBAN' mag niet leeg zijn.");
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
              .WithErrorMessage("'IBAN' is verplicht.");
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
                GebruiktVoor = "Lidgeld",
                Titularis = "Frodo Baggins",
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(toeRequest => toeRequest.Bankrekeningnummer);
    }
}
