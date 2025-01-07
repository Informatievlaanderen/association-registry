namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_A_Null_Locatie : ValidatorTest
{
    [Fact]
    public void Has_ValidationError_For_Locatie()
    {
        var validator = new WijzigLocatieRequestValidator();
        var request = new WijzigLocatieRequest { Locatie = null! };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(locatieRequest => locatieRequest.Locatie)
              .WithErrorMessage("'Locatie' is verplicht.");
    }
}
