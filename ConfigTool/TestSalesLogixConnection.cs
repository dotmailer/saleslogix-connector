namespace EmailMarketing.SalesLogix
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Sage.SData.Client.Core;

    /// <summary>
    /// Form for testing whether it is possible to connect to an SData service using the specified details
    /// </summary>
    public partial class TestSalesLogixConnection : Form
    {
        private CancellationTokenSource taskCancelSource;
        private bool testComplete = false;

        public TestSalesLogixConnection()
        {
            InitializeComponent();
        }

        public string SdataUrl { get; set; }

        public string SdataUsername { get; set; }

        public string SdataPassword { get; set; }

        private void TestSalesLogixConnection_Shown(object sender, EventArgs e)
        {
            if (SdataUrl == null || SdataUsername == null || SdataPassword == null)
            {
                throw new InvalidOperationException("sdataUri, sdataUsername and sdataPassword must all be set");
            }

            taskCancelSource = new CancellationTokenSource();
            var cancelToken = taskCancelSource.Token;
            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew<ConnectionTestResult>(DoWork, cancelToken, cancelToken)
                .ContinueWith(WorkComplete, uiScheduler);
        }

        private ConnectionTestResult DoWork(object cancelToken)
        {
            var castedToken = (CancellationToken)cancelToken;
            castedToken.ThrowIfCancellationRequested();

            SdataHelper helper = new SdataHelper();
            string fixedUrl = helper.AppendRequiredUrlSegments(SdataUrl);

            ISDataService sdata = new SDataService(fixedUrl, SdataUsername, SdataPassword);
            var req = new SDataTemplateResourceRequest(sdata);
            req.ResourceKind = "contacts";
            var entry = req.Read();

            castedToken.ThrowIfCancellationRequested();

            try
            {
                req = new SDataTemplateResourceRequest(sdata);
                req.ResourceKind = "emailcampaigns";
                entry = req.Read();
            }
            catch (SDataClientException ex)
            {
                throw new SDataClientException(
                    string.Format("Could not find EmailCampaign entity.  Is the Email Marketing bundle installed in SalesLogix?{0}{1}", Environment.NewLine, ex.Message),
                    ex);
            }

            return new ConnectionTestResult() { ConnectedOk = true, Message = "Connected OK" };
        }

        private void WorkComplete(Task<ConnectionTestResult> antecedent)
        {
            bool wasTestOk = false;
            string statusMessage;
            string errorMessage = null;
            int heightIncrement = 0;
            if (antecedent.IsCanceled)
            {
                testComplete = true;
                return;
            }

            if (antecedent.Exception != null)
            {
                wasTestOk = false;
                if (antecedent.Exception.InnerException is SDataClientException)
                {
                    // We sort of expect sdata exceptions, so only show the message.
                    statusMessage = "Connection failed:";
                    errorMessage = antecedent.Exception.InnerException.Message;
                    heightIncrement = 40;
                }
                else
                {
                    // We don't expect other exceptions, so show the whole exception details
                    statusMessage = "Connection failed:";
                    errorMessage = antecedent.Exception.InnerException.ToString();
                    heightIncrement = 200;
                }
            }
            else
            {
                wasTestOk = antecedent.Result.ConnectedOk;
                statusMessage = antecedent.Result.Message;
            }

            testComplete = true;
            btnCancel.Text = "&Close";
            lblStatus.Text = statusMessage;

            if (errorMessage != null)
            {
                txtErrorMessage.Visible = true;
                txtErrorMessage.Height = heightIncrement;
                Height += heightIncrement;
                const int WidthIncrement = 250;
                Width += WidthIncrement;
                txtErrorMessage.Width += WidthIncrement;
                txtErrorMessage.Text = errorMessage;
            }

            PictureBox substitutePicture;
            if (wasTestOk)
            {
                substitutePicture = picTick;
            }
            else
            {
                substitutePicture = picCross;
            }

            substitutePicture.Location = picWorking.Location;
            picWorking.Visible = false;
            substitutePicture.Visible = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (!testComplete && taskCancelSource != null)
            {
                // Cancel the test
                taskCancelSource.Cancel();
            }

            Close();
        }

        /// <summary>
        /// Class for returning the result of the connection test
        /// </summary>
        private class ConnectionTestResult
        {
            public bool ConnectedOk { get; set; }

            public string Message { get; set; }
        }
    }
}