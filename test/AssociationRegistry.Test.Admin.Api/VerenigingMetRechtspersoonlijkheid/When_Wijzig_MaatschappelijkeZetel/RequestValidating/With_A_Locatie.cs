namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Locatie : ValidatorTest
{
    [Fact]
    public void Uses_TeWijzigenMaatschappelijkeZetelValidator()
    {
        var validator = new WijzigMaatschappelijkeZetelRequestValidator();
        validator.ShouldHaveChildValidator(request => request.Locatie, typeof(TeWijzigenMaatschappelijkeZetelValidator));
    }
}
