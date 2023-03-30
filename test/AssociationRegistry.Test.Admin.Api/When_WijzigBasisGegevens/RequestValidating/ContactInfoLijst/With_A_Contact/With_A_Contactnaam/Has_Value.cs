namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.RequestValidating.ContactInfoLijst.With_A_Contact.With_A_Contactnaam;

using AssociationRegistry.Admin.Api.Verenigingen.CommonRequestDataTypes;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Has_Value
{
    [Fact]
    public void Has_validation_error()
    {
        var validator = new WijzigBasisgegevensRequestValidator();
        var request = new WijzigBasisgegevensRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            ContactInfoLijst = new []{new ContactInfo
            {
                Contactnaam = "Algemeen",
                Email = "info@something.be",
            }},
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(request.ContactInfoLijst)}[0]" +
                                            $".{nameof(ContactInfo.Contactnaam)}");
    }
}
