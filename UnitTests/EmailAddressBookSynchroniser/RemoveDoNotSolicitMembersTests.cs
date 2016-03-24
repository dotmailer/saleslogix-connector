using System.Collections.Generic;
using EmailMarketing.SalesLogix.Entities;
using Moq;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class RemoveDoNotSolicitMembersTests
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
        public void When1_BatchDeleteRunFor1Item()
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

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            syncer.CallBase = true;

            syncer.Object.RemoveDoNotSolicitMembers(GetTestAddrBook(), testMemberList);

            mockDm.Verify(fw => fw.DeleteAddressBookContacts(
                It.Is<int>(n => n == GetTestAddrBook().EmailServiceAddressBookId),
                It.Is<ICollection<string>>(c => c.Count == 1)),
                Times.Once());
        }

        [Test]
        public void When3_BatchDeleteRunFor3Items()
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

            EmailAddressBookMember testABM2 = new EmailAddressBookMember()
            {
                Contact = new Sage.SData.Client.Extensions.SDataPayload(),
                SlxMemberType = "Contact",
                LastSyncedEmailAddress = "testemail2@example.com",
                LastSyncHashCode = 1111111,
                SlxContactId = "C23456789012"
            };
            testABM2.Contact.Values.Add("Email", "testemai2l@example.com");

            EmailAddressBookMember testABM3 = new EmailAddressBookMember()
            {
                Contact = new Sage.SData.Client.Extensions.SDataPayload(),
                SlxMemberType = "Contact",
                LastSyncedEmailAddress = "testemail3@example.com",
                LastSyncHashCode = 1111111,
                SlxContactId = "C23456789012"
            };
            testABM3.Contact.Values.Add("Email", "testemail3@example.com");
            var testMemberList = new List<EmailAddressBookMember>()
            {
                testABM, testABM2, testABM3
            };

            var syncer = new Mock<EmailAddressBookSynchroniser>(mockSlx.Object, mockDm.Object);
            syncer.CallBase = true;

            syncer.Object.RemoveDoNotSolicitMembers(GetTestAddrBook(), testMemberList);

            mockDm.Verify(fw => fw.DeleteAddressBookContacts(
                It.Is<int>(n => n == GetTestAddrBook().EmailServiceAddressBookId),
                It.Is<ICollection<string>>(c => c.Count == 3)),
                Times.Once());
        }
    }
}