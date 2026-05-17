using BackgroundTasks;
using DYS.JPay.Shared.Shared.Services;
using Foundation;
using UIKit;

namespace DYS.JPay
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
        {
            BGTaskScheduler.Shared.Register("com.devyard.jpay.dailyemailreport", null, HandleDailyEmailTask);
            return base.FinishedLaunching(application, launchOptions);
        }

        private void HandleDailyEmailTask(BGTask task)
        {
            // Cast to BGProcessingTask if needed
            DYS.JPay.Services.BackgroundService.ScheduleNextDailyEmail(); // always reschedule first

            var processingTask = task as BGProcessingTask;
            var cancelToken = new CancellationTokenSource();
            task.ExpirationHandler = () => cancelToken.Cancel();
            // Run your export/email logic here
            Task.Run(async () =>
            {
                try
                {
                    var emailService = IPlatformApplication.Current!
                      .Services.GetRequiredService<ISchedulerService>();

                    await emailService.RunDailyExport(cancelToken.Token);
                    task.SetTaskCompleted(success: true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"BGTask failed: {ex.Message}");
                    task.SetTaskCompleted(success: false);
                }
            });
        }

        public override void DidEnterBackground(UIApplication application)
        {
            base.DidEnterBackground(application);
            DYS.JPay.Services.BackgroundService.ScheduleNextDailyEmail();
        }
    }
}
