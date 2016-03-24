using System;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class SdataHelper_AppendRequiredUrlSegments
    {
        [Test]
        public void WhenNotAValidUrl_ArgExceptionThrown()
        {
            string testUrl = "thisisnotavalidurl";
            var helper = new SdataHelper();
            Assert.Throws<ArgumentException>(() => helper.AppendRequiredUrlSegments(testUrl));
        }

        [Test]
        public void WhenNullUrl_ArgExceptionThrown()
        {
            string testUrl = null;
            var helper = new SdataHelper();
            Assert.Throws<ArgumentException>(() => helper.AppendRequiredUrlSegments(testUrl));
        }

        [Test]
        public void WhenEmptyUrl_ArgExceptionThrown()
        {
            string testUrl = "";
            var helper = new SdataHelper();
            Assert.Throws<ArgumentException>(() => helper.AppendRequiredUrlSegments(testUrl));
        }

        [Test]
        public void WhenAllSegmentsPresent_SameUrlReturned()
        {
            string testUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic/-";
            var helper = new SdataHelper();
            var result = helper.AppendRequiredUrlSegments(testUrl);

            Assert.That(result, Is.EqualTo(testUrl));
        }

        [Test]
        public void WhenLastSegmentMissing_SegmentAdded()
        {
            string testUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic";
            string expectUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic/-";
            var helper = new SdataHelper();
            var result = helper.AppendRequiredUrlSegments(testUrl);

            Assert.That(result, Is.EqualTo(expectUrl));
        }

        [Test]
        public void WhenLastSegmentMissingWithTrailingSlash_SegmentAdded()
        {
            string testUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic/";
            string expectUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic/-";
            var helper = new SdataHelper();
            var result = helper.AppendRequiredUrlSegments(testUrl);

            Assert.That(result, Is.EqualTo(expectUrl));
        }

        [Test]
        public void WhenLast2SegmentsMissing_SegmentAdded()
        {
            string testUrl = "http://someserver:1234/aaa/bbb/ccc/slx";
            string expectUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic/-";
            var helper = new SdataHelper();
            var result = helper.AppendRequiredUrlSegments(testUrl);

            Assert.That(result, Is.EqualTo(expectUrl));
        }

        [Test]
        public void WhenLast2SegmentsMissingWithTrailingSlash_SegmentAdded()
        {
            string testUrl = "http://someserver:1234/aaa/bbb/ccc/slx/";
            string expectUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic/-";
            var helper = new SdataHelper();
            var result = helper.AppendRequiredUrlSegments(testUrl);

            Assert.That(result, Is.EqualTo(expectUrl));
        }

        [Test]
        public void WhenAll3SegmentsMissing_SegmentAdded()
        {
            string testUrl = "http://someserver:1234/aaa/bbb/ccc/";
            string expectUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic/-";
            var helper = new SdataHelper();
            var result = helper.AppendRequiredUrlSegments(testUrl);

            Assert.That(result, Is.EqualTo(expectUrl));
        }

        [Test]
        public void WhenAll3SegmentsMissingWithTrailingSlash_SegmentAdded()
        {
            string testUrl = "http://someserver:1234/aaa/bbb/ccc/";
            string expectUrl = "http://someserver:1234/aaa/bbb/ccc/slx/dynamic/-";
            var helper = new SdataHelper();
            var result = helper.AppendRequiredUrlSegments(testUrl);

            Assert.That(result, Is.EqualTo(expectUrl));
        }
    }
}