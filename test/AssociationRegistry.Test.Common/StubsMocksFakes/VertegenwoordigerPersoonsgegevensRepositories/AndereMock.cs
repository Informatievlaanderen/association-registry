namespace AssociationRegistry.Test.Common.StubsMocksFakes.VertegenwoordigerPersoonsgegevensRepositories;

using Admin.Schema.Persoonsgegevens;
using Persoonsgegevens;

public class AndereMock
{
    private readonly VertegenwoordigerPersoonsgegevensRepositoryMock _m;

    public AndereMock(VertegenwoordigerPersoonsgegevensRepositoryMock m)
    {
        _m = m;
    }

    public VertegenwoordigerPersoonsgegevensDocument? FindByRefId(Guid refId)
        => _m.FindByRefId(refId);
}

