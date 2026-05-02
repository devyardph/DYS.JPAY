using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Platforms.Windows
{
    public static class BackgroundService
    {
        private const string TaskName = "com.devyard.jpay.dailyemailreport";
        private const string TaskFolder = "devyard";

        public static void RegisterDailyEmailTask()
        {
            using var ts = new TaskService();

            // Create or update the task
            var td = ts.NewTask();
            td.RegistrationInfo.Description = "Sends the daily email report at 6 PM";
            td.Settings.StartWhenAvailable = true; // run if missed (e.g. PC was off)
            td.Settings.RunOnlyIfNetworkAvailable = true;
            td.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;

            // Trigger: daily at 5:00 PM
            var trigger = new DailyTrigger
            {
                StartBoundary = DateTime.Today.AddHours(2).AddMinutes(10),
                DaysInterval = 1,
                Enabled = true
            };
            td.Triggers.Add(trigger);

            // Action: launch this app with a special argument
            var exePath = Environment.ProcessPath!;
            td.Actions.Add(new ExecAction(exePath, "--send-daily-report", null));

            // Register under a folder (creates it if needed)
            ts.GetFolder("\\").CreateFolder(TaskFolder, exceptionOnExists: false);
            ts.GetFolder($"\\{TaskFolder}").RegisterTaskDefinition(TaskName, td);

            System.Diagnostics.Debug.WriteLine("[TaskScheduler] Daily email task registered.");
        }

        public static void UnregisterDailyEmailTask()
        {
            using var ts = new TaskService();
            try
            {
                ts.GetFolder($"\\{TaskFolder}").DeleteTask(TaskName);
            }
            catch { /* task didn't exist */ }
        }
    }
}
