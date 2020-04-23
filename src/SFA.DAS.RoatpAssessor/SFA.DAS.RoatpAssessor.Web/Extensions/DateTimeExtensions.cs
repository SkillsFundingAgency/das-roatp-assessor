using System;

namespace SFA.DAS.RoatpAssessor.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToSfaShortDateString(this DateTime time)
        {
            return time.ToString("dd MMMM yyyy");
        }

        public static string ToSfaShortDateString(this DateTime? time)
        {
            return time == null ? string.Empty : time.Value.ToString("dd MMMM yyyy");
        }

        public static string ToSfaShortestDateString(this DateTime time)
        {
            return time.ToString("dd MMM yy");
        }

        public static string ToSfaShortestDateString(this DateTime? time)
        {
            return time == null ? string.Empty : time.Value.ToString("dd MMM yy");
        }
    }
}
