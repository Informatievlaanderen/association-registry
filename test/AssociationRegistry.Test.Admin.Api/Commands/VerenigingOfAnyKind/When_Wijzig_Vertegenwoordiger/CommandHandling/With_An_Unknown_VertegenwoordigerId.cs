namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.Acties.WijzigVertegenwoordiger;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging.Emails;
using AssociationRegistry.Vereniging.Exceptions;
using AssociationRegistry.Vereniging.SocialMedias;
using AssociationRegistry.Vereniging.TelefoonNummers;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_VertegenwoordigerId
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields _scenario;
    private readonly WijzigVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_VertegenwoordigerId()
    {
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _fixture = new Fixture().CustomizeAdminApi();
        _commandHandler = new WijzigVertegenwoordigerCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async Task Then_A_UnknownVertegenoordigerException_Is_Thrown()
    {
        var command = new WijzigVertegenwoordigerCommand(
            _scenario.VCode,
            new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
                _fixture.Create<int>(),
                _fixture.Create<string?>(),
                _fixture.Create<string?>(),
                _fixture.Create<Email>(),
                _fixture.Create<TelefoonNummer>(),
                _fixture.Create<TelefoonNummer>(),
                _fixture.Create<SocialMedia>(),
                IsPrimair: false));

        var commandMetadata = _fixture.Create<CommandMetadata>();

        var handle = () => _commandHandler.Handle(new CommandEnvelope<WijzigVertegenwoordigerCommand>(command, commandMetadata));

        await handle.Should().ThrowAsync<VertegenwoordigerIsNietGekend>();
    }
}
