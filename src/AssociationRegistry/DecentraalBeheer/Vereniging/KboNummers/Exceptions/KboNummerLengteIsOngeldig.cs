namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using System.Runtime.Serialization;

[Serializable]
public class KboNummerLengteIsOngeldig : KboNummerIsOngeldig
{
    public KboNummerLengteIsOngeldig() : base(ExceptionMessages.InvalidKboNummerLength)
    {
    }

    protected KboNummerLengteIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
