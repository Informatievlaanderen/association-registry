namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanGeenDubbelWordenVanZichzelf : DomainException
{
    public VerenigingKanGeenDubbelWordenVanZichzelf() : base(ExceptionMessages.VerenigingKanGeenDubbelWordenVanZichzelf)
    {
    }

    protected VerenigingKanGeenDubbelWordenVanZichzelf(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

[Serializable]
public class InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf : ApplicationException
{
    public InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf() : base(ExceptionMessages.VerenigingKanGeenDubbelWordenVanZichzelf)
    {
    }

    protected InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
