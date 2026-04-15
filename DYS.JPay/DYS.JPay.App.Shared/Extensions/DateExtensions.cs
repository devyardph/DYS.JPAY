using DYS.JPay.Common.Dtos;
using System.Globalization;

namespace DYS.JPay.Common.Extensions
{
	public static class DateExtensions
    {
        public static DateTime GetCurrentDateInUTC() => DateTime.UtcNow;
        public static DateTime GetCurrentDate() => DateTime.Now;
        public static DateTimeOffset LocalTime(this DateTime now, 
            string timeZone = "New Zealand Standard Time")
        {
            DateTime dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, timeZone);
            return dateTime;
        }

        public static string FormatDate(this DateTime? value, string format= "dd MMM yyyy", string timeZone = "Asia/Singapore")
		{
            if (value == null) return string.Empty;
            DateTime dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(value.Value, timeZone);
            return dateTime.ToString(format);
        }

        public static DateTime? ToTimezone(this DateTime? value, string timeZone = "Asia/Singapore")
        {
            if (value == null) return null;
            DateTime dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(value.Value, timeZone);
            return dateTime;
        }

        public static DateTime? ToUTCTime(this DateTime? value)
        {
            if (value == null) return null;
            DateTime date = value.Value.ToUniversalTime();
            return date;
        }

        public static DateTime ToUTCTime(this DateTime value)
        {
            DateTime date = value.ToUniversalTime();
            return date;
        }

        public static string FormatDate(this TimeSpan? value, string format = "hh:mm tt")
        {
            return value.HasValue ? (DateTime.Today + value.Value).ToString(format) : string.Empty;
        }

        public static string FormatDate(this DateTime value, string format = "dd MMM yyyy")
		{
			return value.ToString(format);
		}

		public static DateTime? StartDay(this DateTime? input)
		{
			if (!input.HasValue) return null;
			var date = new DateTime(input.Value.Year, input.Value.Month, input.Value.Day, 0, 0, 0, 0);
			return date;
		}

        public static DateTime StartOfDay(this DateTime input)
        {
            var date = new DateTime(input.Year, input.Month, input.Day, 0, 0, 0, 0);
            return date;
        }

        public static DateTime ToDateTime(string? value)
        {
            if (value == null)
                return new DateTime(0);
            return DateTime.Parse(value);
        }

        public static DateTime? EndOfDay(this DateTime? input)
		{
            if (!input.HasValue) return null;
            var date = new DateTime(input.Value.Year, input.Value.Month, input.Value.Day, 23, 59, 59, 0);
			return date;
		}

        public static DateTime EndOfDay(this DateTime input)
        {
            var date = new DateTime(input.Year, input.Month, input.Day, 23, 59, 59, 0);
            return date;
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, 1);
		}

		public static int DaysInMonth(this DateTime value)
		{
			return DateTime.DaysInMonth(value.Year, value.Month);
		}

		public static DateTime LastDayOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.DaysInMonth());
		}


	

		public static DateTime ConvertDate(string date)
		{
			return Convert.ToDateTime(date);
		}

        public static DateTime? TryToConvertDate(string date)
        {
			if (DateTime.TryParse(date, out DateTime dateValue))
				return dateValue;
			else return null;
        }

        public static DateTime ConvertDate(this string date, string format="en-NZ")
		{
			return DateTime.Parse(date, new CultureInfo(format, true));
        }

        /// <summary>
        /// pattern example:yyyyMMddHHmm
        /// </summary>
        /// <param name="date"></param>
        /// <param name="pattern"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ConvertDate(this string date,string pattern, string format = "en-NZ")
        {
			return DateTime.ParseExact(date, pattern, new CultureInfo(format, true));
        }

        public static bool ValidateDate(string date)
		{
			if (DateTime.TryParse(date, out DateTime dateValue))
				return true;
			else return false;
		}

		public static DateTime? GetDateFromQuery(this string date)
		{

			//make sure the format is dd/mm/yyyy 09302020
			if (!string.IsNullOrEmpty(date) && date.Length == 8)
			{
				var month = date.Substring(0, 2).ToInt32();
				var day = date.Substring(2, 2).ToInt32();
				var year = date.Substring(4, 4).ToInt32();
;
				return new DateTime(year, month, day);
			}
			return null;
		}

		public static DateTime FromUnixTimeMilliseconds(long unixtime)
		{
			DateTimeOffset dateTimeOffSet = DateTimeOffset.FromUnixTimeMilliseconds(unixtime);
			DateTime dateTime = dateTimeOffSet.DateTime;
			return dateTime;
		}

		public static DateTime FromUnixTimeSeconds(long unixtime)
		{
			DateTimeOffset dateTimeOffSet = DateTimeOffset.FromUnixTimeSeconds(unixtime);
			DateTime dateTime = dateTimeOffSet.DateTime;
			return dateTime;
		}

		public static bool IsOverlapDateRange(DateTime? firstStartDate, DateTime? firstEndDate, DateTime? secondStartDate, DateTime? secondEndDate)
		{
			return firstStartDate <= secondEndDate && secondStartDate <= firstEndDate;
		}

        public static DateTime StartOfWeek(this DateTime date)
        {
            DateTime today = new DateTime(date.Year, date.Month, date.Day);
            DateTime startOfWeek = today.AddDays(-((int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1));
            return startOfWeek;

        }

        public static DateTime EndOfWeek(this DateTime date)
        {
            DateTime output = date.StartOfWeek().AddDays(6);
            return new DateTime(output.Year, output.Month, output.Day, 0, 0, 0, 0);
        }

        public static DateTime StartOfLastWeek(this DateTime date)
        {
            DateTime output = date.AddDays(-(int)date.DayOfWeek - 7);
            return new DateTime(output.Year, output.Month, output.Day, 0, 0, 0, 0);
        }

        public static DateTime EndOfLastWeek(this DateTime date)
        {
            DateTime output = StartOfLastWeek(date).AddDays(6);
            return new DateTime(output.Year, output.Month, output.Day, 0, 0, 0, 0);
        }

        public static DateTime StartOfNextWeek(this DateTime date)
        {
            DateTime output = date.AddDays(-(int)date.DayOfWeek + 7);
            return new DateTime(output.Year, output.Month, output.Day, 0, 0, 0, 0);
        }

        public static DateTime EndOfNextWeek(this DateTime date)
        {
            DateTime output = StartOfNextWeek(date).AddDays(6);
            return new DateTime(output.Year, output.Month, output.Day, 0, 0, 0, 0);
        }

        public static List<DateTime> GetDays(DateTime? startDate, DateTime? endDate)
        {
          
            var days = new List<DateTime>();
            if (startDate.HasValue && endDate.HasValue)
            {
                while (startDate <= endDate)
				{
					days.Add(startDate.Value);
                    startDate = startDate.Value.AddDays(1);
				}
			}
			return days;
        }

        public static string GetDay(this DateTime date)
        {
			return date.FormatDate("dddd").ToLower();
        }

        public static DateTime GetNextWeekday(DateTime? date = null, DayOfWeek day = DayOfWeek.Monday)
        {
            DateTime result = date.HasValue ? date.Value.AddDays(1) : DateTime.Now.AddDays(1);
            while (result.DayOfWeek != day)
                result = result.AddDays(1);
            return result;
        }

        public static List<DateTime> Days(DateTime? startDate, DateTime? endDate)
        {
            var days = new List<DateTime>();

            if (startDate.HasValue && endDate.HasValue)
            {
                while (startDate <= endDate)
                {
                    days.Add(startDate.Value);
                    startDate = startDate.Value.AddDays(1);
                }
            }

            return days;
        }

        public static List<GroupDateDto> Months(List<string> years)
        {
            var dates = new List<GroupDateDto>();
            foreach (var year in years)
            {
                for (int month = 1; month <= 12; month++)
                {
                    var group = new GroupDateDto();
                    var date = new DateTime(year.ToInt32(), month, 1);
                    group.StartDate = date.FirstDayOfMonth();
                    group.EndDate = date.LastDayOfMonth();
                    dates.Add(group);
                }
            }
            return dates;
        }

        public static bool InRange(this DateTime? dateToCheck, DateTime? startDate, DateTime? endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }

        public static bool InRange(this TimeSpan? timeToCheck, TimeSpan? startTime, TimeSpan? endTime)
        {
            return timeToCheck >= startTime && timeToCheck <= endTime;
        }

        public static bool InRange(this DateTime dateToCheck, List<DateTime?> dates)
        {
            foreach (var date in dates) 
                if (InRange(dateToCheck, date.StartDay(), date.EndOfDay())) return true;
            return false;
        }

        public static string TimeAgo(this DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }
        public static string Age(this DateTime? dateTime)
        {
            if (dateTime == null) return string.Empty;
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime!.Value);
            if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("{0} m/o", timeSpan.Days / 30) :
                    "1 m/o";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("{0} y/o", timeSpan.Days / 365) :
                    "1 y/o";
            }

            return result;
        }
        public static double TimeDifference(this DateTime date) => Math.Abs(DateTime.Now.Subtract(date).TotalMinutes);

        public static List<DateTime> TimeSlots(this DateTime startDate) {
            List<DateTime> slots = new List<DateTime>();
            var day = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, 0);
            DateTime startTime = day.AddHours(8); // 8:00 AM
            DateTime endTime = day.AddHours(17);  // 7:00 PM
            TimeSpan interval = TimeSpan.FromMinutes(30);

            for (DateTime time = startTime; time <= endTime; time += interval)
            {
                slots.Add(time);
            }
            return slots;
        }

        public static List<DateTime> Weekly(this DateTime date)
        {
            List<DateTime> slots = new List<DateTime>();
            var startDate = date.StartOfWeek();
            var endDate = startDate.EndOfWeek();
            while (startDate <= endDate)
            {
                slots.Add(startDate);
                startDate = startDate.AddDays(1);
            }
            return slots;
        }

        public static bool WithinRange(this IEnumerable<DateTime?> dates, DateTime? date, DateTime? time = null)
        {
            if (date == null) return false;
            var hour = time.HasValue ? time.Value.Hour : 0;
            var minute = time.HasValue ? time.Value.Minute : 0;
            var dateWithTime = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, hour, minute, 0, 0);
            foreach (var item in dates)
            {
                if (hour > 0 || minute > 0)
                {
                    if (item >= dateWithTime && item <= dateWithTime) return true;
                }
                else
                {
                    if (item == dateWithTime) return true;
                }
            }
            return false;
        }
     
      

        public static List<List<DateTime?>> CalendarWeeks(this DateTime CurrentMonth)
        {
            var calendarWeeks = new List<List<DateTime?>>();

            DateTime firstDay = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
            int startDayOffset = (int)firstDay.DayOfWeek;

            List<DateTime?> week = new();

            // Fill empty spaces before the first day of the month
            for (int i = 0; i < startDayOffset; i++)
                week.Add(null);

            for (int day = 1; day <= daysInMonth; day++)
            {
                week.Add(new DateTime(CurrentMonth.Year, CurrentMonth.Month, day));

                if (week.Count == 7)
                {
                    calendarWeeks.Add(week);
                    week = new();
                }
            }

            // Fill remaining slots in the last week
            if (week.Count > 0)
                calendarWeeks.Add(week);
            return calendarWeeks;

        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            return startDate;
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            DateTime endOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            return endOfMonth;
        }

        public static int? CalculateAge(DateTime? date)
        {
            if (date == null) return null;
            DateTime today = DateTime.Today;
            int age = today.Year - date.Value.Year;

            // Adjust if birthday hasn't occurred yet this year
            if (date.Value.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

    }
}
