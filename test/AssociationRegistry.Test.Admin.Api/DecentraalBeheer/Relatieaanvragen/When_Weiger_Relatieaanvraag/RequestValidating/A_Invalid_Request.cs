namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Weiger_Relatieaanvraag.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.WeigerRelatieaanvraag.RequestModels;
using FluentValidation.TestHelper;
using Resources;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

/// <summary>The aanvraagId is checked by its value object: see To_WeigerRelatieaanvraagCommand.</summary>
public class A_Invalid_Request : ValidatorTest
{
    private readonly WeigerRelatieaanvraagRequestValidator _validator = new();

    [Fact]
    public void Has_validation_error_when_the_reden_contains_html()
    {
        var result = _validator.TestValidate(new WeigerRelatieaanvraagRequest { Reden = "<b>Geen lid</b>" });

        result
            .ShouldHaveValidationErrorFor(request => request.Reden)
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
