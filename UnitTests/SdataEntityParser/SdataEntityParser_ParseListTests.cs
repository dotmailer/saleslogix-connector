using System.Collections.Generic;
using EmailMarketing.SalesLogix.Entities;
using NUnit.Framework;
using Sage.SData.Client.Atom;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class SdataEntityParser_ParseListTests
    {
        [Test]
        public void WhenAtomListNull_EmptyListReturned()
        {
            var output = SdataEntityParser.ParseList<EmailAccount>(null);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.Count, Is.EqualTo(0));
        }

        [Test]
        public void WhenOneItemInList_OneItemReturned()
        {
            var atomlist = new List<AtomEntry>();
            atomlist.Add(SdataEntityParser_ParseTests.GetPartialAtomEntry());

            var output = SdataEntityParser.ParseList<EmailAccount>(atomlist);

            Assert.That(output.Count, Is.EqualTo(1));
        }

        [Test]
        public void When3ItemsInList_3ItemsReturned()
        {
            var atomlist = new List<AtomEntry>();
            atomlist.Add(SdataEntityParser_ParseTests.GetPartialAtomEntry());
            atomlist.Add(SdataEntityParser_ParseTests.GetPartialAtomEntry());
            atomlist.Add(SdataEntityParser_ParseTests.GetPartialAtomEntry());

            var output = SdataEntityParser.ParseList<EmailAccount>(atomlist);

            Assert.That(output.Count, Is.EqualTo(3));
        }
    }
}