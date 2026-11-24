namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Bevestig_Relatieaanvraag.RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag.RequestModels;
using FluentValidation.TestHelper;
using Resources;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

/// <summary>The aanvraagId is checked by its value object: see To_BevestigRelatieaanvraagCommand.</summary>
public class A_Invalid_Request : ValidatorTest
{
    private readonly BevestigRelatieaanvraagRequestValidator _validator = new();

    [Fact]
    public void Has_validation_error_when_the_identificatie_contains_html()
    {
        var result = _validator.TestValidate(new BevestigRelatieaanvraagRequest { Identificatie = "<b>LID-1</b>" });

        result
            .ShouldHaveValidationErrorFor(request => request.Identificatie)
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }

    [Fact]
    public void Has_validation_error_when_the_beschrijving_contains_html()
    {
        var result = _validator.TestValidate(new BevestigRelatieaanvraagRequest { Beschrijving = "<p>Lid</p>" });

        result
            .ShouldHaveValidationErrorFor(request => request.Beschrijving)
            .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
