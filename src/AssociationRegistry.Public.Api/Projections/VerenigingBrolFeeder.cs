namespace AssociationRegistry.Public.Api.Projections;

using System;
using System.Collections.Generic;

public interface IVerenigingBrolFeeder
{
    string KorteNaam { get; }
    string Hoofdlocatie { get; }
    string AndereLocaties { get; }
    string Hoofdactiviteit { get; }
    string Doelgroep { get; }
}

public class VerenigingBrolFeeder : IVerenigingBrolFeeder
{
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

    private string GetHoofdactiviteit()
    {
        var activiteiten = BrolFeederHoofdactiviteit.All();
        var index = _random.Next(activiteiten.Count);

        return activiteiten[index].ToString();
    }

    public string KorteNaam
        => ComposeText(1);

    public string Hoofdlocatie
        => ComposeText(3);

    public string AndereLocaties
        => ComposeText();

    public string Hoofdactiviteit
        => GetHoofdactiviteit();

    public string Doelgroep
        => ComposeText(1);
}
