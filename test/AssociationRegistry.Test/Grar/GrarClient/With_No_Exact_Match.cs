﻿namespace AssociationRegistry.Test.Grar.GrarClient;

using Fixtures;
using FluentAssertions;
using Xunit;

public class With_No_Exact_Match : IClassFixture<WithNoExactMatchFixture>
{
    private readonly WithNoExactMatchFixture _fixture;

    public With_No_Exact_Match(WithNoExactMatchFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_It_Returns_A_SuccessResult()
    {
        _fixture.Result.Should().NotBeEmpty();
        _fixture.Result.Max(r => r.Score).Should().BeLessOrEqualTo(99);
    }
}
