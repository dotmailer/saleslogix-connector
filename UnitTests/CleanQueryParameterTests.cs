using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class CleanQueryParameterTests
    {
        [Test]
        public void WhenOk_ReturnsNoChange()
        {
            string toClean = "justanormalstring";
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.EqualTo(toClean));
        }

        [Test]
        public void WhenSingleQuote_CleansQuote()
        {
            string toClean = "'";
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.EqualTo("%27%27"));
        }

        [Test]
        public void WhenContainsSingleQuote_CleansQuote()
        {
            string toClean = "aaa'bbb";
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.EqualTo("aaa%27%27bbb"));
        }

        [Test]
        public void WhenPercent_CleansPercent()
        {
            string toClean = "%";
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.EqualTo("%25"));
        }

        [Test]
        public void WhenContainsPercent_CleansPercent()
        {
            string toClean = "aaa%bbb";
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.EqualTo("aaa%25bbb"));
        }

        [Test]
        public void WhenContainsQuoteAndPercent_CleansBoth()
        {
            string toClean = "aaa'bbb%ccc";
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.EqualTo("aaa%27%27bbb%25ccc"));
        }

        [Test]
        public void WhenContainsMultipleOccurrences_CleansAll()
        {
            string toClean = "a%aa'bb%'%'b%c'cc";
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.EqualTo("a%25aa%27%27bb%25%27%27%25%27%27b%25c%27%27cc"));
        }

        [Test]
        public void WhenEmpty_ReturnsEmpty()
        {
            string toClean = "";
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.EqualTo(""));
        }

        [Test]
        public void WhenNull_ReturnsNull()
        {
            string toClean = null;
            string cleaned = SlxSdata.CleanQueryParameterString(toClean);
            Assert.That(cleaned, Is.Null);
        }
    }
}