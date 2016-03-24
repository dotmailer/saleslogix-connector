using System;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class LoggingMethodParseTests
    {
        [Test]
        public void WhenNull_ThrowsException()
        {
            LoggingMethod lm = default(LoggingMethod);
            Assert.Throws<ArgumentNullException>(() => lm.Parse(null));
        }

        [Test]
        public void WhenDiskFileLC_ReturnsDiskFile()
        {
            LoggingMethod lm = default(LoggingMethod);
            lm = lm.Parse("diskfile");
            Assert.That(lm, Is.EqualTo(LoggingMethod.DiskFile));
        }

        [Test]
        public void WhenDiskFileUC_ReturnsDiskFile()
        {
            LoggingMethod lm = default(LoggingMethod);
            lm = lm.Parse("DISKFILE");
            Assert.That(lm, Is.EqualTo(LoggingMethod.DiskFile));
        }

        [Test]
        public void WhenWinLC_ReturnsWin()
        {
            LoggingMethod lm = default(LoggingMethod);
            lm = lm.Parse("windowseventlog");
            Assert.That(lm, Is.EqualTo(LoggingMethod.WindowsEventLog));
        }

        [Test]
        public void WhenWinUC_ReturnsWin()
        {
            LoggingMethod lm = default(LoggingMethod);
            lm = lm.Parse("WINDOWSEVENTLOG");
            Assert.That(lm, Is.EqualTo(LoggingMethod.WindowsEventLog));
        }

        [Test]
        public void WhenInvalid_ThrowsException()
        {
            LoggingMethod lm = default(LoggingMethod);
            Assert.Throws<ArgumentException>(() => lm.Parse("randomstuff"));
        }
    }
}