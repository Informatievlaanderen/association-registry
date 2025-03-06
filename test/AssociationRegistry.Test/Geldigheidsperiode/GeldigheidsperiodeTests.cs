namespace AssociationRegistry.Test;

using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using System;
using Xunit;
using Geldigheidsperiode = AssociationRegistry.Geldigheidsperiode;

public class GeldigheidsperiodeTests
{
    [Fact]
    public void GeldigheidsperiodesOverlapWhenTheyHaveTheSameBeginAndTheSameEnd()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 01)), new GeldigTot(new DateOnly(2017, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 01)), new GeldigTot(new DateOnly(2017, 01, 01)));

        period1.OverlapsWith(period2).Should().BeTrue();
        period2.OverlapsWith(period1).Should().BeTrue();
    }

    [Fact]
    public void AGeldigheidsperiodeOverlapsWhenItsBeginAndEndFallWithinTheOtherGeldigheidsperiode()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2015, 01, 01)), new GeldigTot(new DateOnly(2018, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 01)), new GeldigTot(new DateOnly(2017, 01, 01)));

        period1.OverlapsWith(period2).Should().BeTrue();
    }

    [Fact]
    public void AGeldigheidsperiodeOverlapsWhenTheOtherGeldigheidsperiodesBeginAndEndFallWithinItself()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2015, 01, 01)), new GeldigTot(new DateOnly(2018, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 01)), new GeldigTot(new DateOnly(2017, 01, 01)));

        period2.OverlapsWith(period1).Should().BeTrue();
    }

    [Fact]
    public void AGeldigheidsperiodeOverlapsWhenItsEndFallsBetweenTheOtherGeldigheidsperiodesBeginAndEnd()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2015, 01, 01)), new GeldigTot(new DateOnly(2017, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 01)), new GeldigTot(new DateOnly(2018, 01, 01)));

        period1.OverlapsWith(period2).Should().BeTrue();
    }

    [Fact]
    public void AGeldigheidsperiodeOverlapsWhenItsBeginFallsBetweenTheOtherGeldigheidsperiodesBeginAndEnd()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2015, 01, 01)), new GeldigTot(new DateOnly(2017, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 01)), new GeldigTot(new DateOnly(2018, 01, 01)));

        period2.OverlapsWith(period1).Should().BeTrue();
    }

    [Fact]
    public void AGeldigheidsperiodeOverlapsWhenItHasNoEndAndItsStartIsBeforeTheOtherGeldigheidsperiodesStart()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2015, 01, 01)), new GeldigTot());
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 02)), new GeldigTot(new DateOnly(2017, 01, 01)));

        period1.OverlapsWith(period2).Should().BeTrue();
        period2.OverlapsWith(period1).Should().BeTrue();
    }


    [Fact]
    public void AGeldigheidsperiodeOverlapsWhenItHasNoStartAndItsEndIsAfterTheOtherGeldigheidsperiodesStart()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(), new GeldigTot(new DateOnly(2015, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2014, 01, 02)), new GeldigTot(new DateOnly(2016, 01, 01)));

        period1.OverlapsWith(period2).Should().BeTrue();
        period2.OverlapsWith(period1).Should().BeTrue();
    }

    [Fact]
    public void AGeldigheidsperiodeDoesNotOverlapWhenItHasNoStartAndItsStartIsAfterTheOtherGeldigheidsperiodesStart()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(), new GeldigTot(new DateOnly(2015, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 02)), new GeldigTot(new DateOnly(2017, 12, 31)));

        period1.OverlapsWith(period2).Should().BeFalse();
        period2.OverlapsWith(period1).Should().BeFalse();
    }

    [Fact]
    public void AGeldigheidsperiodeDoesNotOverlapWhenTheOtherGeldigheidsperiodesStartsAfterItsEndGeldigheidsperiode()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2015, 01, 01)), new GeldigTot(new DateOnly(2016, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 02)), new GeldigTot(new DateOnly(2017, 01, 01)));

        period2.OverlapsWith(period1).Should().BeFalse();
    }

    [Fact]
    public void AGeldigheidsperiodeDoesNotOverlapWhenTheOtherGeldigheidsperiodesEndsBeforeItsStartGeldigheidsperiode()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2015, 01, 01)), new GeldigTot(new DateOnly(2016, 01, 01)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 02)), new GeldigTot(new DateOnly(2017, 01, 01)));

        period1.OverlapsWith(period2).Should().BeFalse();
        period2.OverlapsWith(period1).Should().BeFalse();
    }

    [Fact]
    public void SomeoneDidNotBelieveMe()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(), new GeldigTot());
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2016, 01, 02)), new GeldigTot(new DateOnly(2017, 01, 01)));

        period1.OverlapsWith(period2).Should().BeTrue();
        period2.OverlapsWith(period1).Should().BeTrue();
    }

    [Fact]
    public void SomeoneStillDidntBelieveMe()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(), new GeldigTot());
        var period2 = new Geldigheidsperiode(new GeldigVan(), new GeldigTot());

        period1.OverlapsWith(period2).Should().BeTrue();
        period2.OverlapsWith(period1).Should().BeTrue();
    }

    [Fact]
    public void Nope()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(), new GeldigTot(new DateOnly(2000, 1, 1)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(1990, 1, 1)), new GeldigTot());

        period1.OverlapsWith(period2).Should().BeTrue();
        period2.OverlapsWith(period1).Should().BeTrue();
    }

    [Fact]
    public void StillNot()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(), new GeldigTot(new DateOnly(2000, 1, 1)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(2010, 1, 1)), new GeldigTot());

        period1.OverlapsWith(period2).Should().BeFalse();
        period2.OverlapsWith(period1).Should().BeFalse();
    }

    [Fact]
    public void AlmostGivingUp()
    {
        var period1 = new Geldigheidsperiode(new GeldigVan(), new GeldigTot(new DateOnly(2000, 1, 1)));
        var period2 = new Geldigheidsperiode(new GeldigVan(new DateOnly(1900, 1, 1)), new GeldigTot(new DateOnly(2010, 1, 1)));

        period1.OverlapsWith(period2).Should().BeTrue();
        period2.OverlapsWith(period1).Should().BeTrue();
    }

    [Fact]
    public void CannotCreateAGeldigheidsperiodeWithStartDateAfterEndDate()
    {
        Assert.Throws<StartdatumLigtNaEinddatum>(() => new Geldigheidsperiode(new GeldigVan(new DateOnly(2000, 1, 2)), new GeldigTot(new DateOnly(2000, 1, 1))));
    }
}

public class IsInFutureOfTests
{
    [Fact]
    public void FutureGeldigVan_Is_InFutureOf()
    {
        new GeldigVan(new DateOnly(2017, 1, 2))
           .IsInFutureOf(new DateOnly(2017, 1, 1))
           .Should()
           .BeTrue();
    }

    [Fact]
    public void NullGeldigVan_IsNot_InFutureOf()
    {
        new GeldigVan()
           .IsInFutureOf(new DateOnly(2017, 1, 1))
           .Should()
           .BeFalse();
    }

    [Fact]
    public void PastGeldigVan_IsNot_InFutureOf()
    {
        new GeldigVan(new DateOnly(2016, 1, 2))
           .IsInFutureOf(new DateOnly(2017, 1, 1))
           .Should()
           .BeFalse();
    }

    [Fact]
    public void SameDayGeldigVan_IsNot_InFutureOfExclusive()
    {
        new GeldigVan(new DateOnly(2017, 1, 1))
           .IsInFutureOf(new DateOnly(2017, 1, 1))
           .Should()
           .BeFalse();
    }

    [Fact]
    public void SameDayGeldigVan_Is_InFutureOfInclusive()
    {
        new GeldigVan(new DateOnly(2017, 1, 1))
           .IsInFutureOf(new DateOnly(2017, 1, 1), true)
           .Should()
           .BeTrue();
    }
}

public class IsInPastOfTests
{
    [Fact]
    public void PastGeldigVan_Is_IsInPastOf()
    {
        new GeldigVan(new DateOnly(2016, 1, 1))
           .IsInPastOf(new DateOnly(2017, 1, 1))
           .Should()
           .BeTrue();
    }

    [Fact]
    public void NullGeldigVan_Is_IsInPastOf()
    {
        new GeldigVan()
           .IsInPastOf(new DateOnly(2017, 1, 1))
           .Should()
           .BeTrue();
    }

    [Fact]
    public void FutureGeldigVan_IsNot_IsInPastOf()
    {
        new GeldigVan(new DateOnly(2018, 1, 1))
           .IsInPastOf(new DateOnly(2017, 1, 1))
           .Should()
           .BeFalse();
    }

    [Fact]
    public void SameDayGeldigVan_IsNot_IsInPastOfExclusive()
    {
        new GeldigVan(new DateOnly(2017, 1, 1))
           .IsInPastOf(new DateOnly(2017, 1, 1))
           .Should()
           .BeFalse();
    }

    [Fact]
    public void SameDayGeldigVan_Is_IsInPastOfInclusive()
    {
        new GeldigVan(new DateOnly(2017, 1, 1))
           .IsInPastOf(new DateOnly(2017, 1, 1), true)
           .Should()
           .BeTrue();
    }
}

public class GeldigVanTests
{
    [Fact]
    public void WithSameDate_IsEqualTo()
    {
        new GeldigVan().Should().Be(new GeldigVan());
        new GeldigVan(new DateOnly(2016, 1, 1).AddDays(2)).Should().Be(new GeldigVan(new DateOnly(2016, 1, 1).AddDays(2)));
        new GeldigVan(2017, 1, 1).Should().Be(new GeldigVan(2017, 1, 1));
    }

    [Fact]
    public void WithDifferentDate_IsNotEqualTo()
    {
        new GeldigVan().Should().NotBe(new GeldigVan(2017, 1, 1));
        new GeldigVan(new DateOnly(2017, 1, 1).AddDays(2)).Should().NotBe(new GeldigVan(new DateOnly(2016, 1, 1).AddDays(2)));
        new GeldigVan(2018, 1, 1).Should().NotBe(new GeldigVan(2017, 1, 1));
    }

    [Fact]
    public void NullDate_IsSmallerThan_AnyDate()
    {
        (new GeldigVan(null) < new GeldigVan(DateOnly.MinValue)).Should().BeTrue();
        (new GeldigVan(null) <= new GeldigVan(DateOnly.MinValue)).Should().BeTrue();

        (new GeldigVan(DateOnly.MaxValue) > new GeldigVan(null)).Should().BeTrue();
        (new GeldigVan(DateOnly.MaxValue) >= new GeldigVan(null)).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_UseRegularGreaterThan()
    {
        (new GeldigVan(DateOnly.FromDateTime(DateTime.Today)) > new GeldigVan(DateOnly.FromDateTime(DateTime.Today).AddDays(-1))).Should().BeTrue();
        (new GeldigVan(DateOnly.FromDateTime(DateTime.Today)) >= new GeldigVan(DateOnly.FromDateTime(DateTime.Today).AddDays(-1))).Should().BeTrue();

        (new GeldigVan(DateOnly.FromDateTime(DateTime.Today).AddDays(-1)) < new GeldigVan(DateOnly.FromDateTime(DateTime.Today))).Should().BeTrue();
        (new GeldigVan(DateOnly.FromDateTime(DateTime.Today).AddDays(-1)) <= new GeldigVan(DateOnly.FromDateTime(DateTime.Today))).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_AreEqualTo()
    {
        (new GeldigVan(DateOnly.FromDateTime(DateTime.Today)) == new GeldigVan(DateOnly.FromDateTime(DateTime.Today))).Should().BeTrue();

        new GeldigVan(DateOnly.FromDateTime(DateTime.Today)).Equals(new GeldigVan(DateOnly.FromDateTime(DateTime.Today))).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_AreNotEqualTo_NullDates()
    {
        (new GeldigVan(DateOnly.FromDateTime(DateTime.Today)) == new GeldigVan()).Should().BeFalse();
        new GeldigVan(DateOnly.FromDateTime(DateTime.Today)).Equals(new GeldigVan()).Should().BeFalse();
    }

    [Fact]
    public void NullDates_AreEqualTo_NullDates()
    {
        (new GeldigVan() == new GeldigVan()).Should().BeTrue();
        new GeldigVan().Equals(new GeldigVan()).Should().BeTrue();
    }
}

public class GeldigTotTests
{
    [Fact]
    public void WithSameDate_IsEqualTo()
    {
        new GeldigTot().Should().Be(new GeldigTot());
        new GeldigTot(new DateOnly(2016, 1, 1).AddDays(2)).Should().Be(new GeldigTot(new DateOnly(2016, 1, 1).AddDays(2)));
        new GeldigTot(2017, 1, 1).Should().Be(new GeldigTot(2017, 1, 1));
    }

    [Fact]
    public void WithDifferentDate_IsNotEqualTo()
    {
        new GeldigTot().Should().NotBe(new GeldigTot(2017, 1, 1));
        new GeldigTot(new DateOnly(2017, 1, 1).AddDays(2)).Should().NotBe(new GeldigTot(new DateOnly(2016, 1, 1).AddDays(2)));
        new GeldigTot(2018, 1, 1).Should().NotBe(new GeldigTot(2017, 1, 1));
    }

    [Fact]
    public void NullDate_IsGreaterThan_AnyDate()
    {
        (new GeldigTot(null) > new GeldigTot(DateOnly.MaxValue)).Should().BeTrue();
        (new GeldigTot(null) >= new GeldigTot(DateOnly.MaxValue)).Should().BeTrue();

        (new GeldigTot(DateOnly.MaxValue) < new GeldigTot(null)).Should().BeTrue();
        (new GeldigTot(DateOnly.MaxValue) <= new GeldigTot(null)).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_UseRegularGreaterThan()
    {
        (new GeldigTot(DateOnly.FromDateTime(DateTime.Today)) > new GeldigTot(DateOnly.FromDateTime(DateTime.Today).AddDays(-1))).Should().BeTrue();
        (new GeldigTot(DateOnly.FromDateTime(DateTime.Today)) >= new GeldigTot(DateOnly.FromDateTime(DateTime.Today).AddDays(-1))).Should().BeTrue();

        (new GeldigTot(DateOnly.FromDateTime(DateTime.Today).AddDays(-1)) < new GeldigTot(DateOnly.FromDateTime(DateTime.Today))).Should().BeTrue();
        (new GeldigTot(DateOnly.FromDateTime(DateTime.Today).AddDays(-1)) <= new GeldigTot(DateOnly.FromDateTime(DateTime.Today))).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_AreEqualTo()
    {
        (new GeldigTot(DateOnly.FromDateTime(DateTime.Today)) == new GeldigTot(DateOnly.FromDateTime(DateTime.Today))).Should().BeTrue();

        new GeldigTot(DateOnly.FromDateTime(DateTime.Today)).Equals(new GeldigTot(DateOnly.FromDateTime(DateTime.Today))).Should().BeTrue();
    }

    [Fact]
    public void RegularDates_AreNotEqualTo_NullDates()
    {
        (new GeldigTot(DateOnly.FromDateTime(DateTime.Today)) == new GeldigTot()).Should().BeFalse();
        new GeldigTot(DateOnly.FromDateTime(DateTime.Today)).Equals(new GeldigTot()).Should().BeFalse();
    }

    [Fact]
    public void NullDates_AreEqualTo_NullDates()
    {
        (new GeldigTot() == new GeldigTot()).Should().BeTrue();
        new GeldigTot().Equals(new GeldigTot()).Should().BeTrue();
    }
}
