namespace ONEbook.UM.Api.Helpers;

using System;

public interface IDateTimeHelper
{
    string GetUtcNowShortDateString();
}

public class DateTimeHelper : IDateTimeHelper
{
    public string GetUtcNowShortDateString() => DateTimeOffset.UtcNow.DateTime.ToShortDateString();
}