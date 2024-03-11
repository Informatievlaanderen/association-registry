namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class VCodeFormaatIsOngeldig : VCodeIsOngeldig
{
    public VCodeFormaatIsOngeldig() : base(ExceptionMessages.InvalidVCodeFormat)
    {
    }

    protected VCodeFormaatIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
