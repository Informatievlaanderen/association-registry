namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven.RequestValidating.ContactGegeven.Type;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new VoegContactgegevenToeValidator();

        var result = validator.TestValidate(
            new VoegContactgegevenToeRequest
            {
                Contactgegeven = new ToeTeVoegenContactgegeven
                {
                    Contactgegeventype = Contactgegeventype.Email,
                },
            });

        result.ShouldNotHaveValidationErrorFor(nameof(VoegContactgegevenToeRequest.Contactgegeven) + "." +
                                               nameof(ToeTeVoegenContactgegeven.Contactgegeventype));
    }
}
