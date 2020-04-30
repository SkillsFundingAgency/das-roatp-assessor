using System;

namespace SFA.DAS.RoatpAssessor.Web.Extensions
{
    public static class DateTimeUtils
    {
        public static string ToSfaShortDateString(this DateTime time)
        {
            return time.ToString("dd MMMM yyyy");
        }

        public static string ToSfaShortDateString(this DateTime? time)
        {
            return time?.ToString("dd MMMM yyyy");
        }
    }
}
