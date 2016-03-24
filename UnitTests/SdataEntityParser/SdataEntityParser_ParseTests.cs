using System;
using EmailMarketing.SalesLogix.Entities;
using NUnit.Framework;
using Sage.SData.Client.Atom;
using Sage.SData.Client.Extensions;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class SdataEntityParser_ParseTests
    {
        private const string TestCreateUserId = "U12345678901";
        private const string TestKey = "AAA123456789";
        private static readonly DateTime _TestCreateDate = new DateTime(2012, 3, 4, 5, 6, 7);

        public static AtomEntry GetPartialAtomEntry()
        {
            AtomEntry entry = new AtomEntry(
                            new AtomId(),
                            new AtomTextConstruct("TestEmailAccount"),
                            DateTime.Now);
            var payload = new SDataPayload();
            payload.Key = TestKey;
            payload.Values.Add("CreateUser", TestCreateUserId);
            payload.Values.Add("CreateDate", _TestCreateDate.ToString("s"));
            entry.SetSDataPayload(payload);
            return entry;
        }

        [Test]
        public void WhenEntryNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => SdataEntityParser.Parse<EmailAccount>(null));
        }

        [Test]
        public void WhenEntryHasJustAKey_PopulatedEntityReturned()
        {
            AtomEntry entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("TestEmailAccount"),
                DateTime.Now);
            var payload = new SDataPayload();
            const string TestKey = "K12345678901";
            payload.Key = TestKey;
            entry.SetSDataPayload(payload);

            var output = SdataEntityParser.Parse<EmailAccount>(entry);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.Id, Is.EqualTo(TestKey));
        }

        [Test]
        public void WhenEntryContainsStringFields_PopulatedEntityReturned()
        {
            AtomEntry entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("TestEmailAccount"),
                DateTime.Now);
            var payload = new SDataPayload();
            //const string TestEmailAccountId = "AAA123456789";
            const string TestCreateUserId = "U12345678901";
            DateTime TestCreateDate = new DateTime(2012, 3, 4, 5, 6, 7);
            payload.Values.Add("CreateUser", TestCreateUserId);
            entry.SetSDataPayload(payload);

            var output = SdataEntityParser.Parse<EmailAccount>(entry);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.CreateUserId, Is.EqualTo(TestCreateUserId));
        }

        [Test]
        public void WhenEntryContainsDateFields_PopulatedEntityReturned()
        {
            AtomEntry entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("TestEmailAccount"),
                DateTime.Now);
            var payload = new SDataPayload();
            DateTime TestCreateDate = new DateTime(2012, 3, 4, 5, 6, 7);
            payload.Values.Add("CreateDate", TestCreateDate.ToString("s"));
            entry.SetSDataPayload(payload);

            var output = SdataEntityParser.Parse<EmailAccount>(entry);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.CreateDate, Is.EqualTo(TestCreateDate));
        }

        [Test]
        public void WhenEntryContainsNullDateFields_PopulatedEntityReturned()
        {
            AtomEntry entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("TestEmailAccount"),
                DateTime.Now);
            var payload = new SDataPayload();
            payload.Values.Add("CreateDate", null);
            entry.SetSDataPayload(payload);

            var output = SdataEntityParser.Parse<EmailAccount>(entry);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.CreateDate, Is.Null);
        }

        [Test]
        public void WhenEntryContainsIntFields_PopulatedEntityReturned()
        {
            AtomEntry entry = new AtomEntry(
                new AtomId(),
                new AtomTextConstruct("TestEmailAccount"),
                DateTime.Now);
            var payload = new SDataPayload();
            payload.Values.Add("SyncIntervalMinutes", "5");
            entry.SetSDataPayload(payload);

            var output = SdataEntityParser.Parse<EmailAccount>(entry);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.SyncIntervalMinutes, Is.EqualTo(5));
        }

        [Test]
        public void WhenEntryContainsMixedFields_PopulatedEntityReturned()
        {
            AtomEntry entry = GetPartialAtomEntry();

            var output = SdataEntityParser.Parse<EmailAccount>(entry);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.Id, Is.EqualTo(TestKey));
            Assert.That(output.CreateUserId, Is.EqualTo(TestCreateUserId));
            Assert.That(output.CreateDate, Is.EqualTo(_TestCreateDate));
        }
    }
}