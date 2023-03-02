namespace AssociationRegistry.Admin.Api.Verenigingen;

using System;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.ConfigurationBindings;
using Newtonsoft.Json;

public class BevestigingsTokenHelper
{
    private string _salt;

    public BevestigingsTokenHelper(AppSettings appSettings)
    {
        _salt = appSettings.Salt;
    }
    public bool IsValid(string? bevestigingsToken, object request)
    {
        if (bevestigingsToken is null) return false;
        return Calculate(request).Equals(bevestigingsToken);
    }

    public string Calculate(object request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var provider = MD5.Create();
        var bytes = provider.ComputeHash(Encoding.Unicode.GetBytes(_salt + JsonConvert.SerializeObject(request)));
        return BitConverter.ToString(bytes);
    }
}
