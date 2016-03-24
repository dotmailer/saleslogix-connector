using System;
using System.Runtime.Serialization;

namespace EmailMarketing.SalesLogix.Exceptions
{
    [Serializable]
    public class EmailCampaignHeaderSynchronisationException : SynchronisationException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public EmailCampaignHeaderSynchronisationException()
        {
        }

        public EmailCampaignHeaderSynchronisationException(string message) : base(message)
        {
        }

        public EmailCampaignHeaderSynchronisationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EmailCampaignHeaderSynchronisationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}