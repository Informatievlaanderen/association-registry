﻿namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using NodaTime;
using System;

public static class StringExtensions
{
    public static bool IsNullOrBeforeToday(this DateOnly? dateOnly)
        => dateOnly is null || dateOnly <= DateOnly.FromDateTime(SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());

    public static bool IsNullOrAfterToday(this DateOnly? dateOnly)
        => dateOnly is null || dateOnly >= DateOnly.FromDateTime(SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc());
}
