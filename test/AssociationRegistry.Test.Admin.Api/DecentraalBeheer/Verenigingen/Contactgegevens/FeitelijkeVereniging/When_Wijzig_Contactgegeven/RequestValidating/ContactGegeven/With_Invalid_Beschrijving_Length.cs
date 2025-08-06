namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class With_Invalid_Beschrijving_Length : ValidatorTest
{
    [Fact]
    public void Has_validation_error()
    {
        var validator = new WijzigContactgegevenValidator();

        var request = new WijzigContactgegevenRequest
        {
            Contactgegeven = new TeWijzigenContactgegeven()
            {
                Beschrijving = new string('A', 129),
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven.Beschrijving)
              .WithErrorMessage("Beschrijving mag niet langer dan 128 karakters zijn.");
    }
}
