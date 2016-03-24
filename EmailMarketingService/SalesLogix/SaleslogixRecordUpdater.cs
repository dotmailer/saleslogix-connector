using System;
using System.Collections.Generic;
using EmailMarketing.SalesLogix.Entities;

namespace EmailMarketing.SalesLogix
{
    /// <summary>
    /// A class to assist with updating records in Saleslogix.  It can either use batching or individual record uploads.  Where batch uploading is used uploading is delayed until enough have accumulated in memory.
    /// This class implements IDisposable.
    /// </summary>
    /// <typeparam name="T">The record type (a Saleslogix entity type)</typeparam>
    public class SaleslogixRecordUpdater<T> : IDisposable where T : Entity
    {
        private const int InitialBatchSize = 100;
        private readonly ISlxConnector _saleslogixConnector;
        private List<T> _recordsWaiting = new List<T>(InitialBatchSize);
        private int _batchSize = InitialBatchSize;
        private bool _useBatchUpdatingMethod = true;

        public SaleslogixRecordUpdater(ISlxConnector saleslogixConnector)
        {
            _saleslogixConnector = saleslogixConnector;
        }

        /// <summary>
        /// Applies only when using the batch updating method.  Indicates the number of records that must accumulate in memory before a batch update operation is triggered and records are sent to the Saleslogix server.
        /// </summary>
        public int BatchSize
        {
            get
            {
                return _batchSize;
            }
            set
            {
                _batchSize = value;
                UploadWaitingRecordsIfNecessary();
            }
        }

        /// <summary>
        /// If false, then each record is updated in Saleslogix individually and immediately.  If true, then the update is delayed according to the BatchSize property's value.
        /// </summary>
        public bool UseBatchUpdatingMethod
        {
            get
            {
                return _useBatchUpdatingMethod;
            }
            set
            {
                _useBatchUpdatingMethod = value;
                UploadWaitingRecordsIfNecessary();
            }
        }

        /// <summary>
        /// Requests a record to be updated on the Saleslogix server.  The update is only immediate if UseBatchUpdatingMethod is set to false.
        /// </summary>
        /// <param name="record"></param>
        public void Update(T record)
        {
            _recordsWaiting.Add(record);
            UploadWaitingRecordsIfNecessary();
        }

        private void UploadWaitingRecordsIfNecessary()
        {
            if ((_recordsWaiting.Count >= _batchSize) || (!UseBatchUpdatingMethod))
                UpdateWaitingRecords();
        }

        private void UpdateWaitingRecords()
        {
            List<T> recordsToUpdate = _recordsWaiting;
            _recordsWaiting = new List<T>(_batchSize);

            if (UseBatchUpdatingMethod)
                _saleslogixConnector.BatchUpdateRecords(recordsToUpdate, false);
            else
            {
                foreach (T record in recordsToUpdate)
                    _saleslogixConnector.UpdateRecord(record);
            }
        }

        public void Dispose()
        {
            UpdateWaitingRecords();
        }
    }
}