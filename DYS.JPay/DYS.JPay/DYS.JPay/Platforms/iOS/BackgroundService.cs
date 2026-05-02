using BackgroundTasks;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Services
{
    public class BackgroundService
    {
        public static void ScheduleNextDailyEmail()
        {
#if IOS
        var request = new BGAppRefreshTaskRequest("com.devyard.jpay.dailyemailreport")
        {
            // Target 5:00 PM today (or tomorrow if past 5 PM)
            EarliestBeginDate = GetNextEndOfDay()
        };

        NSError? error;
        BGTaskScheduler.Shared.Submit(request, out error);

        if (error != null)
            Console.WriteLine($"BGTask schedule error: {error.LocalizedDescription}");
#endif
        }

        private static NSDate GetNextEndOfDay()
        {
            var now = DateTime.Now;
            var targetToday = now.Date.AddHours(18); // 6 PM

            var target = now < targetToday ? targetToday : targetToday.AddDays(1);
            return (NSDate)target;
        }
    }
}
