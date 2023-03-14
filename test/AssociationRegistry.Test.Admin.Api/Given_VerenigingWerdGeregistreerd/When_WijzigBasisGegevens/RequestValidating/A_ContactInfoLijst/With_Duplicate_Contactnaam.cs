namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.RequestValidating.A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Duplicate_Contactnaam
{
    [Fact]
    public void Has_validation_error_for_ContactInfoLijst()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var request = new WijzigBasisgegevensRequest
        {
            ContactInfoLijst = new ContactInfo[]
            {
                new()
                {
                    Contactnaam = "Algemeen",
                },
                new()
                {
                    Contactnaam = "Algemeen",
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(v=>v.ContactInfoLijst)
            .WithErrorMessage("Een contactnaam moet uniek zijn.");
    }
}
