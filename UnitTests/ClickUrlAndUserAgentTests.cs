using EmailMarketing.SalesLogix.Entities;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class ClickUrlTests
    {
        [Test]
        public void WhenNull_NullAssigned()
        {
            EmailCampaignClick click = new EmailCampaignClick();
            click.URL = null;
            Assert.That(click.URL, Is.Null);
        }

        [Test]
        public void When300Set_300Assigned()
        {
            EmailCampaignClick click = new EmailCampaignClick();
            string teststring = new string('x', 300);
            click.URL = teststring;
            Assert.That(click.URL, Is.EqualTo(teststring));
        }

        [Test]
        public void When301Set_TruncatedTo300()
        {
            EmailCampaignClick click = new EmailCampaignClick();
            string teststring = new string('x', 301);
            click.URL = teststring;
            Assert.That(click.URL.Length, Is.EqualTo(300));
        }
    }

    [TestFixture]
    public class ClickUserAgentTests
    {
        [Test]
        public void WhenNull_NullAssigned()
        {
            EmailCampaignClick click = new EmailCampaignClick();
            click.UserAgent = null;
            Assert.That(click.UserAgent, Is.Null);
        }

        [Test]
        public void When300Set_300Assigned()
        {
            EmailCampaignClick click = new EmailCampaignClick();
            string teststring = new string('x', 300);
            click.UserAgent = teststring;
            Assert.That(click.UserAgent, Is.EqualTo(teststring));
        }

        [Test]
        public void When301Set_TruncatedTo300()
        {
            EmailCampaignClick click = new EmailCampaignClick();
            string teststring = new string('x', 301);
            click.UserAgent = teststring;
            Assert.That(click.UserAgent.Length, Is.EqualTo(300));
        }
    }
}