namespace EmailMarketing.SalesLogix.Tasks
{
    using System;

    /// <summary>
    /// A task that can be run by the scheduler
    /// </summary>
    public interface IScheduledTask
    {
        /// <summary>The name of the task.  Must be unique within the task list.</summary>
        string TaskName { get; }

        /// <summary>Gets when this task was last run</summary>
        DateTime LastRunAtUtc { get; }

        /// <summary>Gets or sets the time between runs of the task</summary>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Polls the task.  If the task deems that is is due to run then it will be run
        /// </summary>
        /// <returns>Whether the task was run</returns>
        bool RunTaskIfItIsDueToRun();
    }
}