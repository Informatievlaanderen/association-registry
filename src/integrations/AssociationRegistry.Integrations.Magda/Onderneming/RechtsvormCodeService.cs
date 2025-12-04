namespace AssociationRegistry.Integrations.Magda.Onderneming;

using AssociationRegistry.Magda;
using Shared.Constants;

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
