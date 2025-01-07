namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven.RequestValidating.ContactGegeven.Beschrijving;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_Valid_Beschrijving_Length : ValidatorTest
{
    [Fact]
    public void Has_validation_error()
    {
        var validator = new VoegContactgegevenToeValidator();

        var request = new VoegContactgegevenToeRequest
        {
            Contactgegeven = new ToeTeVoegenContactgegeven()
            {
                Beschrijving = new string('A', 128),
            },
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven.Beschrijving);
    }
}
