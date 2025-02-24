namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_Valid_Beschrijving_Length : ValidatorTest
{
    [Fact]
    public void Has_validation_error()
    {
        var validator = new WijzigContactgegevenValidator();

        var request = new WijzigContactgegevenRequest
        {
            Contactgegeven = new TeWijzigenContactgegeven()
            {
                Beschrijving = new string('A', 128),
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven.Beschrijving);
    }
}
