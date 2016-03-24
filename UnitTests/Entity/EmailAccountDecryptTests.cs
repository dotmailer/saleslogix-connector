using System.Security.Cryptography;
using EmailMarketing.SalesLogix.Entities;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class EmailAccountDecryptTests
    {
        [Test]
        public void WhenPasswordNull_ReturnsNull()
        {
            EmailAccount emailAcc = new EmailAccount();
            emailAcc.ApiPasswordEncrypted = null;
            var output = emailAcc.GetDecryptedPassword();
            Assert.That(output, Is.Null);
        }

        [Test]
        public void WhenValidEncryptedPassword_ReturnsDecryptedPassword()
        {
            EmailAccount emailAcc = new EmailAccount();
            emailAcc.ApiPasswordEncrypted = "st5/1iq5JytIHbB2KhIJOg==";
            var output = emailAcc.GetDecryptedPassword();
            Assert.That(output, Is.EqualTo("password"));
        }

        [Test]
        public void WhenEncryptedPasswordInvalid_ThrowsException()
        {
            EmailAccount emailAcc = new EmailAccount();
            emailAcc.ApiPasswordEncrypted = "st5/1iq5JyuIHbB2KhIJOg==";
            Assert.Throws<CryptographicException>(() => emailAcc.GetDecryptedPassword());
        }
    }
}