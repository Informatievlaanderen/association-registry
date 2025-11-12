namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Admin.Schema.Persoonsgegevens;
using DecentraalBeheer.Vereniging;
using Events;

public interface IScenario
{
    Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service);
    VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => [];
}
