using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class TimeSpanExtension
{
    public static string ToReadableString(this TimeSpan span)
    {
        return string.Join(", ", span.GetReadableStringElements().Where(str => !string.IsNullOrEmpty(str)).ToArray());
    }

    public static string ToFormatedString(this TimeSpan span)
    {
        return string.Join(" ", span.GetFormatedElements().Where(str => !string.IsNullOrEmpty(str)).ToArray());
    }

    public static string ToShortString(this TimeSpan span)
    {
        return ((int)Math.Floor(span.TotalDays) * 24 + span.Hours).ToString("00") + ":" + span.Minutes.ToString("00");
    }

    private static IEnumerable<string> GetFormatedElements(this TimeSpan span)
    {
        yield return GetHoras((int)Math.Floor(span.TotalDays), span.Hours);
        yield return GetMinutos(span.Minutes);
    }

    private static IEnumerable<string> GetReadableStringElements(this TimeSpan span)
    {
        yield return GetDaysString((int)Math.Floor(span.TotalDays));
        yield return GetHoursString(span.Hours);
        yield return GetMinutesString(span.Minutes);
        yield return GetSecondsString(span.Seconds);
    }

    private static string GetDaysString(int days)
    {
        if (days == 0)
            return string.Empty;

        if (days == 1)
            return "1 dia";

        return string.Format("{0:0} dias", days);
    }

    private static string GetHoras(int days, int hours)
    {
        hours += days * 24;

        if (hours == 1)
            return "1 hr";

        return string.Format("{0:0} hrs", hours);
    }

    private static string GetHoursString(int hours)
    {
        if (hours == 0)
            return string.Empty;

        if (hours == 1)
            return "1 hora";

        return string.Format("{0:0} horas", hours);
    }

    private static string GetMinutos(int minutes)
    {
        if (minutes == 0)
            return string.Empty;

        return string.Format("{0:0} min", minutes);
    }

    private static string GetMinutesString(int minutes)
    {
        if (minutes == 0)
            return string.Empty;

        if (minutes == 1)
            return "1 minuto";

        return string.Format("{0:0} minutos", minutes);
    }

    private static string GetSecondsString(int seconds)
    {
        if (seconds == 0)
            return string.Empty;

        if (seconds == 1)
            return "1 segundo";

        return string.Format("{0:0} segundos", seconds);
    }
}
