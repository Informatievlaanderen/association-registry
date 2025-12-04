namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz: DomainException
{
    public EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz() : base(ExceptionMessages.EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz)
    {
    }

    protected EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
