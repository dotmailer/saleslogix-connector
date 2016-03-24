using System;
using System.Collections.Generic;
using EmailMarketing.SalesLogix.Entities;
using EmailMarketing.SalesLogix.Tasks;
using Moq;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class SyncEmailAddressBookHeaders
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
        public void WhenNoSlxAddressBooks_NoSyncRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(new List<EmailAddressBook>());

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            syncer.SyncEmailAddressBookHeaders(GetTestEmailAccount(), SyncType.Scheduled, new DateTime(2012, 3, 4));

            mockDm.Verify(fw => fw.CreateAddressBook(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void When1SlxBookAlreadySynced_NoSyncRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testBooks = new List<EmailAddressBook>()
                {
                    new EmailAddressBook()
                    {
                        EmailServiceAddressBookId = 12345
                    }
                };
            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testBooks);

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            syncer.SyncEmailAddressBookHeaders(GetTestEmailAccount(), SyncType.Scheduled, new DateTime(2012, 3, 4));

            mockDm.Verify(fw => fw.CreateAddressBook(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void When1SlxBookNotSynced_CreatedInDm()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testBooks = new List<EmailAddressBook>()
                {
                    new EmailAddressBook()
                    {
                        Name = "TestAbName"
                    }
                };
            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testBooks);

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            syncer.SyncEmailAddressBookHeaders(GetTestEmailAccount(), SyncType.Scheduled, new DateTime(2012, 3, 4));

            mockDm.Verify(fw => fw.CreateAddressBook(It.IsRegex("TestAbName")), Times.Once());
        }

        [Test]
        public void When3SlxBooksNotSynced_3CreatedInDm()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testBooks = new List<EmailAddressBook>()
                {
                    new EmailAddressBook() { Name = "TestAbName1" },
                    new EmailAddressBook() { Name = "TestAbName2" },
                    new EmailAddressBook() { Name = "TestAbName3" }
                };
            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testBooks);

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            syncer.SyncEmailAddressBookHeaders(GetTestEmailAccount(), SyncType.Scheduled, new DateTime(2012, 3, 4));

            mockDm.Verify(fw => fw.CreateAddressBook(It.IsRegex("TestAbName1")), Times.Once());
            mockDm.Verify(fw => fw.CreateAddressBook(It.IsRegex("TestAbName2")), Times.Once());
            mockDm.Verify(fw => fw.CreateAddressBook(It.IsRegex("TestAbName3")), Times.Once());
        }

        [Test]
        public void WhenScheduledSyncAnd1SlxBookIsManualOnly_NoSyncRun()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testBooks = new List<EmailAddressBook>()
                {
                    new EmailAddressBook()
                    {
                        ManualSyncOnly = true
                    }
                };
            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testBooks);

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            syncer.SyncEmailAddressBookHeaders(GetTestEmailAccount(), SyncType.Scheduled, new DateTime(2012, 3, 4));

            mockDm.Verify(fw => fw.CreateAddressBook(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void WhenManualSyncAnd1SlxBookIsManualOnly_CreatedInDm()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testBooks = new List<EmailAddressBook>()
                {
                    new EmailAddressBook()
                    {
                        ManualSyncOnly = true
                    }
                };
            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testBooks);

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            syncer.SyncEmailAddressBookHeaders(GetTestEmailAccount(), SyncType.Manual, new DateTime(2012, 3, 4));

            mockDm.Verify(fw => fw.CreateAddressBook(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void WhenSyncTypeNotValid_Exception()
        {
            var mockSlx = new Mock<ISlxConnector>();
            var mockDm = new Mock<IDotMailerConnector>();
            var testBooks = new List<EmailAddressBook>();
            mockSlx.Setup(fw => fw.GetEmailAddressBooks(It.IsAny<string>())).Returns(testBooks);

            var syncer = new EmailAddressBookSynchroniser(mockSlx.Object, mockDm.Object);
            Assert.Throws<InvalidOperationException>(() => syncer.SyncEmailAddressBookHeaders(GetTestEmailAccount(), (SyncType)123, new DateTime(2012, 3, 4)));
        }
    }
}