﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_Wijzig_ContactgegevenFromKbo.CommandHandling;

using Acties.WijzigContactgegevenFromKbo;
using Acties.WijzigMaatschappelijkeZetel;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;
using Framework;
using Vereniging;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario _scenario;
    private readonly WijzigContactgegevenFromKboCommand _command;

    public Given_A_Contactgegeven()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var fixture = new Fixture().CustomizeAdminApi();

        _command = new WijzigContactgegevenFromKboCommand(_scenario.VCode,
                                                          new WijzigContactgegevenFromKboCommand.CommandContactgegeven(
                                                              _scenario.ContactgegevenWerdOvergenomenUitKBO.ContactgegevenId,
                                                              fixture.Create<string>(), fixture.Create<bool>()));

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigContactgegevenFromKboCommandHandler(_verenigingRepositoryMock);

        commandHandler.Handle(
            new CommandEnvelope<WijzigContactgegevenFromKboCommand>(_command, commandMetadata)).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded<VerenigingMetRechtspersoonlijkheid>(_scenario.VCode);
    }

    [Fact]
    public void Then_A_MaatschappelijkeZetelVolgensKBOWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactgegevenVolgensKBOWerdGewijzigd(_command.Contactgegeven.ContacgegevenId, _command.Contactgegeven.Beschrijving!,
                                                      _command.Contactgegeven.IsPrimair!.Value)
        );
    }
}
