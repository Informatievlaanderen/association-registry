namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.
    RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
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
        validator.ShouldHaveChildValidator(expression: request => request.Locatie, typeof(TeWijzigenMaatschappelijkeZetelValidator));
    }
}
