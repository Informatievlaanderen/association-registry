namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_Request
{
    [Fact]
    public void Then_it_should_have_errors()
    {
        var validator = new WijzigMaatschappelijkeZetelRequestValidator();
        var result = validator.TestValidate(new WijzigMaatschappelijkeZetelRequest
                                                { Naam = null, IsPrimair = null});

        result.ShouldHaveValidationErrorFor("request")
              .WithErrorMessage("Een request mag niet leeg zijn.");
    }
}
