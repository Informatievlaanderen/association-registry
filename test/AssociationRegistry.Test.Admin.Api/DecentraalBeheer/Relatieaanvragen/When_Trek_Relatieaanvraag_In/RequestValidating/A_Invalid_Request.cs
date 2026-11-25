namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Trek_Relatieaanvraag_In.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn.RequestModels;
using FluentValidation.TestHelper;
using Resources;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

/// <summary>The aanvraagId is checked by its value object: see To_TrekRelatieaanvraagInCommand.</summary>
public class A_Invalid_Request : ValidatorTest
{
    private readonly TrekRelatieaanvraagInRequestValidator _validator = new();

    [Fact]
    public void Has_validation_error_when_the_reden_contains_html()
    {
        var result = _validator.TestValidate(new TrekRelatieaanvraagInRequest { Reden = "<b>Vergissing</b>" });

        result
            .ShouldHaveValidationErrorFor(request => request.Reden)
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
