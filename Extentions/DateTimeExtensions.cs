namespace Apptivate_UQMS_WebApp.Extentions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek)
        {
            var diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
            return dateTime.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek endOfWeek)
        {
            var diff = (7 + (endOfWeek - dateTime.DayOfWeek)) % 7;
            return dateTime.AddDays(diff).Date;
        }
    }
}
