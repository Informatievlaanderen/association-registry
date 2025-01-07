namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestValidating
    .
    A_Locatie;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_An_Empty_Locatie : ValidatorTest
{
    [Fact]
    public void Has_ValidationError_For_Locatie()
    {
        var validator = new WijzigMaatschappelijkeZetelRequestValidator();

        var request = new WijzigMaatschappelijkeZetelRequest
            { Locatie = new TeWijzigenMaatschappelijkeZetel() };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(locatieRequest => locatieRequest.Locatie)
              .WithErrorMessage("'Locatie' moet ingevuld zijn.");
    }
}
