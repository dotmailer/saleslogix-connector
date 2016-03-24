using System;
using System.Collections.Generic;
using System.Linq;
using EmailMarketing.SalesLogix.Entities;
using Moq;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class SyncEmailAddressBookMembersForAddrBookTests
    {
        public static EmailAccount GetTestEmailAccount()
        {
            return new EmailAccount()
            {
                AccountName = "UnitTest",
                Id = "EMAILACC9012"
            };
        }

        public static EmailAddressBook GetTestAddrBook()
        {
            return new EmailAddressBook()
            {
                Id = "ADDRBOOK9012",
                EmailServiceAddressBookId = 654123
            };
        }

        public static DeletedItem GetDeletedAddressBookMember()
        {
            return new DeletedItem()
            {
                EntityType = EmailAddressBookMember.EntityName,
                Data = "<DeletedMemberDetails><EmailAddress>test@example.com</EmailAddress></DeletedMemberDetails>"
            };
        }

        [Test]
        public void WhenRun_SyncStatusUpdated()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), GetTestAddrBook(), Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            mockSlx.Verify(fw => fw.UpdateEmailAddressBook(It.IsAny<EmailAddressBook>()), Times.Exactly(2));
        }

        [Test]
        public void WhenProgressObjectTooShort_InvalidOperationException()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            var book = GetTestAddrBook();
            book.MemberSyncStatus = EmailAddressBookSynchroniser.StatusImportInProgress;
            book.MemberSyncProgressObject = "tooshort";
            Assert.Throws<InvalidOperationException>(() => syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), book, Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false));
        }

        [Test]
        public void WhenImportStillInProgress_SyncNotRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            var book = GetTestAddrBook();
            book.MemberSyncStatus = EmailAddressBookSynchroniser.StatusImportInProgress;
            book.MemberSyncProgressObject = "{CA4C3F95-9D3E-49EC-A94C-A29151F0A3CD}";
            syncer.SyncEmailAddressBookMembers(GetTestEmailAccount(), book, Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            mockSlx.Verify(fw => fw.UpdateEmailAddressBook(It.IsAny<EmailAddressBook>()), Times.Never());
        }

        [Test]
        public void WhenInProgressImportIsNowComplete_MembersHashcodeUpdated()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            mockDm.Setup(fw => fw.IsImportComplete(It.IsAny<string>())).Returns(true);
            List<EmailAddressBookMember> testeabmList = new List<EmailAddressBookMember>()
            {
                new EmailAddressBookMember() {PendingImporthashCode = 123456}
            };
            mockSlx.Setup(fw => fw.GetEmailAddressBookMembersFromImport(It.IsAny<string>(), It.IsAny<Guid>())).Returns(testeabmList);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;
            var book = GetTestAddrBook();
            book.MemberSyncStatus = EmailAddressBookSynchroniser.StatusImportInProgress;
            book.MemberSyncProgressObject = "{CA4C3F95-9D3E-49EC-A94C-A29151F0A3CD}";

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), book, Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            Assert.That(testeabmList[0].LastSyncHashCode, Is.EqualTo(123456));
        }

        [Test]
        public void WhenInProgressImportIsNowComplete_SyncRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            mockDm.Setup(fw => fw.IsImportComplete(It.IsAny<string>())).Returns(true);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;
            var book = GetTestAddrBook();
            book.MemberSyncStatus = EmailAddressBookSynchroniser.StatusImportInProgress;
            book.MemberSyncProgressObject = "{CA4C3F95-9D3E-49EC-A94C-A29151F0A3CD}";

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), book, Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            mockSlx.Verify(fw => fw.UpdateEmailAddressBook(It.IsAny<EmailAddressBook>()), Times.Exactly(2));
        }

        [Test]
        public void When1DeletedMember_DeleteRunForMember()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testDeletedList = new List<DeletedItem>()
            {
                GetDeletedAddressBookMember()
            };
            mockSlx.Setup(fw => fw.GetDeletedItemsUnprocessed(It.IsAny<string>(), It.IsAny<string>())).Returns(testDeletedList);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), GetTestAddrBook(), Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            mockDm.Verify(fw => fw.DeleteAddressBookContacts(
                It.Is<int>(n => n == GetTestAddrBook().EmailServiceAddressBookId),
                It.Is<ICollection<string>>(c => c.Count == 1))
                , Times.Once());
        }

        [Test]
        public void When3DeletedMembers_3ItemsDeletedInBatch()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testDeletedList = new List<DeletedItem>()
            {
                GetDeletedAddressBookMember(),
                GetDeletedAddressBookMember(),
                GetDeletedAddressBookMember()
            };
            mockSlx.Setup(fw => fw.GetDeletedItemsUnprocessed(It.IsAny<string>(), It.IsAny<string>())).Returns(testDeletedList);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), GetTestAddrBook(), Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            mockDm.Verify(fw => fw.DeleteAddressBookContacts(
                It.Is<int>(n => n == GetTestAddrBook().EmailServiceAddressBookId),
                It.Is<ICollection<string>>(c => c.Count == 3)), Times.Once());
        }

        [Test]
        public void When1NewContactMember_1MemberAdded()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            EmailAddressBookMember testABM = new EmailAddressBookMember()
            {
                Contact = new Sage.SData.Client.Extensions.SDataPayload(),
                SlxMemberType = "Contact"
            };
            testABM.Contact.Values.Add("Email", "testemail@example.com");
            var testMemberList = new List<EmailAddressBookMember>()
            {
                testABM
            };

            mockSlx.Setup(fw => fw.GetEmailAddressBookMembersAddedBetween(It.IsAny<string>(), It.Is<DateTime?>(d => true), It.IsAny<DateTime>(), It.IsAny<Dictionary<string, List<string>>>())).Returns(testMemberList);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), GetTestAddrBook(), Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            bool boolOut = true;
            string icrOut = null;
            mockDm.Verify(fw => fw.ImportContactsIntoAddressBook(It.IsAny<int>(), It.Is<IEnumerable<EmailServiceContact>>(e => e.Count() == 1), out boolOut, out icrOut), Times.Once());
        }

        [Test]
        public void When1ContactMemberModified_1MemberUpdated()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            EmailAddressBookMember testABM = new EmailAddressBookMember()
            {
                Contact = new Sage.SData.Client.Extensions.SDataPayload(),
                SlxMemberType = "Contact",
                LastSyncedEmailAddress = "testemail@example.com",
                LastSyncHashCode = 1111111,
                SlxContactId = "C23456789012"
            };
            testABM.Contact.Values.Add("Email", "testemail@example.com");
            var testMemberList = new List<EmailAddressBookMember>()
            {
                testABM
            };

            mockSlx.Setup(fw => fw.GetEmailAddressBookMembersModifiedBetween(It.IsAny<string>(), It.Is<DateTime?>(d => true), It.IsAny<DateTime>(), It.IsAny<Dictionary<string, List<string>>>())).Returns(testMemberList);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), GetTestAddrBook(), Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            bool boolOut = true;
            string icrOut = null;
            mockDm.Verify(fw => fw.DeleteAddressBookContact(It.IsAny<int>(), It.IsAny<string>()), Times.Never());
            mockDm.Verify(fw => fw.ImportContactsIntoAddressBook(It.IsAny<int>(), It.Is<IEnumerable<EmailServiceContact>>(e => e.Count() == 1), out boolOut, out icrOut), Times.Once());
        }

        [Test]
        public void When1MemberModifiedButHashTheSame_MemberNotUpdated()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            EmailAddressBookMember testABM = new EmailAddressBookMember()
            {
                Contact = new Sage.SData.Client.Extensions.SDataPayload(),
                SlxMemberType = "Contact",
                LastSyncedEmailAddress = "testemail@example.com",
                LastSyncHashCode = 17 * 31 + "testemail@example.com".GetHashCode()
            };
            testABM.Contact.Values.Add("Email", "testemail@example.com");
            var testMemberList = new List<EmailAddressBookMember>()
            {
                testABM
            };

            mockSlx.Setup(fw => fw.GetEmailAddressBookMembersModifiedBetween(It.IsAny<string>(), It.Is<DateTime?>(d => true), It.IsAny<DateTime>(), It.IsAny<Dictionary<string, List<string>>>())).Returns(testMemberList);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), GetTestAddrBook(), Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            bool boolOut = true;
            string icrOut = null;
            mockDm.Verify(fw => fw.DeleteAddressBookContact(It.IsAny<int>(), It.IsAny<string>()), Times.Never());
            mockDm.Verify(fw => fw.ImportContactsIntoAddressBook(It.IsAny<int>(), It.IsAny<IEnumerable<EmailServiceContact>>(), out boolOut, out icrOut), Times.Never());
        }

        [Test]
        public void When1EmailAddrModified_1MemberDeletedAndUpdated()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            EmailAddressBookMember testABM = new EmailAddressBookMember()
            {
                Contact = new Sage.SData.Client.Extensions.SDataPayload(),
                SlxMemberType = "Contact",
                LastSyncedEmailAddress = "testDIFFERENTemail@example.com",
                SlxContactId = "C23456789012"
            };
            testABM.Contact.Values.Add("Email", "testemail@example.com");
            var testMemberList = new List<EmailAddressBookMember>()
            {
                testABM
            };

            mockSlx.Setup(fw => fw.GetEmailAddressBookMembersModifiedBetween(It.IsAny<string>(), It.Is<DateTime?>(d => true), It.IsAny<DateTime>(), It.IsAny<Dictionary<string, List<string>>>())).Returns(testMemberList);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), GetTestAddrBook(), Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), false, false);

            bool boolOut = true;
            string icrOut = null;
            mockDm.Verify(fw => fw.DeleteAddressBookContacts(It.IsAny<int>(), It.Is<ICollection<string>>(c => c.Count == 1)), Times.Once());
            mockDm.Verify(fw => fw.ImportContactsIntoAddressBook(It.IsAny<int>(), It.Is<IEnumerable<EmailServiceContact>>(e => e.Count() == 1), out boolOut, out icrOut), Times.Once());
        }

        [Test]
        public void WhenReloadAll_ModifyQueryDateIsMinimum()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testMemberList = new List<EmailAddressBookMember>();

            mockSlx.Setup(fw => fw.GetEmailAddressBookMembersModifiedBetween(It.IsAny<string>(), It.Is<DateTime?>(d => true), It.IsAny<DateTime>(), It.IsAny<Dictionary<string, List<string>>>())).Returns(testMemberList);

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            bool shouldReturn = false;
            syncer.Setup(fw => fw.VerifyDotMailerAddrBook(It.IsAny<EmailAddressBook>(), out shouldReturn));
            syncer.CallBase = true;

            syncer.Object.SyncEmailAddressBookMembers(GetTestEmailAccount(), GetTestAddrBook(), Tasks.SyncType.Scheduled, new DateTime(2012, 3, 4), true, false);

            mockDm.Verify(fw => fw.DeleteAddressBookContact(It.IsAny<int>(), It.IsAny<string>()), Times.Never());
            mockSlx.Verify(fw => fw.GetEmailAddressBookMembersModifiedBetween(It.IsAny<string>(), It.Is<DateTime?>(d => d == SlxSdata.MinimumDateTimeValue), It.IsAny<DateTime>(), It.IsAny<Dictionary<string, List<string>>>()), Times.Once());
        }
    }
}