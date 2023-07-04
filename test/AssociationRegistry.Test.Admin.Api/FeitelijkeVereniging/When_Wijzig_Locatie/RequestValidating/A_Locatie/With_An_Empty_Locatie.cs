namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_An_Empty_Locatie: ValidatorTest
{
    [Fact]
    public void Has_ValidationError_For_Locatie()
    {
        var validator = new WijzigLocatieRequestValidator();
        var request = new WijzigLocatieRequest { Locatie = new WijzigLocatieRequest.TeWijzigenLocatie() };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(locatieRequest => locatieRequest.Locatie)
            .WithErrorMessage("'Locatie' moet ingevuld zijn.");
    }
}
