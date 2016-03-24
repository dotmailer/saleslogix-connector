using System;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class LoggingLevelParseTests
    {
        [Test]
        public void WhenNull_ThrowsException()
        {
            LoggingLevel ll = default(LoggingLevel);
            Assert.Throws<ArgumentNullException>(() => ll.Parse(null));
        }

        [Test]
        public void WhenOffLC_ReturnsOff()
        {
            LoggingLevel ll = default(LoggingLevel);
            ll = ll.Parse("off");
            Assert.That(ll, Is.EqualTo(LoggingLevel.Off));
        }

        [Test]
        public void WhenOffUC_ReturnsOff()
        {
            LoggingLevel ll = default(LoggingLevel);
            ll = ll.Parse("OFF");
            Assert.That(ll, Is.EqualTo(LoggingLevel.Off));
        }

        [Test]
        public void WhenFatalLC_ReturnsFatal()
        {
            LoggingLevel ll = default(LoggingLevel);
            ll = ll.Parse("fatal");
            Assert.That(ll, Is.EqualTo(LoggingLevel.Fatal));
        }

        [Test]
        public void WhenErrorLC_Returnserror()
        {
            LoggingLevel ll = default(LoggingLevel);
            ll = ll.Parse("error");
            Assert.That(ll, Is.EqualTo(LoggingLevel.Error));
        }

        [Test]
        public void WhenWarningLC_ReturnsWarning()
        {
            LoggingLevel ll = default(LoggingLevel);
            ll = ll.Parse("warning");
            Assert.That(ll, Is.EqualTo(LoggingLevel.Warning));
        }

        [Test]
        public void WhenInfoLC_ReturnsInfo()
        {
            LoggingLevel ll = default(LoggingLevel);
            ll = ll.Parse("information");
            Assert.That(ll, Is.EqualTo(LoggingLevel.Info));
        }

        [Test]
        public void WhenDebugLC_ReturnsDebug()
        {
            LoggingLevel ll = default(LoggingLevel);
            ll = ll.Parse("debug");
            Assert.That(ll, Is.EqualTo(LoggingLevel.Debug));
        }

        [Test]
        public void WhenAllLC_ReturnsAll()
        {
            LoggingLevel ll = default(LoggingLevel);
            ll = ll.Parse("all");
            Assert.That(ll, Is.EqualTo(LoggingLevel.All));
        }

        [Test]
        public void WhenInvalid_ThrowsException()
        {
            LoggingMethod lm = default(LoggingMethod);
            Assert.Throws<ArgumentException>(() => lm.Parse("randomstuff"));
        }
    }
}