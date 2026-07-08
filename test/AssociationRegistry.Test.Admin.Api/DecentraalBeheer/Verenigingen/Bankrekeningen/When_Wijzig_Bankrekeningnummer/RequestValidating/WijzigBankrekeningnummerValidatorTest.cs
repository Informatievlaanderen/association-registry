namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Bankrekeningen.When_Wijzig_Bankrekeningnummer.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer;
using FluentValidation.TestHelper;
using Framework;
using Microsoft.AspNetCore.Http;
using Resources;
using Xunit;
using Bankrekeningnummer = AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Bankrekeningnummer;
using TeWijzigenBankrekeningnummer = AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer.RequestModels.TeWijzigenBankrekeningnummer;
using WijzigBankrekeningnummerRequest = AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer.RequestModels.WijzigBankrekeningnummerRequest;

public class WijzigBankrekeningnummerValidatorTest : ValidatorTest
{
    // ---------------------------------------------------------------
    // Bankrekeningnummer (root)
    // ---------------------------------------------------------------

    [Fact]
    public void With_Bankrekeningnummer_Null_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest { Bankrekeningnummer = null };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer)
            .WithErrorMessage("'Bankrekeningnummer' is verplicht.");
    }

    // ---------------------------------------------------------------
    // Geen enkele wijziging opgegeven
    // ---------------------------------------------------------------

    [Fact]
    public void With_Doel_And_Titularis_Null_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Doel = null, Titularissen = null },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer)
            .WithErrorMessage("Doel of Titularissen moet ingevuld zijn.");
    }

    [Fact]
    public void With_Doel_Null_And_Titularis_Empty_Array_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Doel = null, Titularissen = [] },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer)
            .WithErrorMessage("Doel of Titularissen moet ingevuld zijn.");

        Assert.Single(result.Errors);
    }

    [Fact]
    public void With_Doel_Provided_And_Titularis_Empty_Array_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Doel = "Lidgeld", Titularissen = [] },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("'Titularissen' mag niet leeg zijn.");

        Assert.Single(result.Errors);
    }

    // ---------------------------------------------------------------
    // Patch: enkel Doel of enkel Titularis is geldig
    // ---------------------------------------------------------------

    [Fact]
    public void With_Only_Doel_Provided_Then_Has_No_ValidationErrors()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Doel = "Lidgeld", Titularissen = null },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void With_Only_Doel_Empty_String_Then_Has_No_ValidationErrors()
    {
        // Doel = "" is een geldige patch: het doel wordt leeggemaakt
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Doel = string.Empty, Titularissen = null },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void With_Only_Titularis_Provided_Then_Has_No_ValidationErrors()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Doel = null, Titularissen = ["Frodo Baggins"] },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void With_Doel_And_Titularis_Provided_Then_Has_No_ValidationErrors()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer
            {
                Doel = "Lidgeld",
                Titularissen = ["Frodo Baggins", "Samwise Gamgee"],
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    // ---------------------------------------------------------------
    // Titularis (element-level)
    // ---------------------------------------------------------------

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void With_Titularis_Element_NullOrWhiteSpace_Then_Has_ValidationError(string? titularis)
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Titularissen = [titularis] },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("Titularissen mag niet leeg zijn.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void With_Second_Titularis_Element_NullOrWhiteSpace_Then_Has_ValidationError(string? titularis)
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Titularissen = ["voornaam achternaam", titularis] },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("Titularissen mag niet leeg zijn.");
    }

    [Fact]
    public void With_Titularis_Exceeds_Max_Length_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer
            {
                Titularissen = [new string('A', Bankrekeningnummer.MaxLengthTitularis + 1)],
            },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage($"Titularis mag niet langer dan {Bankrekeningnummer.MaxLengthTitularis} karakters zijn.");
    }

    [Fact]
    public void With_Titularis_At_Max_Length_Then_Has_No_ValidationErrors()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer
            {
                Titularissen = [new string('A', Bankrekeningnummer.MaxLengthTitularis)],
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_ValidationError_When_Html_Detected_In_Titularis()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Titularissen = ["<p>Something something</p>"] },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_One_ValidationError_When_Multiple_Html_Detected_In_Titularis()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer
            {
                Titularissen = ["<p>Something</p>", "<p>Something something</p>"],
            },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);

        Assert.Single(result.Errors);
    }

    // ---------------------------------------------------------------
    // Doel
    // ---------------------------------------------------------------

    [Fact]
    public void With_Doel_Exceeds_Max_Length_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer
            {
                Doel = new string('A', Bankrekeningnummer.MaxLengthDoel + 1),
            },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Doel)
            .WithErrorMessage($"Doel mag niet langer dan {Bankrekeningnummer.MaxLengthDoel} karakters zijn.");
    }

    [Fact]
    public void With_Doel_At_Max_Length_Then_Has_No_ValidationErrors()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer
            {
                Doel = new string('A', Bankrekeningnummer.MaxLengthDoel),
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_ValidationError_When_Html_Detected_In_Doel()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Doel = "<p>Something something</p>" },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Doel)
            .WithErrorCode(StatusCodes.Status400BadRequest.ToString())
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    // ---------------------------------------------------------------
    // Titularis (uniek)
    // ---------------------------------------------------------------

    [Fact]
    public void With_Duplicate_Titularis_Then_Has_ValidationError()
    {
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Titularissen = ["Frodo Baggins", "Frodo Baggins"] },
        };

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
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer { Titularissen = ["Frodo Baggins", duplicate] },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage(ExceptionMessages.TitularissenMoetenUniekZijn);
    }

    [Fact]
    public void With_Duplicate_And_NullOrWhiteSpace_Titularis_Then_Has_Only_Element_ValidationError()
    {
        // null/lege elementen krijgen hun eigen error; de uniek-check mag dan niet ook nog vuren
        var validator = new WijzigBankrekeningnummerValidator();

        var request = new WijzigBankrekeningnummerRequest
        {
            Bankrekeningnummer = new TeWijzigenBankrekeningnummer
            {
                Titularissen = ["Frodo Baggins", "Frodo Baggins", null],
            },
        };

        var result = validator.TestValidate(request);

        result
            .ShouldHaveValidationErrorFor(r => r.Bankrekeningnummer.Titularissen)
            .WithErrorMessage("Titularissen mag niet leeg zijn.");

        Assert.DoesNotContain(result.Errors, e => e.ErrorMessage == ExceptionMessages.TitularissenMoetenUniekZijn);
    }
}
