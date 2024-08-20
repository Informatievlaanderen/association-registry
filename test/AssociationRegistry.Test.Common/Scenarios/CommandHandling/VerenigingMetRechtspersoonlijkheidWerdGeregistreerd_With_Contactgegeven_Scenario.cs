namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Kbo;
using AssociationRegistry.Vereniging;
using AutoFixture;
using global::AutoFixture;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public KboNummer KboNummer => KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer);
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKBO;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Contactgegeven_Scenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        ContactgegevenWerdOvergenomenUitKBO = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>();
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            ContactgegevenWerdOvergenomenUitKBO,
        };

    public VerenigingVolgensKbo VerenigingVolgensKbo
        => new()
        {
            Naam = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
            KboNummer = KboNummer.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer),
            Startdatum = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum,
            Type = Verenigingstype.Parse(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm),
            KorteNaam = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam,
            Adres = new AdresVolgensKbo(),
            Contactgegevens = ContactgegevensVolgensKbo,
            IsActief = true,
            EindDatum = null,
            Vertegenwoordigers = Array.Empty<VertegenwoordigerVolgensKbo>(),
        };

    public ContactgegevensVolgensKbo ContactgegevensVolgensKbo
        => new()
        {
            Email = ContactgegevenWerdOvergenomenUitKBO.TypeVolgensKbo == ContactgegeventypeVolgensKbo.Email
                ? ContactgegevenWerdOvergenomenUitKBO.Waarde
                : null,
            Telefoonnummer = ContactgegevenWerdOvergenomenUitKBO.TypeVolgensKbo == ContactgegeventypeVolgensKbo.Telefoon
                ? ContactgegevenWerdOvergenomenUitKBO.Waarde
                : null,
            Website = ContactgegevenWerdOvergenomenUitKBO.TypeVolgensKbo == ContactgegeventypeVolgensKbo.Website
                ? ContactgegevenWerdOvergenomenUitKBO.Waarde
                : null,
            GSM = ContactgegevenWerdOvergenomenUitKBO.TypeVolgensKbo == ContactgegeventypeVolgensKbo.GSM
                ? ContactgegevenWerdOvergenomenUitKBO.Waarde
                : null,
        };
}
