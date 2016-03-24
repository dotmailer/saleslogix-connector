namespace EmailMarketing.SalesLogix
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnApply = new System.Windows.Forms.Button();
            this.tabSchedule = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lnkResetScheduleToDefault = new System.Windows.Forms.LinkLabel();
            this.chkEnableDeletedItemsScan = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numScanDeletedItemsMinutes = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numSyncSlxCampaignsMinutes = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numSyncEmailServiceMinutes = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numCheckSlxTasksSeconds = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabAuthentication = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lnkTestSdataConnection = new System.Windows.Forms.LinkLabel();
            this.txtSdataPassword = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtSdataUsername = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSdataUrl = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabLogging = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.radAdvanced = new System.Windows.Forms.RadioButton();
            this.btnBrowseLogFile = new System.Windows.Forms.Button();
            this.txtLogFileDirectory = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.radDiskFile = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.radWindowsEventLog = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.cboLoggingLevel = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnSaveAndClose = new System.Windows.Forms.Button();
            this.labSaveWarning = new System.Windows.Forms.Label();
            this.tabSchedule.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanDeletedItemsMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSyncSlxCampaignsMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSyncEmailServiceMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckSlxTasksSeconds)).BeginInit();
            this.tabs.SuspendLayout();
            this.tabAuthentication.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabLogging.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(614, 399);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "Paribus.Interactive.Plugin.dll";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(533, 399);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tabSchedule
            // 
            this.tabSchedule.Controls.Add(this.groupBox1);
            this.tabSchedule.Location = new System.Drawing.Point(4, 26);
            this.tabSchedule.Name = "tabSchedule";
            this.tabSchedule.Padding = new System.Windows.Forms.Padding(3);
            this.tabSchedule.Size = new System.Drawing.Size(669, 351);
            this.tabSchedule.TabIndex = 0;
            this.tabSchedule.Text = "Schedule";
            this.tabSchedule.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.lnkResetScheduleToDefault);
            this.groupBox1.Controls.Add(this.chkEnableDeletedItemsScan);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.numScanDeletedItemsMinutes);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numSyncSlxCampaignsMinutes);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numSyncEmailServiceMinutes);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numCheckSlxTasksSeconds);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(10, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(651, 338);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Service Schedule";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(54, 39);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(325, 17);
            this.label19.TabIndex = 30;
            this.label19.Text = "Specify how frequently service tasks are run.";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(16, 29);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 29;
            this.pictureBox2.TabStop = false;
            // 
            // lnkResetScheduleToDefault
            // 
            this.lnkResetScheduleToDefault.AutoSize = true;
            this.lnkResetScheduleToDefault.Location = new System.Drawing.Point(14, 188);
            this.lnkResetScheduleToDefault.Name = "lnkResetScheduleToDefault";
            this.lnkResetScheduleToDefault.Size = new System.Drawing.Size(199, 17);
            this.lnkResetScheduleToDefault.TabIndex = 28;
            this.lnkResetScheduleToDefault.TabStop = true;
            this.lnkResetScheduleToDefault.Text = "Reset to Schedule Defaults";
            this.lnkResetScheduleToDefault.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkResetScheduleToDefault_LinkClicked);
            // 
            // chkEnableDeletedItemsScan
            // 
            this.chkEnableDeletedItemsScan.AutoSize = true;
            this.chkEnableDeletedItemsScan.Location = new System.Drawing.Point(17, 167);
            this.chkEnableDeletedItemsScan.Name = "chkEnableDeletedItemsScan";
            this.chkEnableDeletedItemsScan.Size = new System.Drawing.Size(18, 17);
            this.chkEnableDeletedItemsScan.TabIndex = 27;
            this.chkEnableDeletedItemsScan.UseVisualStyleBackColor = true;
            this.chkEnableDeletedItemsScan.CheckedChanged += new System.EventHandler(this.chkEnableDeletedItemsScan_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(459, 167);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 17);
            this.label9.TabIndex = 26;
            this.label9.Text = "minutes.";
            // 
            // numScanDeletedItemsMinutes
            // 
            this.numScanDeletedItemsMinutes.Location = new System.Drawing.Point(398, 165);
            this.numScanDeletedItemsMinutes.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numScanDeletedItemsMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numScanDeletedItemsMinutes.Name = "numScanDeletedItemsMinutes";
            this.numScanDeletedItemsMinutes.Size = new System.Drawing.Size(55, 24);
            this.numScanDeletedItemsMinutes.TabIndex = 25;
            this.numScanDeletedItemsMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numScanDeletedItemsMinutes.ValueChanged += new System.EventHandler(this.SettingChange);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(38, 167);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(391, 17);
            this.label17.TabIndex = 24;
            this.label17.Text = "Scan SalesLogix for deleted Contacts and Leads every";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(375, 136);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 17);
            this.label7.TabIndex = 23;
            this.label7.Text = "minutes.";
            // 
            // numSyncSlxCampaignsMinutes
            // 
            this.numSyncSlxCampaignsMinutes.Location = new System.Drawing.Point(314, 134);
            this.numSyncSlxCampaignsMinutes.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numSyncSlxCampaignsMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSyncSlxCampaignsMinutes.Name = "numSyncSlxCampaignsMinutes";
            this.numSyncSlxCampaignsMinutes.Size = new System.Drawing.Size(55, 24);
            this.numSyncSlxCampaignsMinutes.TabIndex = 22;
            this.numSyncSlxCampaignsMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSyncSlxCampaignsMinutes.ValueChanged += new System.EventHandler(this.SettingChange);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(319, 17);
            this.label8.TabIndex = 21;
            this.label8.Text = "Synchronise to SalesLogix Campaigns every";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 17);
            this.label3.TabIndex = 20;
            this.label3.Text = "minutes (minimum 10).";
            // 
            // numSyncEmailServiceMinutes
            // 
            this.numSyncEmailServiceMinutes.Location = new System.Drawing.Point(250, 103);
            this.numSyncEmailServiceMinutes.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numSyncEmailServiceMinutes.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSyncEmailServiceMinutes.Name = "numSyncEmailServiceMinutes";
            this.numSyncEmailServiceMinutes.Size = new System.Drawing.Size(55, 24);
            this.numSyncEmailServiceMinutes.TabIndex = 19;
            this.numSyncEmailServiceMinutes.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSyncEmailServiceMinutes.ValueChanged += new System.EventHandler(this.SettingChange);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(253, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Synchronise to Email Service every";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(311, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 17);
            this.label2.TabIndex = 17;
            this.label2.Text = "seconds.";
            // 
            // numCheckSlxTasksSeconds
            // 
            this.numCheckSlxTasksSeconds.Location = new System.Drawing.Point(250, 72);
            this.numCheckSlxTasksSeconds.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numCheckSlxTasksSeconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCheckSlxTasksSeconds.Name = "numCheckSlxTasksSeconds";
            this.numCheckSlxTasksSeconds.Size = new System.Drawing.Size(55, 24);
            this.numCheckSlxTasksSeconds.TabIndex = 16;
            this.numCheckSlxTasksSeconds.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCheckSlxTasksSeconds.ValueChanged += new System.EventHandler(this.SettingChange);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(241, 17);
            this.label1.TabIndex = 15;
            this.label1.Text = "Check SalesLogix for tasks every";
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabAuthentication);
            this.tabs.Controls.Add(this.tabSchedule);
            this.tabs.Controls.Add(this.tabLogging);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(677, 381);
            this.tabs.TabIndex = 5;
            // 
            // tabAuthentication
            // 
            this.tabAuthentication.Controls.Add(this.groupBox3);
            this.tabAuthentication.Location = new System.Drawing.Point(4, 26);
            this.tabAuthentication.Name = "tabAuthentication";
            this.tabAuthentication.Size = new System.Drawing.Size(669, 351);
            this.tabAuthentication.TabIndex = 3;
            this.tabAuthentication.Text = "Authentication";
            this.tabAuthentication.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Controls.Add(this.lnkTestSdataConnection);
            this.groupBox3.Controls.Add(this.txtSdataPassword);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.txtSdataUsername);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.txtSdataUrl);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox3.Location = new System.Drawing.Point(8, 11);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(651, 338);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "SalesLogix SData Connection";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(54, 39);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(359, 17);
            this.label18.TabIndex = 32;
            this.label18.Text = "Specify your SalesLogix SData connection details.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(16, 29);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 31;
            this.pictureBox1.TabStop = false;
            // 
            // lnkTestSdataConnection
            // 
            this.lnkTestSdataConnection.AutoSize = true;
            this.lnkTestSdataConnection.Location = new System.Drawing.Point(439, 157);
            this.lnkTestSdataConnection.Name = "lnkTestSdataConnection";
            this.lnkTestSdataConnection.Size = new System.Drawing.Size(249, 17);
            this.lnkTestSdataConnection.TabIndex = 6;
            this.lnkTestSdataConnection.TabStop = true;
            this.lnkTestSdataConnection.Text = "Test SalesLogix SData Connection";
            this.lnkTestSdataConnection.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTestSdataConnection_LinkClicked);
            // 
            // txtSdataPassword
            // 
            this.txtSdataPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSdataPassword.Location = new System.Drawing.Point(256, 133);
            this.txtSdataPassword.Name = "txtSdataPassword";
            this.txtSdataPassword.PasswordChar = '*';
            this.txtSdataPassword.Size = new System.Drawing.Size(382, 24);
            this.txtSdataPassword.TabIndex = 5;
            this.txtSdataPassword.TextChanged += new System.EventHandler(this.SettingChange);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(14, 136);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(209, 17);
            this.label16.TabIndex = 4;
            this.label16.Text = "SalesLogix SData Password:";
            // 
            // txtSdataUsername
            // 
            this.txtSdataUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSdataUsername.Location = new System.Drawing.Point(256, 102);
            this.txtSdataUsername.Name = "txtSdataUsername";
            this.txtSdataUsername.Size = new System.Drawing.Size(382, 24);
            this.txtSdataUsername.TabIndex = 3;
            this.txtSdataUsername.TextChanged += new System.EventHandler(this.SettingChange);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 105);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(212, 17);
            this.label10.TabIndex = 2;
            this.label10.Text = "SalesLogix SData Username:";
            // 
            // txtSdataUrl
            // 
            this.txtSdataUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSdataUrl.Location = new System.Drawing.Point(256, 71);
            this.txtSdataUrl.Name = "txtSdataUrl";
            this.txtSdataUrl.Size = new System.Drawing.Size(382, 24);
            this.txtSdataUrl.TabIndex = 1;
            this.txtSdataUrl.TextChanged += new System.EventHandler(this.SettingChange);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(170, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "SalesLogix SData URL:";
            // 
            // tabLogging
            // 
            this.tabLogging.Controls.Add(this.groupBox2);
            this.tabLogging.Location = new System.Drawing.Point(4, 26);
            this.tabLogging.Name = "tabLogging";
            this.tabLogging.Size = new System.Drawing.Size(669, 351);
            this.tabLogging.TabIndex = 1;
            this.tabLogging.Text = "Logging";
            this.tabLogging.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.radAdvanced);
            this.groupBox2.Controls.Add(this.btnBrowseLogFile);
            this.groupBox2.Controls.Add(this.txtLogFileDirectory);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.radDiskFile);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.radWindowsEventLog);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.cboLoggingLevel);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.pictureBox4);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(8, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(651, 338);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Service Message Logging";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(33, 249);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(486, 37);
            this.label15.TabIndex = 15;
            this.label15.Text = "Logging settings will be loaded from a file called \"log4net.config.xml\" in the sa" +
    "me directory as the Email Marketing Service executable (.exe).";
            // 
            // radAdvanced
            // 
            this.radAdvanced.AutoSize = true;
            this.radAdvanced.Location = new System.Drawing.Point(16, 229);
            this.radAdvanced.Name = "radAdvanced";
            this.radAdvanced.Size = new System.Drawing.Size(438, 21);
            this.radAdvanced.TabIndex = 14;
            this.radAdvanced.Text = "Advanced - Load logging settings from a configuration file";
            this.radAdvanced.UseVisualStyleBackColor = true;
            this.radAdvanced.CheckedChanged += new System.EventHandler(this.LoggingRadio_CheckedChanged);
            // 
            // btnBrowseLogFile
            // 
            this.btnBrowseLogFile.Enabled = false;
            this.btnBrowseLogFile.Location = new System.Drawing.Point(584, 108);
            this.btnBrowseLogFile.Name = "btnBrowseLogFile";
            this.btnBrowseLogFile.Size = new System.Drawing.Size(25, 23);
            this.btnBrowseLogFile.TabIndex = 13;
            this.btnBrowseLogFile.Text = "...";
            this.btnBrowseLogFile.UseVisualStyleBackColor = true;
            this.btnBrowseLogFile.Click += new System.EventHandler(this.btnBrowseLogFile_Click);
            // 
            // txtLogFileDirectory
            // 
            this.txtLogFileDirectory.Location = new System.Drawing.Point(106, 109);
            this.txtLogFileDirectory.Name = "txtLogFileDirectory";
            this.txtLogFileDirectory.ReadOnly = true;
            this.txtLogFileDirectory.Size = new System.Drawing.Size(472, 24);
            this.txtLogFileDirectory.TabIndex = 12;
            this.txtLogFileDirectory.Text = "C:\\";
            this.txtLogFileDirectory.TextChanged += new System.EventHandler(this.SettingChange);
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(33, 133);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(501, 47);
            this.label14.TabIndex = 11;
            this.label14.Text = resources.GetString("label14.Text");
            // 
            // radDiskFile
            // 
            this.radDiskFile.AutoSize = true;
            this.radDiskFile.Location = new System.Drawing.Point(16, 110);
            this.radDiskFile.Name = "radDiskFile";
            this.radDiskFile.Size = new System.Drawing.Size(86, 21);
            this.radDiskFile.TabIndex = 10;
            this.radDiskFile.Text = "Disk File";
            this.radDiskFile.UseVisualStyleBackColor = true;
            this.radDiskFile.CheckedChanged += new System.EventHandler(this.LoggingRadio_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(33, 203);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(552, 17);
            this.label13.TabIndex = 9;
            this.label13.Text = "Messages will be sent to the \"Application\" section of the Windows Event Log.";
            // 
            // radWindowsEventLog
            // 
            this.radWindowsEventLog.AutoSize = true;
            this.radWindowsEventLog.Location = new System.Drawing.Point(16, 183);
            this.radWindowsEventLog.Name = "radWindowsEventLog";
            this.radWindowsEventLog.Size = new System.Drawing.Size(169, 21);
            this.radWindowsEventLog.TabIndex = 8;
            this.radWindowsEventLog.Text = "Windows Event Log";
            this.radWindowsEventLog.UseVisualStyleBackColor = true;
            this.radWindowsEventLog.CheckedChanged += new System.EventHandler(this.LoggingRadio_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(262, 75);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(370, 17);
            this.label12.TabIndex = 7;
            this.label12.Text = "Messages of this severity and higher will be logged.";
            // 
            // cboLoggingLevel
            // 
            this.cboLoggingLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLoggingLevel.FormattingEnabled = true;
            this.cboLoggingLevel.Items.AddRange(new object[] {
            "Off",
            "Fatal",
            "Error",
            "Warning",
            "Information",
            "Debug",
            "All"});
            this.cboLoggingLevel.Location = new System.Drawing.Point(106, 72);
            this.cboLoggingLevel.Name = "cboLoggingLevel";
            this.cboLoggingLevel.Size = new System.Drawing.Size(121, 25);
            this.cboLoggingLevel.TabIndex = 6;
            this.cboLoggingLevel.SelectedIndexChanged += new System.EventHandler(this.SettingChange);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 75);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 17);
            this.label11.TabIndex = 5;
            this.label11.Text = "Logging level:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(54, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(513, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "Control where and how warning and diagnostic messages are recorded.";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(16, 29);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 32);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox4.TabIndex = 3;
            this.pictureBox4.TabStop = false;
            // 
            // btnSaveAndClose
            // 
            this.btnSaveAndClose.Location = new System.Drawing.Point(403, 399);
            this.btnSaveAndClose.Name = "btnSaveAndClose";
            this.btnSaveAndClose.Size = new System.Drawing.Size(124, 23);
            this.btnSaveAndClose.TabIndex = 6;
            this.btnSaveAndClose.Text = "&Save and Close";
            this.btnSaveAndClose.UseVisualStyleBackColor = true;
            this.btnSaveAndClose.Click += new System.EventHandler(this.btnSaveAndClose_Click);
            // 
            // labSaveWarning
            // 
            this.labSaveWarning.AutoSize = true;
            this.labSaveWarning.ForeColor = System.Drawing.Color.Maroon;
            this.labSaveWarning.Location = new System.Drawing.Point(149, 404);
            this.labSaveWarning.Name = "labSaveWarning";
            this.labSaveWarning.Size = new System.Drawing.Size(306, 17);
            this.labSaveWarning.TabIndex = 7;
            this.labSaveWarning.Text = "Setting changes have not yet been saved.";
            this.labSaveWarning.Visible = false;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(701, 434);
            this.Controls.Add(this.labSaveWarning);
            this.Controls.Add(this.btnSaveAndClose);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabs);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Email Marketing for SalesLogix Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.Load += new System.EventHandler(this.form_Load);
            this.tabSchedule.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScanDeletedItemsMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSyncSlxCampaignsMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSyncEmailServiceMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckSlxTasksSeconds)).EndInit();
            this.tabs.ResumeLayout(false);
            this.tabAuthentication.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabLogging.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabPage tabSchedule;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabLogging;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cboLoggingLevel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.RadioButton radDiskFile;
        private System.Windows.Forms.Button btnBrowseLogFile;
        private System.Windows.Forms.TextBox txtLogFileDirectory;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.RadioButton radAdvanced;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TabPage tabAuthentication;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSaveAndClose;
        private System.Windows.Forms.TextBox txtSdataPassword;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtSdataUsername;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtSdataUrl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labSaveWarning;
        private System.Windows.Forms.LinkLabel lnkTestSdataConnection;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton radWindowsEventLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel lnkResetScheduleToDefault;
        private System.Windows.Forms.CheckBox chkEnableDeletedItemsScan;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numScanDeletedItemsMinutes;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numSyncSlxCampaignsMinutes;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numSyncEmailServiceMinutes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numCheckSlxTasksSeconds;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

