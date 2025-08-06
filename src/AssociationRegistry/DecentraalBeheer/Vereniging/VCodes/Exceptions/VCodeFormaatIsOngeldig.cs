namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
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
