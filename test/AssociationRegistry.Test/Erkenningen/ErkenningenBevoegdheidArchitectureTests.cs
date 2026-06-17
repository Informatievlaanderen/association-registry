namespace AssociationRegistry.Test.Erkenningen;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerSchorsingErkenning;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.HefSchorsingErkenningOp;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerwijderErkenning;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using FluentAssertions;
using Xunit;

public class ErkenningenBevoegdheidArchitectureTests
{
    [Fact]
    public void Manual_Erkenning_CommandHandlers_Should_Use_OrganisatieBevoegdheidService()
    {
        Type[] handlers =
        [
            typeof(SchorsErkenningCommandHandler),
            typeof(HefSchorsingErkenningOpCommandHandler),
            typeof(CorrigeerRedenSchorsingErkenningCommandHandler),
            typeof(WijzigErkenningCommandHandler),
            typeof(VerwijderErkenningCommandHandler),
        ];

        var handlersWithoutBevoegdheidService = handlers
            .Where(handler =>
                !handler
                    .GetConstructors()
                    .SelectMany(constructor => constructor.GetParameters())
                    .Any(parameter => parameter.ParameterType.Name == "IOrganisatieBevoegdheidService")
            )
            .Select(handler => handler.Name);

        handlersWithoutBevoegdheidService.Should().BeEmpty();
    }
}
