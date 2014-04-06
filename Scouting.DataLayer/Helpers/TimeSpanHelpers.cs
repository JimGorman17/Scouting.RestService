using System;

namespace Scouting.DataLayer.Helpers
{
    public static class TimeSpanHelpers
    {
        public static string ToReadableAgeString(this TimeSpan span)
        {
            return string.Format("{0:0}", span.Days / 365.25);
        }

        public static string ToReadableString(this TimeSpan span, bool showSeconds = true, bool appendAgo = false)
        {
            var formatted = string.Format("{0}{1}{2}{3}",
                0 < span.Duration().Days ? string.Format("{0:0} day{1}", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                0 < span.Duration().Days || 0 < span.Duration().Hours ? string.Format("{2}{0:0} hour{1}", span.Hours, span.Hours == 1 ? String.Empty : "s", 0 < span.Duration().Days ? ", " : String.Empty) : string.Empty,
                0 < span.Duration().Days || 0 < span.Duration().Hours || 0 < span.Duration().Minutes ? string.Format("{2}{0:0} minute{1}", span.Minutes, span.Minutes == 1 ? String.Empty : "s", 0 < span.Duration().Days || 0 < span.Duration().Hours ? ", " : String.Empty) : string.Empty,
                (0 < span.Duration().Days || 0 < span.Duration().Hours || 0 < span.Duration().Minutes || 0 < span.Duration().Seconds) && showSeconds == true ? string.Format("{2}{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s", 0 < span.Duration().Days || 0 < span.Duration().Hours || 0 < span.Duration().Minutes ? ", " : String.Empty) : string.Empty);

            if (string.IsNullOrEmpty(formatted) && showSeconds == true)
            {
                formatted = "0 seconds";
            }

            if (string.IsNullOrEmpty(formatted) == false && appendAgo == true)
            {
                formatted += " ago";
            }

            return formatted;
        }
    }
}