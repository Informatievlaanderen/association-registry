namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

/// <summary>vr-namens is the aanvrager, or no partij of the relatieaanvraag.</summary>
[Serializable]
public class NamensIsNietDeOntvanger : DomainException
{
    public NamensIsNietDeOntvanger()
        : base(ExceptionMessages.NamensIsNietDeOntvanger) { }

    protected NamensIsNietDeOntvanger(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
