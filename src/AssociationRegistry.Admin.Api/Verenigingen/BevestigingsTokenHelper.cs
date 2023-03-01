namespace AssociationRegistry.Admin.Api.Verenigingen;

using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public class BevestigingsTokenHelper
{
    public static bool IsValid(string? bevestigingsToken, object request)
    {
        if (bevestigingsToken is null) return false;
        return Calculate(request).Equals(bevestigingsToken);
    }

    public static string Calculate(object request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var provider = MD5.Create();
        var salt = "S0m3R@nd0mSalt"; // todo: from configuration
        var bytes = provider.ComputeHash(Encoding.Unicode.GetBytes(salt + JsonConvert.SerializeObject(request)));
        return BitConverter.ToString(bytes);
    }
}
