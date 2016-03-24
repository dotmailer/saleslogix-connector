namespace EmailMarketing.SalesLogix.Tasks
{
    using System;

    /// <summary>
    /// A Task that can be run
    /// </summary>
    internal interface IRunnable
    {
        /// <summary>
        /// Run the IRunnable task
        /// </summary>
        /// <param name="lastRunAtUtc">The time the task was last run at</param>
        void Run(DateTime lastRunAtUtc);
    }
}