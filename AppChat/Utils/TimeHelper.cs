namespace AppChat.Utils
{
    public static class TimeHelper
    {
        public static string ConvertToVietnamTime(DateTime utcTime)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh"
            );

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tz)
                               .ToString("HH:mm:ss dd/MM/yyyy");
        }
    }
}
