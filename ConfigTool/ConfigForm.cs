using System.Reflection;

namespace EmailMarketing.SalesLogix
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using QGate.Components.Serialization;

    /// <summary>
    /// Configuration form the the Email Marketing service
    /// </summary>
    internal partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        // *************************************************************
        // Form Event Handlers
        // *************************************************************

        private void form_Load(object sender, EventArgs e)
        {
            radDiskFile.Checked = true;

            // Auto load the file.  It will be created on save if it doesn't currently exist.
            string serverConfigDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string serverConfigFilename = Path.Combine(serverConfigDirectory ?? "", SettingsInitialiser.ServerConfigFilename);
            LoadSettingsFromFile(serverConfigFilename);
            EnableDisableControls();
            labSaveWarning.Visible = false;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            WriteSettingToFile(SettingsInitialiser.ServerConfigFilename);
        }

        /// <summary>
        /// Flag that a setting has changed
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event args</param>
        private void SettingChange(object sender, EventArgs e)
        {
            labSaveWarning.Visible = true;
        }

        private void LoggingRadio_CheckedChanged(object sender, EventArgs e)
        {
            btnBrowseLogFile.Enabled = radDiskFile.Checked;
            SettingChange(sender, e);
        }

        private void btnBrowseLogFile_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.SelectedPath = txtLogFileDirectory.Text;
            folderBrowserDialog.Description = "Select a directory to store log files:";
            var dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                txtLogFileDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void lnkTestSdataConnection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var testForm = new TestSalesLogixConnection())
            {
                testForm.StartPosition = FormStartPosition.CenterParent;
                testForm.SdataUrl = txtSdataUrl.Text;
                testForm.SdataUsername = txtSdataUsername.Text;
                testForm.SdataPassword = txtSdataPassword.Text;
                testForm.ShowDialog();
            }
        }

        private void chkEnableDeletedItemsScan_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableControls();
            SettingChange(sender, e);
        }

        private void lnkResetScheduleToDefault_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            numCheckSlxTasksSeconds.Value = SettingsInitialiser.DefaultCheckSlxTasksFrequencySeconds; //This is how often SalesLogixUserTasksRunnable is used to check the database for (probably only) manually requested syncs and send operations.
            numSyncEmailServiceMinutes.Value = SettingsInitialiser.DefaultSyncToEmailServiceFrequencyMinutes; //How often all email accounts and their various details are automatically synced in the background.
            numSyncSlxCampaignsMinutes.Value = SettingsInitialiser.DefaultSyncToSlxCampaignsFrequencyMinutes;
            chkEnableDeletedItemsScan.Checked = SettingsInitialiser.DefaultScanDeletedItemsEnabled;
            numScanDeletedItemsMinutes.Value = SettingsInitialiser.DefaultScanDeletedItemsFrequencyMinutes;
        }

        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            if (WriteSettingToFile(SettingsInitialiser.ServerConfigFilename))
            {
                CloseForm();
            }
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !QueryAllowCloseForm();
        }

        // *************************************************************

        // *************************************************************
        // Internal Supporting Members

        /// <summary>
        /// Load the settings out of the config file
        /// </summary>
        /// <param name="filePath">Name and path of the config file</param>
        private void LoadSettingsFromFile(string filePath)
        {
            try
            {
                if (filePath.Length > 0)
                {
                    // Load settings from specified file...
                    ConfigurationFile configFile = new ConfigurationFile(filePath, SettingsInitialiser.ConfigSectionTitle);
                    configFile.Load();

                    // SData tab
                    txtSdataUrl.Text = configFile.GetProperty<string>("SDataUrl", "http://server:3333/sdata");
                    txtSdataUsername.Text = configFile.GetProperty<string>("SDataUsername", string.Empty);
                    txtSdataPassword.Text = configFile.GetProperty<string>("SDataPassword", string.Empty);

                    // Schedule tab
                    numCheckSlxTasksSeconds.Value = configFile.GetProperty<int>("CheckSlxTasksFrequencySeconds", SettingsInitialiser.DefaultCheckSlxTasksFrequencySeconds); //This is how often SalesLogixUserTasksRunnable is used to check the database for (probably only) manually requested syncs and send operations.
                    numSyncEmailServiceMinutes.Value = configFile.GetProperty<int>("SyncToEmailServiceFrequencyMinutes", SettingsInitialiser.DefaultSyncToEmailServiceFrequencyMinutes); //How often all email accounts and their various details are automatically synced in the background.
                    numSyncSlxCampaignsMinutes.Value = configFile.GetProperty<int>("SyncToSlxCampaignsFrequencyMinutes", SettingsInitialiser.DefaultSyncToSlxCampaignsFrequencyMinutes);
                    chkEnableDeletedItemsScan.Checked = configFile.GetProperty<bool>("EnableDeletedItemsScan", SettingsInitialiser.DefaultScanDeletedItemsEnabled);
                    numScanDeletedItemsMinutes.Value = configFile.GetProperty<int>("ScanDeletedItemsFrequencyMinutes", SettingsInitialiser.DefaultScanDeletedItemsFrequencyMinutes);

                    // Logging tab
                    cboLoggingLevel.SelectedItem = configFile.GetProperty<string>("LoggingLevel", "Warning");
                    string loggingMethod = configFile.GetProperty<string>("LoggingMethod", SettingsInitialiser.LoggingMethodDiskFile);
                    switch (loggingMethod)
                    {
                        case SettingsInitialiser.LoggingMethodWindowsEventLog:
                            radWindowsEventLog.Checked = true;
                            break;

                        case SettingsInitialiser.LoggingMethodAdvanced:
                            radAdvanced.Checked = true;
                            break;

                        default:
                            radDiskFile.Checked = true;
                            break;
                    }

                    string defaultLogFileDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    defaultLogFileDirectory = Path.Combine(defaultLogFileDirectory, SettingsInitialiser.LogFileDirectoryName);
                    txtLogFileDirectory.Text = configFile.GetProperty<string>("LogFileDirectory", defaultLogFileDirectory);

                    // Create default log directory if required
                    if (txtLogFileDirectory.Text == defaultLogFileDirectory)
                    {
                        DirectoryInfo dir = new DirectoryInfo(defaultLogFileDirectory);
                        if (!dir.Exists)
                        {
                            dir.Create();
                        }
                    }

                    labSaveWarning.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The following error occurred whilst attempting to read the configuration file:" + Environment.NewLine + Environment.NewLine
                    + ex.Message,
                    "Configuration File Error!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Write the settings into the config file
        /// </summary>
        /// <param name="filePath">Name and path of the config file</param>
        /// <returns>Whether the settings were written to the file</returns>
        private bool WriteSettingToFile(string filePath)
        {
            if (!ValidateEntry())
            {
                return false;
            }

            try
            {
                if (filePath.Length > 0)
                {
                    if (Path.GetDirectoryName(filePath) == string.Empty)
                    {
                        // The serializer will not save it if it does not know the directory, so
                        // add the current directory to the filePath.
                        filePath = Path.Combine(
                            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                            filePath);
                    }

                    ConfigurationFile configFile = new ConfigurationFile(filePath, SettingsInitialiser.ConfigSectionTitle);
                    const bool SECURED = true;

                    // SData tab
                    configFile.SetProperty("SDataUrl", txtSdataUrl.Text);
                    configFile.SetProperty("SDataUsername", txtSdataUsername.Text);
                    configFile.SetProperty("SDataPassword", txtSdataPassword.Text, SECURED);

                    // Schedule tab
                    configFile.SetProperty("CheckSlxTasksFrequencySeconds", (int)numCheckSlxTasksSeconds.Value);
                    configFile.SetProperty("SyncToEmailServiceFrequencyMinutes", (int)numSyncEmailServiceMinutes.Value); //How often all email accounts and their various details are automatically synced in the background.
                    configFile.SetProperty("SyncToSlxCampaignsFrequencyMinutes", (int)numSyncSlxCampaignsMinutes.Value);
                    configFile.SetProperty("EnableDeletedItemsScan", chkEnableDeletedItemsScan.Checked);
                    configFile.SetProperty("ScanDeletedItemsFrequencyMinutes", (int)numScanDeletedItemsMinutes.Value);

                    // Logging tab
                    configFile.SetProperty("LoggingLevel", cboLoggingLevel.SelectedItem);

                    string loggingMethod;
                    if (radAdvanced.Checked)
                    {
                        loggingMethod = SettingsInitialiser.LoggingMethodAdvanced;
                    }
                    else if (radDiskFile.Checked)
                    {
                        loggingMethod = SettingsInitialiser.LoggingMethodDiskFile;
                    }
                    else
                    {
                        loggingMethod = SettingsInitialiser.LoggingMethodWindowsEventLog;
                    }

                    configFile.SetProperty("LoggingMethod", loggingMethod);
                    configFile.SetProperty("LogFileDirectory", txtLogFileDirectory.Text);

                    // Save settings to specified file...
                    configFile.Save();

                    labSaveWarning.Visible = false;

                    // Present message info...
                    MessageBox.Show(
                        "Configuration file was successfully updated.",
                        "Configuration File Update",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The following error occurred whilst attempting to update the configuration file:" + Environment.NewLine + Environment.NewLine
                    + ex.Message,
                    "Configuration File Error!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate the data on the form
        /// </summary>
        /// <returns>Whether the data is valid</returns>
        private bool ValidateEntry()
        {
            Control invalidItem = null;
            TabPage invalidTab = null;

            // Check controls (in reverse order)...
            const int TabIdAuthentication = 0;
            const int TabIdLogging = 2;

            // Logging tab
            if (radDiskFile.Checked && string.IsNullOrWhiteSpace(txtLogFileDirectory.Text))
            {
                invalidItem = txtLogFileDirectory;
                invalidTab = tabs.TabPages[TabIdLogging];
            }

            if (cboLoggingLevel.SelectedItem == null || string.IsNullOrWhiteSpace(cboLoggingLevel.SelectedItem.ToString()))
            {
                invalidItem = cboLoggingLevel;
                invalidTab = tabs.TabPages[TabIdLogging];
            }

            // Authentication tab
            Uri tempUri;
            if (!Uri.TryCreate(txtSdataUrl.Text, UriKind.Absolute, out tempUri)
                || (tempUri.Scheme != "http" && tempUri.Scheme != "https"))
            {
                invalidItem = txtSdataUrl;
                invalidTab = tabs.TabPages[TabIdAuthentication];
            }

            if (string.IsNullOrWhiteSpace(txtSdataUsername.Text))
            {
                invalidItem = txtSdataUsername;
                invalidTab = tabs.TabPages[TabIdAuthentication];
            }

            // If an item is reported invalid, report on it and set its focus...
            if (invalidItem != null)
            {
                MessageBox.Show(
                    "Data is invalid please correct and re-try this operation.",
                    "Entry Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                tabs.SelectedTab = invalidTab;
                invalidItem.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enable or disable controls on the form as required by the current state
        /// </summary>
        private void EnableDisableControls()
        {
            numScanDeletedItemsMinutes.Enabled = chkEnableDeletedItemsScan.Checked;
        }

        /// <summary>
        /// Perform necessary actions to close the form
        /// </summary>
        private void CloseForm()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Decide whether the form should be allowed to close
        /// </summary>
        /// <returns>Should the form be allowed to close?</returns>
        private bool QueryAllowCloseForm()
        {
            bool allowClose = true;
            if (labSaveWarning.Visible)
            {
                var messageResult = MessageBox.Show("There are unsaved changes.  Would you like to discard these changes?", "Discard unsaved changes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (messageResult == DialogResult.No)
                {
                    allowClose = false;
                }
            }

            return allowClose;
        }
    }
}