namespace AssociationRegistry.Vereniging.Exceptions;

using Resources;
using System.Runtime.Serialization;

[Serializable]
public class KboNummerBevatOngeldigeTekens : KboNummerIsOngeldig
{
    public KboNummerBevatOngeldigeTekens() : base(ExceptionMessages.InvalidKboNummerChars)
    {
    }

    protected KboNummerBevatOngeldigeTekens(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
