using System;

namespace Scouting.DataLayer.Helpers
{
    public static class TimeSpanHelpers
    {
        public static string ToReadableAgeString(this TimeSpan span)
        {
            return string.Format("{0:0}", span.Days / 365.25);
        }

        public static string ToReadableString(this TimeSpan span, bool showSeconds = true)
        {
            var formatted = string.Format("{0}{1}{2}{3}",
                0 < Math.Floor(span.TotalDays) ? string.Format("{0:0} day{1}", Math.Floor(span.TotalDays), Math.Floor(span.TotalDays) == 1 ? String.Empty : "s") : string.Empty,
                Math.Floor(span.TotalDays) == 0 && 0 < Math.Floor(span.TotalHours) ? string.Format("{0:0} hour{1}", Math.Floor(span.TotalHours), Math.Floor(span.TotalHours) == 1 ? String.Empty : "s") : string.Empty,
                Math.Floor(span.TotalDays) == 0 && Math.Floor(span.TotalHours) == 0 && 0 < Math.Floor(span.TotalMinutes) ? string.Format("{0:0} minute{1}", Math.Floor(span.TotalMinutes), Math.Floor(span.TotalMinutes) == 1 ? String.Empty : "s") : string.Empty,
                (Math.Floor(span.TotalDays) == 0 && Math.Floor(span.TotalHours) == 0 && Math.Floor(span.TotalMinutes) == 0) && showSeconds == true ? string.Format("{0:0} second{1}", Math.Floor(span.TotalSeconds), Math.Floor(span.TotalSeconds) == 1 ? String.Empty : "s") : string.Empty);

            if (string.IsNullOrEmpty(formatted) && showSeconds == true)
            {
                formatted = "0 seconds";
            }

            return formatted + " ago";
        }
    }
}