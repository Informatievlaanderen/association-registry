namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_An_Empty_Locatie : ValidatorTest
{
    [Fact]
    public void Has_ValidationError_For_Locatie()
    {
        var validator = new WijzigLocatieRequestValidator();
        var request = new WijzigLocatieRequest { Locatie = new TeWijzigenLocatie() };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(locatieRequest => locatieRequest.Locatie)
              .WithErrorMessage("'Locatie' moet ingevuld zijn.");
    }
}
