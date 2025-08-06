namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo.
    RequestValidating;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven.RequestModels;
using FluentValidation.TestHelper;
using Xunit;

public class Given_An_Empty_Request
{
    [Fact]
    public void Then_it_should_have_errors()
    {
        var validator = new WijzigContactgegevenValidator();

        var result = validator.TestValidate(new WijzigContactgegevenRequest { Contactgegeven = new TeWijzigenContactgegeven() });

        result.ShouldHaveValidationErrorFor(nameof(WijzigContactgegevenRequest.Contactgegeven))
              .WithErrorMessage("'Contactgegeven' moet ingevuld zijn.");
    }
}
