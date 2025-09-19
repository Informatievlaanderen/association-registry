namespace AssociationRegistry.DecentraalBeheer.Vereniging.DuplicaatDetectie;

using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public interface IBevestigingsTokenCalculator
{
    string Calculate(string key, object request);
}

public sealed class Md5BevestigingsTokenCalculator : IBevestigingsTokenCalculator
{
    private readonly string _salt;

    public Md5BevestigingsTokenCalculator(string salt)
    {
        _salt = salt;
    }

    public string Calculate(string key, object request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key is required.", nameof(key));

        using var md5 = MD5.Create();

        // Canonicalize inputs: GUID in "N" form, JSON of request, and a delimiter to avoid ambiguity.
        // NOTE: Keeping Encoding.Unicode + BitConverter.ToString to preserve your original output shape.
        var keyN = NormalizeGuidOrThrow(key); // throws if not a GUID
        var json = JsonConvert.SerializeObject(request);
        var payload = $"{_salt}|{keyN}|{json}";
        var bytes = md5.ComputeHash(Encoding.Unicode.GetBytes(payload));
        return BitConverter.ToString(bytes); // e.g. "AA-BB-CC-..."
    }

    private static string NormalizeGuidOrThrow(string key)
    {
        if (!Guid.TryParse(key, out var guid))
            throw new FormatException("Key must be a valid GUID.");
        return guid.ToString("N");
    }
}

public sealed record Bevestigingstoken(string Key, string Token)
{
    public override string ToString() => $"{Key}.{Token}";

    public static Bevestigingstoken ParseAndValidate(
        string candidate,
        object request,
        IBevestigingsTokenCalculator calculator)
    {
        if (string.IsNullOrWhiteSpace(candidate))
            throw new FormatException("Bevestigingstoken is required.");

        if (calculator is null) throw new ArgumentNullException(nameof(calculator));
        if (request is null) throw new ArgumentNullException(nameof(request));

        var parts = candidate.Split('.', StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
            throw new FormatException("Bevestigingstoken must be in the form 'key.token'.");

        var keyRaw = parts[0];
        var token = parts[1];

        if (string.IsNullOrWhiteSpace(token))
            throw new FormatException("Token cannot be empty.");

        // Canonicalize key to "N" so hashing is stable.
        if (!Guid.TryParse(keyRaw, out var guid))
            throw new FormatException("Key must be a valid GUID.");
        var keyN = guid.ToString("N");

        var expected = calculator.Calculate(keyN, request);
        if (!string.Equals(token, expected, StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid bevestigingstoken: token does not match request.");

        // Return canonicalized key
        return new Bevestigingstoken(keyN, token);
    }

    public static Bevestigingstoken GenerateFor(
        object request,
        IBevestigingsTokenCalculator calculator)
    {
        if (calculator is null) throw new ArgumentNullException(nameof(calculator));
        if (request is null) throw new ArgumentNullException(nameof(request));

        var keyN = Guid.NewGuid().ToString("N");
        var token = calculator.Calculate(keyN, request);
        return new Bevestigingstoken(keyN, token);
    }
}
