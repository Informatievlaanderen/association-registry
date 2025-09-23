namespace AssociationRegistry.Test.Framework.Customizations;

using Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Contactgegevens.VoegContactgegevenToe;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using Vereniging;

public static class CommandCustomizations
{
    public static void CustomizeCommands(Fixture fixture)
    {
        fixture.CustomizeRegistreerFeitelijkeVerenigingCommand();
        fixture.CustomizeVoegContactgegevenToeCommand();
    }

    private static void CustomizeRegistreerFeitelijkeVerenigingCommand(this IFixture fixture)
    {
        fixture.Customize<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () =>
                                                             {
                                                                 var request =
                                                                     fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();

                                                                 return request.ToCommand(
                                                                     fixture.CreateMany<Werkingsgebied>().Distinct().ToArray());
                                                             })
                                                        .OmitAutoProperties());
    }

    private static void CustomizeVoegContactgegevenToeCommand(this IFixture fixture)
    {
        fixture.Customize<VoegContactgegevenToeCommand>(
            composerTransformation: composer => composer.FromFactory(
                                                             factory: () => new VoegContactgegevenToeCommand(
                                                                 VCode: fixture.Create<VCode>(),
                                                                 Contactgegeven: fixture.Create<Contactgegeven>())
                                                         )
                                                        .OmitAutoProperties());
    }
}
