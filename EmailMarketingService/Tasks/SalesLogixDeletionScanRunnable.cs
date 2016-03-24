using System.Reflection;
using Sage.SData.Client.Atom;

namespace EmailMarketing.SalesLogix.Tasks
{
    using System;
    using System.Collections.Generic;
    using Entities;
    using log4net;
    using Sage.SData.Client.Core;
    using Sage.SData.Client.Extensions;

    public class SalesLogixDeletionScanRunnable : IRunnable
    {
        /// <summary>log4net logger object</summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Run(DateTime lastRunAtUtc)
        {
            try
            {
                Logger.Debug("Checking if deletion scan is enabled");

                ISlxConnector slx = ObjectFactory.Instance.GetSlxConnector();

                Settings settings = ObjectFactory.Instance.Settings;
                if (settings.EnableDeletedItemsScan)
                {
                    Logger.Info("Scheduled scan for deletions started.");
                    IEnumerable<EmailAddressBookMember> allAddressBookMembers = slx.GetAllAddressBookMembers();
                    RunForAddressBookMembers(allAddressBookMembers);
                }
                else
                    Logger.Debug("Scheduled deletion scan is not enabled.");
            }
            catch (SDataClientException ex)
            {
                LoggingAction actionCompleted = SDataClientExceptionLogger.OutputToLog(ex, ObjectFactory.Instance.Settings.SdataUrl, Logger);
                if (actionCompleted == LoggingAction.ErrorLogged)
                    return;
                throw;
            }
        }

        public static void RunForAddressBook(ISlxConnector slxConnector, string emailAddressBookId)
        {
            DateTime startTime = DateTime.Now;
            Logger.InfoFormat("Starting cleaning-up deleted items in address book ID {0}, process starting at {1}.", emailAddressBookId, startTime);
            IEnumerable<EmailAddressBookMember> members = slxConnector.GetEnumerableAddressBookMembers(emailAddressBookId);
            RunForAddressBookMembers(members);
            DateTime endTime = DateTime.Now;
            Logger.InfoFormat("Finished cleaning-up deleted items in address book ID {0}, finished at {1}, time taken = {2}.", emailAddressBookId, endTime, endTime - startTime);
        }

        public static void RunForAllAddressBooks(ISlxConnector slxConnector)
        {
            IEnumerable<EmailAddressBookMember> members = slxConnector.GetAllAddressBookMembers();
            RunForAddressBookMembers(members);
        }

        public static void RunForAddressBookMembers(IEnumerable<EmailAddressBookMember> members)
        {
            try
            {
                ISlxConnector slx = ObjectFactory.Instance.GetSlxConnector();

                // Look for Email Address Book Members whose Lead/Contact has been deleted.
                // Deletions would normally be picked up by the 'OnDelete' rule in SalesLogix Web,
                // but if Leads/Contacts are deleted through SQL (e.g. via an external process)
                // then the OnDelete rule will not be run, leaving the system in an inconsistent state.
                List<EmailAddressBookMember> faultedMembers = new List<EmailAddressBookMember>();
                int numRecordsScanned = 0;
                foreach (EmailAddressBookMember member in members)
                {
                    // The SLX Lead/Contact ID is held in the entity.  There is also a 'link' that we do not
                    // use in the entity, but we can get hold of it through the sdata payload.
                    // We are looking for records that have an ID, but the link is null (because the lead/contact no longer exists)
                    if (member != null)
                    {
                        bool isFaulted = false;
                        var payload = ((AtomEntry)member.SourceData).GetSDataPayload();
                        if (!string.IsNullOrWhiteSpace(member.SlxLeadId) && payload.Values["Lead"] == null)
                        {
                            // Lead link is no longer valid, so null it
                            Logger.DebugFormat("Lead link ({0}) is invalid for EmailAddressBookMember ({1})",
                                member.SlxLeadId, member.Id);
                            member.SlxLeadId = null;
                            payload.Values.Remove("Lead");
                            isFaulted = true;
                        }

                        if (!string.IsNullOrWhiteSpace(member.SlxContactId) && payload.Values["Contact"] == null)
                        {
                            // Contact link is no longer valid, so null it
                            Logger.DebugFormat("Contact link ({0}) is invalid for EmailAddressBookMember ({1})",
                                member.SlxContactId, member.Id);
                            member.SlxContactId = null;
                            isFaulted = true;
                        }

                        if (isFaulted)
                        {
                            Logger.DebugFormat(
                                "Batching EmailAddressBookMember ({0}) with broken link to be updated to null",
                                member.Id);
                            faultedMembers.Add(member);
                        }

                        numRecordsScanned++;
                        if (numRecordsScanned % 1000 == 0)
                        {
                            Logger.DebugFormat("{0} records scanned", numRecordsScanned);
                        }
                    }
                }

                if (faultedMembers.Count > 0)
                {
                    Logger.InfoFormat("Batch updating ({0}) Email Address Book Members with broken Lead/Contact links",
                        faultedMembers.Count);
                    slx.BatchUpdateRecords<EmailAddressBookMember>(faultedMembers);
                }

                Logger.InfoFormat("Scan for deletions complete");
            }
            catch (SDataClientException ex)
            {
                LoggingAction actionCompleted = SDataClientExceptionLogger.OutputToLog(ex, ObjectFactory.Instance.Settings.SdataUrl, Logger);
                if (actionCompleted == LoggingAction.ErrorLogged)
                    return;
                throw;
            }
        }
    }
}