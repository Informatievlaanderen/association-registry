namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestValidating.
    A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_A_Null_Locatie : ValidatorTest
{
    [Fact]
    public void Has_ValidationError_For_Locatie()
    {
        var validator = new WijzigMaatschappelijkeZetelRequestValidator();

        var request = new WijzigMaatschappelijkeZetelRequest
            { Locatie = null! };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(locatieRequest => locatieRequest.Locatie)
              .WithErrorMessage("'Locatie' is verplicht.");
    }
}
