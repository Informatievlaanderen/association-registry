namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestValidating.A_Bankrekeningnummer;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AutoFixture;
using Common.AutoFixture;
using Common.StubsMocksFakes.Clocks;
using FluentValidation.TestHelper;
using Xunit;

public class With_Duplicate_Iban
{
    [Fact]
    public void Has_validation_error_Iban_Moet_Uniek_Zijn()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(
            clock: new ClockStub(now: DateOnly.MaxValue)
        );

        var iban = fixture.Create<IbanNummer>().Value;

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Bankrekeningnummers =
            [
                new ToeTeVoegenBankrekeningnummer() { Iban = iban },
                new ToeTeVoegenBankrekeningnummer() { Iban = iban },
            ],
        };

        var result = validator.TestValidate(objectToTest: request);

        result
            .ShouldHaveValidationErrorFor(
                propertyName: nameof(RegistreerFeitelijkeVerenigingRequest.Bankrekeningnummers)
            )
            .WithErrorMessage(
                expectedErrorMessage: "Het IBAN van een bankrekening moet uniek zijn binnen de vereniging."
            );
    }
}
