namespace AssociationRegistry.Public.Api.Projections;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions;
using SearchVerenigingen;

public interface IVerenigingBrolFeeder
{
    VerenigingDocument.Locatie Hoofdlocatie { get; }
    ImmutableArray<VerenigingDocument.Locatie> Locaties { get; }
    VerenigingDocument.Hoofdactiviteit[] Hoofdactiviteiten { get; }
    string Doelgroep { get; }
    ImmutableArray<string> Activiteiten { get; }
    void SetStatic();
}

public class VerenigingBrolFeeder : IVerenigingBrolFeeder
{
    private static bool isStatic = false;

    private readonly List<string> _randomTexts = new()
    {
        "harbor     ",
        "veil       ",
        "control    ",
        "subway     ",
        "faint      ",
        "joy        ",
        "watch      ",
        "abbey      ",
        "far        ",
        "sink       ",
        "budget     ",
        "salmon     ",
        "facility   ",
        "parameter  ",
        "stumble    ",
        "evening    ",
        "valley     ",
        "branch     ",
        "network    ",
        "snail      ",
        "literacy   ",
        "limit      ",
        "gain       ",
        "need       ",
        "thirsty    ",
        "wine       ",
        "medieval   ",
        "poem       ",
        "diamond    ",
        "deserve    ",
        "pottery    ",
        "ankle      ",
        "soup       ",
        "trade      ",
        "relate     ",
        "relief     ",
        "aisle      ",
        "warm       ",
        "clock      ",
        "premature  ",
        "polite     ",
        "diplomat   ",
        "soar       ",
    };

    private readonly Random _random = new();

    private string ComposeText(int maxNumberOfWords = 7)
    {
        var numberOfWords = _random.Next(1, maxNumberOfWords);
        var words = new string[numberOfWords];

        for (var i = 0; i < numberOfWords; i++)
        {
            words[i] = _randomTexts[_random.Next(_randomTexts.Count - 1)].TrimEnd();
        }

        return string.Join(' ', words);
    }

    private IEnumerable<T> ComposeArray<T>(int maxNumberOfElements, Func<T> composeText)
    {
        var numberOfElements = _random.Next(1, maxNumberOfElements);

        for (var i = 0; i < numberOfElements; i++)
        {
            yield return composeText();
        }
    }

    private BrolFeederHoofdactiviteit GetHoofdactiviteit()
    {
        var hoofdactiviteiten = BrolFeederHoofdactiviteit.All();
        var index = _random.Next(hoofdactiviteiten.Count);

        return hoofdactiviteiten[index];
    }

    public VerenigingDocument.Locatie Hoofdlocatie
        => isStatic
            ? new VerenigingDocument.Locatie("0123", "abcdefg")
            : new VerenigingDocument.Locatie(GetPostcode(), ComposeText(1));

    public ImmutableArray<VerenigingDocument.Locatie> Locaties
        => isStatic
            ? new[]
                {
                    new VerenigingDocument.Locatie("0123", "abcdefg"),
                    new VerenigingDocument.Locatie("0987", "zyxwv"),
                }
                .ToImmutableArray()
            : ComposeArray(3, () => new VerenigingDocument.Locatie(GetPostcode(), ComposeText(1)))
                .ToImmutableArray();

    private string GetPostcode()
        => $"{_random.Next(0, 999):0000}";

    public VerenigingDocument.Hoofdactiviteit[] Hoofdactiviteiten
    {
        get
        {
            var hoofdactiviteiten = isStatic
                ? BrolFeederHoofdactiviteit.Create("BWRK").ObjectToArray()
                : ComposeArray(3, GetHoofdactiviteit).ToArray();

            return hoofdactiviteiten.Select(h => new VerenigingDocument.Hoofdactiviteit(h.Code, h.Naam)).ToArray();
        }
    }

    public string Doelgroep
        => isStatic
            ? "+18"
            : ComposeText(1);

    public ImmutableArray<string> Activiteiten
        => isStatic
            ? new[] { "Basketbal", "Tennis", "Padel" }
                .ToImmutableArray()
            : ComposeArray(3, () => ComposeText(1)).ToImmutableArray();

    public void SetStatic()
        => isStatic = true;
}
