namespace EmailMarketing.SalesLogix
{
    partial class TestSalesLogixConnection
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

            if (disposing && (this.taskCancelSource != null))
            {
                this.taskCancelSource.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestSalesLogixConnection));
            this.picWorking = new System.Windows.Forms.PictureBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.picTick = new System.Windows.Forms.PictureBox();
            this.picCross = new System.Windows.Forms.PictureBox();
            this.txtErrorMessage = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTick)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCross)).BeginInit();
            this.SuspendLayout();
            // 
            // picWorking
            // 
            this.picWorking.Image = ((System.Drawing.Image)(resources.GetObject("picWorking.Image")));
            this.picWorking.Location = new System.Drawing.Point(13, 13);
            this.picWorking.Name = "picWorking";
            this.picWorking.Size = new System.Drawing.Size(32, 32);
            this.picWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWorking.TabIndex = 0;
            this.picWorking.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(158, 53);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(51, 13);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(182, 20);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Testing connection...";
            // 
            // picTick
            // 
            this.picTick.Image = ((System.Drawing.Image)(resources.GetObject("picTick.Image")));
            this.picTick.Location = new System.Drawing.Point(13, 53);
            this.picTick.Name = "picTick";
            this.picTick.Size = new System.Drawing.Size(32, 32);
            this.picTick.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picTick.TabIndex = 3;
            this.picTick.TabStop = false;
            this.picTick.Visible = false;
            // 
            // picCross
            // 
            this.picCross.Image = ((System.Drawing.Image)(resources.GetObject("picCross.Image")));
            this.picCross.Location = new System.Drawing.Point(54, 53);
            this.picCross.Name = "picCross";
            this.picCross.Size = new System.Drawing.Size(32, 32);
            this.picCross.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picCross.TabIndex = 4;
            this.picCross.TabStop = false;
            this.picCross.Visible = false;
            // 
            // txtErrorMessage
            // 
            this.txtErrorMessage.Location = new System.Drawing.Point(13, 51);
            this.txtErrorMessage.Multiline = true;
            this.txtErrorMessage.Name = "txtErrorMessage";
            this.txtErrorMessage.ReadOnly = true;
            this.txtErrorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtErrorMessage.Size = new System.Drawing.Size(220, 34);
            this.txtErrorMessage.TabIndex = 5;
            this.txtErrorMessage.Visible = false;
            // 
            // TestSalesLogixConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 88);
            this.ControlBox = false;
            this.Controls.Add(this.picCross);
            this.Controls.Add(this.picTick);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.picWorking);
            this.Controls.Add(this.txtErrorMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestSalesLogixConnection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Testing SalesLogix SData Connection";
            this.Shown += new System.EventHandler(this.TestSalesLogixConnection_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.picWorking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTick)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCross)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picWorking;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox picTick;
        private System.Windows.Forms.PictureBox picCross;
        private System.Windows.Forms.TextBox txtErrorMessage;
    }
}