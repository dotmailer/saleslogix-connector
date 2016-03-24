using System;
using NUnit.Framework;

namespace EmailMarketing.SalesLogix.UnitTests
{
    [TestFixture]
    public class CampaignMapper_CreateSlxEmailCampaignTests
    {
        [Test]
        public void WhenDmCampNull_ExceptionThrown()
        {
            var mapper = new CampaignMapper();
            Assert.Throws<ArgumentNullException>(() => mapper.CreateSlxEmailCampaign(null, String.Empty));
        }
    }
}