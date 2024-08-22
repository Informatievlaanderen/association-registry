namespace AssociationRegistry.Vereniging.Exceptions;

using Resources;
using System.Runtime.Serialization;

[Serializable]
public class VCodeValtBuitenToegelatenWaardes : VCodeIsOngeldig
{
    public VCodeValtBuitenToegelatenWaardes() : base(ExceptionMessages.OutOfRangeVCode)
    {
    }

    protected VCodeValtBuitenToegelatenWaardes(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
