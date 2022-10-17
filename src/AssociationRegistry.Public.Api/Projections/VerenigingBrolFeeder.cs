namespace AssociationRegistry.Public.Api.Projections;

using System;
using System.Collections.Generic;

public class VerenigingBrolFeeder
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
        "sex        ",
        "facility   ",
        "parameter  ",
        "stumble    ",
        "evening    ",
        "rub        ",
        "valley     ",
        "branch     ",
        "network    ",
        "war        ",
        "snail      ",
        "pill       ",
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
        "bullet     ",
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
        "hostage    ",
        "abnormal   ",
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

    public string KorteNaam
        => ComposeText(1);

    public string Hoofdlocatie
        => ComposeText(3);

    public string AndereLocaties
        => ComposeText();

    public string PROTPUT
        => ComposeText(1);

    public string Doelgroep
        => ComposeText(1);
}
