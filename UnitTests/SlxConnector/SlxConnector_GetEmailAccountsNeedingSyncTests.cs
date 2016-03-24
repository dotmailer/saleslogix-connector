using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sage.SData.Client.Atom;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class SlxConnector_GetEmailAccountsNeedingSyncTests
    {
        [Test]
        public void WhenRun_SyncDateCalculatedCorrectly()
        {
            var mock = new Mock<ISlxSdata>();
            var connector = new SlxConnector(mock.Object);

            DateTime n = DateTime.UtcNow;
            var output = connector.GetEmailAccountsNeedingSync(80);

            mock.Verify(
                framework => framework.GetEmailAccountsLastSyncedBefore(It.IsInRange<DateTime>(n.AddSeconds(-81), n.AddSeconds(-79), Range.Inclusive)),
                    Times.Once());
        }

        [Test]
        public void WhenNoAccounts_EmptyCollectionReturned()
        {
            var mock = new Mock<ISlxSdata>();
            var mockReturnVal = new List<AtomEntry>();
            mock.Setup(fw => fw.GetEmailAccountsLastSyncedBefore(It.IsAny<DateTime>())).Returns(mockReturnVal);
            var connector = new SlxConnector(mock.Object);

            DateTime n = DateTime.Now;
            var output = connector.GetEmailAccountsNeedingSync(80);

            mock.Verify(
                framework => framework.GetEmailAccountsLastSyncedBefore(It.IsAny<DateTime>()),
                    Times.Once());
            Assert.That(output.Count, Is.EqualTo(0));
        }

        [Test]
        public void WhenOneAccount_OneItemReturned()
        {
            var mock = new Mock<ISlxSdata>();
            var mockReturnVal = new List<AtomEntry>();
            mockReturnVal.Add(SdataEntityParser_ParseTests.GetPartialAtomEntry());
            mock.Setup(fw => fw.GetEmailAccountsLastSyncedBefore(It.IsAny<DateTime>())).Returns(mockReturnVal);
            var connector = new SlxConnector(mock.Object);

            DateTime n = DateTime.Now;
            var output = connector.GetEmailAccountsNeedingSync(80);

            mock.Verify(
                framework => framework.GetEmailAccountsLastSyncedBefore(It.IsAny<DateTime>()),
                    Times.Once());
            Assert.That(output.Count, Is.EqualTo(1));
        }

        [Test]
        public void WhenThreeAccounts_ThreeItemsReturned()
        {
            var mock = new Mock<ISlxSdata>();
            var mockReturnVal = new List<AtomEntry>();
            mockReturnVal.Add(SdataEntityParser_ParseTests.GetPartialAtomEntry());
            mockReturnVal.Add(SdataEntityParser_ParseTests.GetPartialAtomEntry());
            mockReturnVal.Add(SdataEntityParser_ParseTests.GetPartialAtomEntry());
            mock.Setup(fw => fw.GetEmailAccountsLastSyncedBefore(It.IsAny<DateTime>())).Returns(mockReturnVal);
            var connector = new SlxConnector(mock.Object);

            DateTime n = DateTime.Now;
            var output = connector.GetEmailAccountsNeedingSync(80);

            mock.Verify(
                framework => framework.GetEmailAccountsLastSyncedBefore(It.IsAny<DateTime>()),
                    Times.Once());
            Assert.That(output.Count, Is.EqualTo(3));
        }
    }
}