namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Ipdc;

using AssociationRegistry.Resources;

[Serializable]
public class IpdcException : Exception
{
    public IpdcException(Exception? innerException)
        : base(string.Format(ExceptionMessages.IpdcException, innerException)) { }

    public IpdcException(string message)
        : base(string.Format(ExceptionMessages.IpdcException, message)) { }
}
