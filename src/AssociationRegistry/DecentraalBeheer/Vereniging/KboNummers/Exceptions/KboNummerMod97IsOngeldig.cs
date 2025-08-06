namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using System.Runtime.Serialization;

[Serializable]
public class KboNummerMod97IsOngeldig : KboNummerIsOngeldig
{
    public KboNummerMod97IsOngeldig() : base(ExceptionMessages.InvalidKboNummerMod97)
    {
    }

    protected KboNummerMod97IsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
