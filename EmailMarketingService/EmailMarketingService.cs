using System.Reflection;
using EmailMarketing.SalesLogix.Tasks;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Threading;
    using log4net;

    public class EmailMarketingService : IDisposable
    {
        /// <summary>log4net logger object</summary>
        private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Responsible for running scheduled tasks</summary>
        private ScheduledTaskRunner _taskRunner;

        /// <summary>
        /// Start the Scheduled task runner
        /// </summary>
        public void Start()
        {
            // Perform the start on a separate thread so the service starts quickly enough that the
            // service manager is happy.
            ThreadPool.QueueUserWorkItem(ThreadStart);
        }

        /// <summary>
        /// Stop the Scheduled task runner and Web Service
        /// </summary>
        public void Stop()
        {
            if (_taskRunner != null && _taskRunner.Status == TaskRunnerStatus.Started)
                _taskRunner.Stop();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_taskRunner != null)
                {
                    _taskRunner.Dispose();
                    _taskRunner = null;
                }
        }

        ~EmailMarketingService()
        {
            Dispose(false);
        }

        /// <summary>
        /// Code for starting the service.
        /// </summary>
        /// <param name="data">not used</param>
        private void ThreadStart(object data)
        {
            ObjectFactory factory = ObjectFactory.Instance;

            ObjectFactory.Instance.ConfigurationChanged += InstanceConfigurationChanged;

            _taskRunner = factory.GetScheduledTaskRunner();
            _taskRunner.Start();
        }

        private void InstanceConfigurationChanged(object sender, EventArgs e)
        {
            ObjectFactory.Instance.RefreshTaskSchedules(_taskRunner.ScheduledTasks);
        }
    }
}