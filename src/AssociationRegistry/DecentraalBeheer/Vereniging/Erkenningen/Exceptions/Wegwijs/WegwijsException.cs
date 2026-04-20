namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions.Wegwijs;

using Resources;

[Serializable]
public class WegwijsException : Exception
{
    public WegwijsException(Exception? innerException)
        : base(string.Format(ExceptionMessages.WegwijsException, innerException)) { }

    public WegwijsException(string message)
        : base(string.Format(ExceptionMessages.WegwijsException, message)) { }
}
