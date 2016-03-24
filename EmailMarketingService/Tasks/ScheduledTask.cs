using System.Reflection;

namespace EmailMarketing.SalesLogix.Tasks
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using log4net;

    /// <summary>
    /// A task that can be scheduled by the TaskScheduler
    /// </summary>
    internal class ScheduledTask : IScheduledTask
    {
        /// <summary>An object used to prevent any Scheduled Task from running when one is currently in progress</summary>
        private static readonly object threadLocker = new object();

        /// <summary>log4net logger object</summary>
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the ScheduledTask class
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="runnableTask">The task to run</param>
        /// <param name="interval">The time interval between runs of the task</param>
        /// <param name="lastRunAtUtc">The time the task was last run at</param>
        internal ScheduledTask(string taskName, IRunnable runnableTask, TimeSpan interval, DateTime lastRunAtUtc)
        {
            Contract.Requires(runnableTask != null);
            Contract.Requires(interval != null);

            TaskName = taskName;
            Interval = interval;
            LastRunAtUtc = lastRunAtUtc;
            RunnableTask = runnableTask;
        }

        /// <summary>The name of the task.  Must be unique within the task list.</summary>
        public string TaskName { get; set; }

        /// <summary>Gets or sets the time when the task was last run</summary>
        public DateTime LastRunAtUtc { get; set; }

        /// <summary>Gets or sets the interval between runs of the task</summary>
        public TimeSpan Interval { get; set; }

        /// <summary>Gets or sets a runnable object that will be run when the task is run.</summary>
        public IRunnable RunnableTask { get; set; }

        /// <summary>
        /// Checks to see if the task is due to be run, and runs it if necessary.
        /// </summary>
        /// <returns>Whether the task was run</returns>
        public bool RunTaskIfItIsDueToRun()
        {
            bool lockAquired = false;
            bool taskWasStarted = false;
            try
            {
                // Only allow one thread at a time to be running a task, but do
                // not block the second thread.
                Monitor.TryEnter(threadLocker, ref lockAquired);
                if (lockAquired)
                {
                    bool taskDueToRun = DateTime.UtcNow - LastRunAtUtc > Interval;
                    if (taskDueToRun)
                    {
                        taskWasStarted = true;
                        RunnableTask.Run(LastRunAtUtc);
                    }
                }
            }
            finally
            {
                if (taskWasStarted)
                {
                    // Make sure the time is updated, even if an exception was thrown
                    LastRunAtUtc = DateTime.UtcNow;
                }

                if (lockAquired)
                    Monitor.Exit(threadLocker);
            }

            return taskWasStarted;
        }
    }
}