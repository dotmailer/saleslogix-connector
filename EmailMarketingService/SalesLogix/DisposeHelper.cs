//using System;

//namespace EmailMarketing.SalesLogix
//{
//    public abstract class DisposeHelper : IDisposable
//    {
//        //Flag to indicate whether the object has been disposed.
//        protected bool Disposed;

//        //------------------------------------------------------------
//        /// <summary>
//        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//        /// </summary>
//        protected virtual void Dispose(bool disposing)
//        {
//            if (!Disposed)
//            {
//                if (disposing)
//                {
//                    // Dispose managed resources.
//                    DisposeManagedResources();
//                }

//                Disposed = true;
//            }
//        }

//        //------------------------------------------------------------
//        /// <summary>
//        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//        /// </summary>
//        /// <filterpriority>2</filterpriority>
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        //------------------------------------------------------------
//        // Virtual methods
//        // defaults implementations do nothing

//        /// <summary>
//        /// Called when managed resources should be disposed
//        /// </summary>
//        protected virtual void DisposeManagedResources() { }
//    }
//}