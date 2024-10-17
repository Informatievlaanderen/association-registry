namespace AssociationRegistry.Magda.Services;

using Constants;

public class RechtsvormCodeService: IRechtsvormCodeService
{
    public bool IsValidRechtsvormCode(string code)
    {
        return new[]
        {
            RechtsvormCodes.IVZW,
            RechtsvormCodes.VZW,
            RechtsvormCodes.StichtingVanOpenbaarNut,
            RechtsvormCodes.PrivateStichting,
        }.Contains(code);
    }
}
