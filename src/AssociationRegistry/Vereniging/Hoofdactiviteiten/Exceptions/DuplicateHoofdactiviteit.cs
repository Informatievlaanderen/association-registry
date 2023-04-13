namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateHoofdactiviteit : DomainException
{
    public DuplicateHoofdactiviteit() : base("Een waarde in de hoofdactiviteitenLijst mag slechts 1 maal voorkomen.")
    {
    }

    protected DuplicateHoofdactiviteit(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
