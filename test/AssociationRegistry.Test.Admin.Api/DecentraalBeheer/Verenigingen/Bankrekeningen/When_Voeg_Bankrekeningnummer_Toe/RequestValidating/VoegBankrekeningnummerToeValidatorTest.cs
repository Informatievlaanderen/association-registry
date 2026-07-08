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
    private const string ValidIban = "BE68539007547034";

    private static VoegBankrekeningnummerToeRequest CreateValidRequest() =>
        new()
        {
            Bankrekeningnummer = new ToeTeVoegenBankrekeningnummer
            {
                Iban = ValidIban,
                Doel = "Lidgeld",
                Titularissen = ["Frodo Baggins"],
            },
        };

    // ---------------------------------------------------------------
    // Bankrekeningnummer (root)
    // ---------------------------------------------------------------

    [Fact]
    public void With_Bankrekeningnummer_Null_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest { Bankrekeningnummer = null };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer)
            .WithErrorMessage("'Bankrekeningnummer' is verplicht.");
    }

    [Fact]
    public void With_Bankrekeningnummer_Null_Then_Has_No_Child_ValidationErrors()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = new VoegBankrekeningnummerToeRequest { Bankrekeningnummer = null };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.Bankrekeningnummer.Iban);
        result.ShouldNotHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen);
        result.ShouldNotHaveValidationErrorFor(r => r.Bankrekeningnummer.Doel);
    }

    // ---------------------------------------------------------------
    // Iban
    // ---------------------------------------------------------------

    [Fact]
    public void With_IBAN_Null_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Iban = null;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Iban).WithErrorMessage("'iban' is verplicht.");
    }

    [Fact]
    public void With_IBAN_Empty_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Iban = string.Empty;

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Iban)
            .WithErrorMessage("'iban' mag niet leeg zijn.");
    }

    [Theory]
    [InlineData("DE68539007547034")] // wrong country code
    [InlineData("be68539007547034")] // lowercase country code
    [InlineData("BE6853900754703")] // too short (13 digits)
    [InlineData("BE685390075470345")] // too long (15 digits)
    [InlineData("BE000000000")] // too short
    [InlineData("BE00000000000000")] // checksum invalid (all zeros)
    [InlineData("BE12ABC456789012")] // contains letters
    [InlineData("BE12-3456-7890-12")] // invalid characters
    [InlineData("XBE68539007547034")] // does not start with BE
    public void With_IBAN_Format_Incorrect_Then_Has_ValidationError(string iban)
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Iban = iban;

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Iban)
            .WithErrorMessage("Het opgegeven 'IBAN' is geen geldig Belgisch IBAN.");
    }

    [Theory]
    [InlineData("BE68539007547034")]
    [InlineData("BE68 5390 0754 7034")]
    [InlineData("BE68.5390.0754.7034")]
    public void With_Valid_IBAN_Then_Has_No_ValidationError(string iban)
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Iban = iban;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    // ---------------------------------------------------------------
    // Titularis (array-level)
    // ---------------------------------------------------------------

    [Fact]
    public void With_Titularis_Null_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = null;

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("'Titularissen' is verplicht.");
    }

    [Fact]
    public void With_Titularis_Null_Then_Has_Only_One_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = null;

        var result = validator.TestValidate(request);

        // Cascade(Stop): NotNull fails, NotEmpty must not also fire
        Assert.Single(result.Errors);
    }

    [Fact]
    public void With_Titularis_Empty_Array_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = [];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("'Titularissen' mag niet leeg zijn.");
    }

    // ---------------------------------------------------------------
    // Titularis (element-level)
    // ---------------------------------------------------------------

    [Fact]
    public void With_Titularis_Element_Empty_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = [string.Empty];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("'Titularissen' mag niet leeg zijn.");
    }

    [Fact]
    public void With_Second_Titularis_Element_Empty_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["voornaam achternaam", string.Empty];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("'Titularissen' mag niet leeg zijn.");
    }

    [Fact]
    public void With_Titularis_Element_Null_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = [null];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("'Titularissen' is verplicht.");
    }

    [Fact]
    public void With_Second_Titularis_Element_Null_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["voornaam achternaam", null];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("'Titularissen' is verplicht.");
    }

    [Fact]
    public void With_Titularis_Exceeds_Max_Length_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = [new string('A', Bankrekeningnummer.MaxLengthTitularis + 1)];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage($"Titularis mag niet langer dan {Bankrekeningnummer.MaxLengthTitularis} karakters zijn.");
    }

    [Fact]
    public void With_Multiple_Titularis_Exceed_Max_Length_Then_Has_ValidationErrors()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen =
        [
            new string('A', Bankrekeningnummer.MaxLengthTitularis + 1),
            new string('B', Bankrekeningnummer.MaxLengthTitularis + 1),
        ];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage($"Titularis mag niet langer dan {Bankrekeningnummer.MaxLengthTitularis} karakters zijn.");

        Assert.Single(result.Errors);
    }

    [Fact]
    public void With_Titularis_At_Max_Length_Then_Has_No_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = [new string('A', Bankrekeningnummer.MaxLengthTitularis)];

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_ValidationError_When_Html_Detected_In_Titularis()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["<p>Something something</p>"];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_ValidationError_When_Html_Detected_In_Second_Titularis()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["voornaam achternaam", "<p>Something something</p>"];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_ValidationErrors_When_Multiple_Html_Detected_In_Titularis()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["<p>Something</p>", "<p>Something something</p>"];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);

        Assert.Single(result.Errors);
    }

    [Fact]
    public void With_Multiple_Valid_Titularissen_Then_Has_No_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["Frodo Baggins", "Samwise Gamgee"];

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    // ---------------------------------------------------------------
    // Doel
    // ---------------------------------------------------------------

    [Fact]
    public void With_Doel_Null_Then_Has_No_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Doel = null;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void With_Doel_Empty_Then_Has_No_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Doel = string.Empty;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void With_Doel_Exceeds_Max_Length_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Doel = new string('A', Bankrekeningnummer.MaxLengthDoel + 1);

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Doel)
            .WithErrorMessage($"Doel mag niet langer dan {Bankrekeningnummer.MaxLengthDoel} karakters zijn.");
    }

    [Fact]
    public void With_Doel_At_Max_Length_Then_Has_No_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Doel = new string('A', Bankrekeningnummer.MaxLengthDoel);

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_ValidationError_When_Html_Detected_In_Doel()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Doel = "<p>Something something</p>";

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Doel)
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    // ---------------------------------------------------------------
    // Happy path
    // ---------------------------------------------------------------

    [Fact]
    public void With_Fully_Valid_Request_Then_Has_No_ValidationErrors()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var result = validator.TestValidate(CreateValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    // ---------------------------------------------------------------
    // Titularis (uniek)
    // ---------------------------------------------------------------

    [Fact]
    public void With_Duplicate_Titularis_Then_Has_ValidationError()
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["Frodo Baggins", "Frodo Baggins"];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage(ExceptionMessages.TitularissenMoetenUniekZijn);

        Assert.Single(result.Errors);
    }

    [Theory]
    [InlineData("frodo baggins")]
    [InlineData("FRODO BAGGINS")]
    [InlineData("Frodo BAGGINS")]
    public void With_Duplicate_Titularis_Different_Casing_Then_Has_ValidationError(string duplicate)
    {
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["Frodo Baggins", duplicate];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage(ExceptionMessages.TitularissenMoetenUniekZijn);
    }

    [Fact]
    public void With_Duplicate_And_Null_Titularis_Then_Has_Only_Element_ValidationError()
    {
        // null/lege elementen krijgen hun eigen error; de uniek-check mag dan niet ook nog vuren
        var validator = new VoegBankrekeningnummerToeValidator();

        var request = CreateValidRequest();
        request.Bankrekeningnummer.Titularissen = ["Frodo Baggins", "Frodo Baggins", null];

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("'Titularissen' is verplicht.");

        Assert.DoesNotContain(result.Errors, e => e.ErrorMessage == ExceptionMessages.TitularissenMoetenUniekZijn);
    }
}
