namespace AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;

public interface IBevestigingsTokenHelper
{
    bool IsValid(string? bevestigingsToken, object request);
    string Calculate(object request);
}
