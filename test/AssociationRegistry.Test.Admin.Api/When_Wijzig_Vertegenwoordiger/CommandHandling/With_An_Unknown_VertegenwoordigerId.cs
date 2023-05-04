namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger.CommandHandling;

using Acties.WijzigVertegenwoordiger;
using Acties.VerwijderVertegenwoordiger;
using AssociationRegistry.Framework;
using Fakes;
using Fixtures.Scenarios;
using Framework;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Unknown_VertegenwoordigerId
{
    private readonly VerenigingWerdGeregistreerdWithoutVertegenwoordigers _scenario;
    private readonly WijzigVertegenwoordigerCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public With_An_Unknown_VertegenwoordigerId()
    {
        _scenario = new VerenigingWerdGeregistreerdWithoutVertegenwoordigers();

        var verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVereniging());

        _fixture = new Fixture().CustomizeAll();
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

        await handle.Should().ThrowAsync<UnknownVertegenwoordiger>();
    }
}
