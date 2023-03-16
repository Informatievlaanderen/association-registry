namespace AssociationRegistry.Test.Admin.Api.When_WijzigBasisGegevens.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Fakes;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging.CommonCommandDataTypes;
using AssociationRegistry.Vereniging.WijzigBasisgegevens;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_New_ContactInfo
{
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly ContactInfo[] _contactInfoLijst;
    private readonly VerenigingWerdGeregistreerd_Commandhandler_Scenario _scenario;

    public With_A_New_ContactInfo()
    {
        var scenarioFixture = new CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_Commandhandler_Scenario>();
        _scenario = scenarioFixture.Scenario;

        _verenigingRepositoryMock = scenarioFixture.VerenigingRepositoryMock;

        var fixture = new Fixture().CustomizeAll();
        _contactInfoLijst = new[] { fixture.Create<ContactInfo>() };
        var command = new WijzigBasisgegevensCommand(_scenario.VCode, ContactInfoLijst: _contactInfoLijst);
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new WijzigBasisgegevensCommandHandler(new ClockStub(new DateTime(2023, 3, 13)));

        commandHandler.Handle(
            new CommandEnvelope<WijzigBasisgegevensCommand>(command, commandMetadata),
            _verenigingRepositoryMock).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_The_Correct_Vereniging_Is_Loaded_Once()
    {
        _verenigingRepositoryMock.ShouldHaveLoaded(_scenario.VCode);
    }

    [Fact]
    public void Then_A_ContactInfoLijstWerdGewijzigd_Event_Is_Saved()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new ContactInfoLijstWerdGewijzigd(
                _scenario.VCode,
                new[]
                {
                    new AssociationRegistry.Events.CommonEventDataTypes.ContactInfo(
                        _contactInfoLijst[0].Contactnaam,
                        _contactInfoLijst[0].Email,
                        _contactInfoLijst[0].Telefoon,
                        _contactInfoLijst[0].Website,
                        _contactInfoLijst[0].SocialMedia,
                        _contactInfoLijst[0].PrimairContactInfo),
                },
                Array.Empty<Events.CommonEventDataTypes.ContactInfo>(),
                Array.Empty<Events.CommonEventDataTypes.ContactInfo>()
            ));
    }
}
