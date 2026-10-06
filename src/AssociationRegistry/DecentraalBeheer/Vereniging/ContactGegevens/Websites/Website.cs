namespace AssociationRegistry.DecentraalBeheer.Vereniging.Websites;

using Exceptions;
using Framework;

public record Website(string Waarde, string Beschrijving, bool IsPrimair)
    : Contactgegeven(Contactgegeventype.Website, Waarde, Beschrijving, IsPrimair)
{
    public static readonly Website Leeg = new(string.Empty, string.Empty, IsPrimair: false);

    public static Website Create(string? website) => Create(website, string.Empty, isPrimair: false);

    public static Website Create(string? website, string beschrijving, bool isPrimair)
    {
        if (string.IsNullOrEmpty(website))
            return Leeg;

        Throw<OngeldigUrl>.IfNot(IsValid(website));

        return new Website(website, beschrijving, isPrimair);
    }

    public static bool IsValid(string url)
    {
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            return false;

        if (uri.IsAbsoluteUri)
            return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) && HasValidHost(uri.Host);

        return Uri.TryCreate($"{Uri.UriSchemeHttp}://{url}", UriKind.Absolute, out var resolved)
            && HasValidHost(resolved.Host);
    }

    private static bool HasValidHost(string host) =>
        !string.IsNullOrWhiteSpace(host) && host.Contains('.') && !host.StartsWith('.') && !host.EndsWith('.');
}
