namespace AssociationRegistry.Test.Erkenningen.When_Creating_ErkenningCorrectie;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Primitives;
using Xunit;

public class Given_Alle_Te_Corrigeren_Waarden
{
    public Given_Alle_Te_Corrigeren_Waarden()
    {
        new Fixture().CustomizeDomain();
    }

    [Fact]
    public void With_TeCorrigeren_Startdatum_Null_Then_Startdatum_Is_State_Startdatum()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.ErkenningsPeriode.Startdatum.Should().Be(erkenning.ErkenningsPeriode.Startdatum);
    }

    [Fact]
    public void With_TeCorrigeren_Startdatum_Empty_Then_Startdatum_Is_Null()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Empty,
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.ErkenningsPeriode.Startdatum.Should().BeNull();
    }

    [Fact]
    public void With_TeCorrigeren_Startdatum_Value_Then_Startdatum_Is_Command_Startdatum()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 2, 1)),
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.ErkenningsPeriode.Startdatum.Should().Be(teCorrigerenErkenning.StartDatum.Value);
    }

    [Fact]
    public void With_TeCorrigeren_Einddatum_Null_Then_Einddatum_Is_State_Einddatum()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            EindDatum = NullOrEmpty<DateOnly>.Null,
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.ErkenningsPeriode.Einddatum.Should().Be(erkenning.ErkenningsPeriode.Einddatum);
    }

    [Fact]
    public void With_TeCorrigeren_Einddatum_Empty_Then_Einddatum_Is_Null()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            EindDatum = NullOrEmpty<DateOnly>.Empty,
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.ErkenningsPeriode.Einddatum.Should().BeNull();
    }

    [Fact]
    public void With_TeCorrigeren_Einddatum_Value_Then_Einddatum_Is_Command_Einddatum()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            EindDatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2027, 12, 1)),
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.ErkenningsPeriode.Einddatum.Should().Be(teCorrigerenErkenning.EindDatum.Value);
    }

    [Fact]
    public void With_TeCorrigeren_Hernieuwingsdatum_Null_Then_Hernieuwingsdatum_Is_State_Hernieuwingsdatum()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.Hernieuwingsdatum.Should().Be(erkenning.Hernieuwingsdatum);
    }

    [Fact]
    public void With_TeCorrigeren_Hernieuwingsdatum_Empty_Then_Hernieuwingsdatum_Is_Null()
    {
        var erkenning = MaakStateErkenning() with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(null, null),
        };

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Empty,
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.Hernieuwingsdatum.Value.Should().BeNull();
    }

    [Fact]
    public void With_TeCorrigeren_Hernieuwingsdatum_Value_Then_Hernieuwingsdatum_Is_Command_Hernieuwingsdatum()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 5, 1)),
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.Hernieuwingsdatum.Value.Should().Be(teCorrigerenErkenning.Hernieuwingsdatum.Value);
    }

    [Fact]
    public void With_TeCorrigeren_HernieuwingsUrl_Null_Then_HernieuwingsUrl_Is_State_HernieuwingsUrl()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            HernieuwingsUrl = null,
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.HernieuwingsUrl.Should().Be(erkenning.HernieuwingsUrl);
    }

    [Fact]
    public void With_TeCorrigeren_HernieuwingsUrl_Empty_Then_HernieuwingsUrl_Is_Command_HernieuwingsUrl()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            HernieuwingsUrl = string.Empty,
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.HernieuwingsUrl.Value.Should().Be(teCorrigerenErkenning.HernieuwingsUrl);
    }

    [Fact]
    public void With_TeCorrigeren_HernieuwingsUrl_Value_Then_HernieuwingsUrl_Is_Command_HernieuwingsUrl()
    {
        var erkenning = MaakStateErkenning();

        var teCorrigerenErkenning = MaakTeCorrigerenErkenning() with
        {
            HernieuwingsUrl = "https://anewurl.isborn",
        };

        var result = ErkenningCorrectie.Create(teCorrigerenErkenning, erkenning);

        result.HernieuwingsUrl.Value.Should().Be(teCorrigerenErkenning.HernieuwingsUrl);
    }

    private static Erkenning MaakStateErkenning()
    {
        var periode = ErkenningsPeriode.Create(
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31));

        return new Erkenning
        {
            ErkenningId = 123,
            ErkenningsPeriode = periode,
            Hernieuwingsdatum = Hernieuwingsdatum.Create(new DateOnly(2026, 6, 1), periode),
            HernieuwingsUrl = HernieuwingsUrl.Create("https://example.org/hernieuw"),
        };
    }

    private static TeCorrigerenErkenning MaakTeCorrigerenErkenning()
    {
        return TeCorrigerenErkenning.Create(123,
                                            NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 1, 1)),
                                            NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 12, 31)),
                                            NullOrEmpty<DateOnly>.Create(new DateOnly(2026, 10, 10)),
                                            "https://a-new-url.isborn"
        );

        ;
    }
}
