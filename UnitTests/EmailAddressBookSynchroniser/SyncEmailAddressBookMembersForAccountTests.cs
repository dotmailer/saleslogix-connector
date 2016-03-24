using System;
using System.Collections.Generic;
using EmailMarketing.SalesLogix.Entities;
using EmailMarketing.SalesLogix.Tasks;
using Moq;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class SyncEmailAddressBookMembersForAccountTests
    {
        public static EmailAccount GetTestEmailAccount()
        {
            return new EmailAccount()
            {
                AccountName = "UnitTest",
                Id = "EMAILACC9012"
            };
        }

        [Test]
        public void WhenNoAddressBooks_NoSyncRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(new List<EmailAddressBook>());

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            syncer.SyncEmailAddressBookMembers(GetTestEmailAccount(), SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            mockSlx.Verify(fw => fw.UpdateEmailAddressBook(It.IsAny<EmailAddressBook>()), Times.Never());
        }

        [Test]
        public void WhenScheduledAndOnlyManualAddrBook_NoSyncRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            List<EmailAddressBook> testAddrBooks = new List<EmailAddressBook>()
            {
                new EmailAddressBook() { ManualSyncOnly = true}
            };

            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testAddrBooks);

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            syncer.SyncEmailAddressBookMembers(GetTestEmailAccount(), SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            mockSlx.Verify(fw => fw.UpdateEmailAddressBook(It.IsAny<EmailAddressBook>()), Times.Never());
        }

        [Test]
        public void WhenManualAndOnlyManualAddrBook_SyncRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            List<EmailAddressBook> testAddrBooks = new List<EmailAddressBook>()
            {
                new EmailAddressBook() { ManualSyncOnly = true}
            };

            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testAddrBooks);

            var mockSyncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            mockSyncer.Setup(fw => fw.SyncEmailAddressBookMembers(It.IsAny<EmailAccount>(), It.IsAny<EmailAddressBook>(), Tasks.SyncType.Manual, It.IsAny<DateTime>(), false, false));

            mockSyncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), Tasks.SyncType.Manual, new DateTime(2012, 3, 4), false, false);

            mockSyncer.Verify(
                fw => fw.SyncEmailAddressBookMembers(It.IsAny<EmailAccount>(), It.IsAny<EmailAddressBook>(), Tasks.SyncType.Manual, It.IsAny<DateTime>(), false, false),
                Times.Once());
        }

        [Test]
        public void When2AddrBooks_2SyncsRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            List<EmailAddressBook> testAddrBooks = new List<EmailAddressBook>()
            {
                new EmailAddressBook() { ManualSyncOnly = true},
                new EmailAddressBook() { ManualSyncOnly = true}
            };

            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testAddrBooks);

            var mockSyncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            mockSyncer.Setup(fw => fw.SyncEmailAddressBookMembers(It.IsAny<EmailAccount>(), It.IsAny<EmailAddressBook>(), Tasks.SyncType.Manual, It.IsAny<DateTime>(), false, false));

            mockSyncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), Tasks.SyncType.Manual, new DateTime(2012, 3, 4), false, false);

            mockSyncer.Verify(
                fw => fw.SyncEmailAddressBookMembers(It.IsAny<EmailAccount>(), It.IsAny<EmailAddressBook>(), Tasks.SyncType.Manual, It.IsAny<DateTime>(), false, false),
                Times.Exactly(2));
        }
    }
}