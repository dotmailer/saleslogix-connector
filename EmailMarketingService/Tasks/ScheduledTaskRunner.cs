using System.Reflection;
using System.Timers;

namespace EmailMarketing.SalesLogix.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    //using System.Linq;
    using log4net;

    /// <summary>
    /// Controls the scheduling of tasks
    /// </summary>
    /// <remarks>The scheduler uses a multi-threaded timer to trigger the polling of tasks.
    /// The PollTask() method on tasks must be able to handle being run multiple times
    /// on different threads.  This can happen if the task runs for longer than the
    /// scheduler interval.</remarks>
    internal class ScheduledTaskRunner : IDisposable
    {
        /// <summary>log4net logger object</summary>
        private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Regularly kicks off the polling of tasks</summary>
        private System.Timers.Timer _schedulerTimer;

        /// <summary>The time in milliseconds between polling for tasks that are due.</summary>
        private double _schedulerPollIntervalInMs;

        /// <summary>
        /// Initializes a new instance of the TaskRunner class.
        /// </summary>
        /// <param name="tasksToSchedule">A non-null list of tasks to schedule.</param>
        /// <param name="schedulerPollIntervalInMs">The time in milliseconds between polling for tasks that are due.</param>
        public ScheduledTaskRunner(List<IScheduledTask> tasksToSchedule, double schedulerPollIntervalInMs = 500)
        {
            Contract.Requires(tasksToSchedule != null);

            HashSet<string> tempSet = new HashSet<string>();
            foreach (var task in tasksToSchedule)
            {
                if (task.TaskName == null)
                    throw new ArgumentException("All tasks must have a Task Name", "tasksToSchedule");

                if (tempSet.Contains(task.TaskName))
                    throw new ArgumentException(string.Format("Task Names must be unique within the list of tasks to schedule ({0})", task.TaskName), "tasksToSchedule");
                tempSet.Add(task.TaskName);
            }

            _schedulerTimer = new System.Timers.Timer();
            SchedulerPollIntervalInMs = schedulerPollIntervalInMs;

            ScheduledTasks = tasksToSchedule;
        }

        /// <summary>
        /// Gets or sets the interval at which the scheduler polls tasks to see if they are due to run.
        /// Can be updated on the fly by changing this property.
        /// </summary>
        public double SchedulerPollIntervalInMs
        {
            get
            {
                return _schedulerPollIntervalInMs;
            }

            set
            {
                if (value != _schedulerPollIntervalInMs)
                {
                    _schedulerPollIntervalInMs = value;
                    _schedulerTimer.Interval = value;
                }
            }
        }

        /// <summary>Gets a list of tasks that this scheduler is due to run</summary>
        public List<IScheduledTask> ScheduledTasks { get; private set; }

        /// <summary>Gets the status of the task runner</summary>
        public TaskRunnerStatus Status { get; private set; }

        /// <summary>
        /// Start the scheduler
        /// </summary>
        public void Start()
        {
            _schedulerTimer.Elapsed += SchedulerTimerElapsed;
            _schedulerTimer.Start();
            Status = TaskRunnerStatus.Started;
        }

        /// <summary>
        /// Stop the scheduler
        /// </summary>
        public void Stop()
        {
            _schedulerTimer.Stop();
            Status = TaskRunnerStatus.Stopped;
        }

        /// <summary>
        /// Dispose of external resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose of external resources
        /// </summary>
        /// <param name="disposing">Is a disposal currently in progress</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_schedulerTimer != null)
                {
                    _schedulerTimer.Dispose();
                    _schedulerTimer = null;
                }
            }
        }

        /// <summary>
        /// Steps through each of the scheduled tasks, running each if it is due to be run.
        /// </summary>
        /// <param name="sender">The control that fired the event</param>
        /// <param name="e">Timer event args</param>
        private void SchedulerTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //Note: I believe that while a task is being processed, this method will run through over and over,
            //achieving nothing.  No harm (probably), but a bit strange.

            //"The Timer component catches and suppresses all exceptions thrown by event handlers for the Elapsed event.
            //This behavior is subject to change in future releases of the .NET Framework."
            //http://msdn.microsoft.com/en-us/library/system.timers.timer.elapsed(v=vs.110).aspx
            //There is no point re-throwing exceptions in this method, as System.Timers.Timer suppresses/swallows them anyway.
            //Also, "subject to change" = prepare for unexpected behaviour in a future .NET release.  No thanks.

            //logger.Debug("Scheduler timer elapsed!");
            foreach (IScheduledTask task in ScheduledTasks)
            {
                try
                {
                    task.RunTaskIfItIsDueToRun();
                }
                catch (Exception ex)
                {
                    logger.Error("An unhandled exception occurred in a scheduled task of type " + task.GetType().Name + ".", ex);
                }
            }
        }
    }
}