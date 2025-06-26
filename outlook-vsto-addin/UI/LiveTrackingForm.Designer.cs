namespace EffyDocOutlookAddin.UI
{
    partial class LiveTrackingForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblConnectionStatus = new System.Windows.Forms.Label();
            this.btnSelectDocument = new System.Windows.Forms.Button();
            this.txtSelectedDocument = new System.Windows.Forms.TextBox();
            this.lblDocument = new System.Windows.Forms.Label();
            this.tabControlTracking = new System.Windows.Forms.TabControl();
            this.tabLiveTracking = new System.Windows.Forms.TabPage();
            this.panelLiveTracking = new System.Windows.Forms.Panel();
            this.groupCurrentReaders = new System.Windows.Forms.GroupBox();
            this.listCurrentReaders = new System.Windows.Forms.ListView();
            this.columnReaderEmail = new System.Windows.Forms.ColumnHeader();
            this.columnReaderPage = new System.Windows.Forms.ColumnHeader();
            this.columnReaderDuration = new System.Windows.Forms.ColumnHeader();
            this.columnReaderSince = new System.Windows.Forms.ColumnHeader();
            this.groupTodayStats = new System.Windows.Forms.GroupBox();
            this.lblUniqueViewers = new System.Windows.Forms.Label();
            this.lblPageViews = new System.Windows.Forms.Label();
            this.lblLinksClicked = new System.Windows.Forms.Label();
            this.lblEmailsOpened = new System.Windows.Forms.Label();
            this.groupRecentActivity = new System.Windows.Forms.GroupBox();
            this.listRecentActivity = new System.Windows.Forms.ListView();
            this.columnActivityType = new System.Windows.Forms.ColumnHeader();
            this.columnActivityUser = new System.Windows.Forms.ColumnHeader();
            this.columnActivityPage = new System.Windows.Forms.ColumnHeader();
            this.columnActivityTime = new System.Windows.Forms.ColumnHeader();
            this.tabAnalytics = new System.Windows.Forms.TabPage();
            this.panelAnalytics = new System.Windows.Forms.Panel();
            this.groupSummary = new System.Windows.Forms.GroupBox();
            this.lblUniqueViewersTotal = new System.Windows.Forms.Label();
            this.lblClickRate = new System.Windows.Forms.Label();
            this.lblOpenRate = new System.Windows.Forms.Label();
            this.lblTotalClicks = new System.Windows.Forms.Label();
            this.lblTotalOpens = new System.Windows.Forms.Label();
            this.lblTotalEmailsSent = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.tabControlTracking.SuspendLayout();
            this.tabLiveTracking.SuspendLayout();
            this.panelLiveTracking.SuspendLayout();
            this.groupCurrentReaders.SuspendLayout();
            this.groupTodayStats.SuspendLayout();
            this.groupRecentActivity.SuspendLayout();
            this.tabAnalytics.SuspendLayout();
            this.panelAnalytics.SuspendLayout();
            this.groupSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.btnRefresh);
            this.panelHeader.Controls.Add(this.btnConnect);
            this.panelHeader.Controls.Add(this.lblConnectionStatus);
            this.panelHeader.Controls.Add(this.btnSelectDocument);
            this.panelHeader.Controls.Add(this.txtSelectedDocument);
            this.panelHeader.Controls.Add(this.lblDocument);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(350, 80);
            this.panelHeader.TabIndex = 0;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(10, 50);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblConnectionStatus
            // 
            this.lblConnectionStatus.AutoSize = true;
            this.lblConnectionStatus.Location = new System.Drawing.Point(100, 55);
            this.lblConnectionStatus.Name = "lblConnectionStatus";
            this.lblConnectionStatus.Size = new System.Drawing.Size(73, 13);
            this.lblConnectionStatus.TabIndex = 3;
            this.lblConnectionStatus.Text = "Disconnected";
            // 
            // btnSelectDocument
            // 
            this.btnSelectDocument.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectDocument.Location = new System.Drawing.Point(270, 25);
            this.btnSelectDocument.Name = "btnSelectDocument";
            this.btnSelectDocument.Size = new System.Drawing.Size(75, 23);
            this.btnSelectDocument.TabIndex = 2;
            this.btnSelectDocument.Text = "Select";
            this.btnSelectDocument.UseVisualStyleBackColor = true;
            this.btnSelectDocument.Click += new System.EventHandler(this.btnSelectDocument_Click);
            // 
            // txtSelectedDocument
            // 
            this.txtSelectedDocument.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelectedDocument.Location = new System.Drawing.Point(70, 25);
            this.txtSelectedDocument.Name = "txtSelectedDocument";
            this.txtSelectedDocument.ReadOnly = true;
            this.txtSelectedDocument.Size = new System.Drawing.Size(190, 20);
            this.txtSelectedDocument.TabIndex = 1;
            // 
            // lblDocument
            // 
            this.lblDocument.AutoSize = true;
            this.lblDocument.Location = new System.Drawing.Point(10, 28);
            this.lblDocument.Name = "lblDocument";
            this.lblDocument.Size = new System.Drawing.Size(57, 13);
            this.lblDocument.TabIndex = 0;
            this.lblDocument.Text = "Document:";
            // 
            // tabControlTracking
            // 
            this.tabControlTracking.Controls.Add(this.tabLiveTracking);
            this.tabControlTracking.Controls.Add(this.tabAnalytics);
            this.tabControlTracking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlTracking.Location = new System.Drawing.Point(0, 80);
            this.tabControlTracking.Name = "tabControlTracking";
            this.tabControlTracking.SelectedIndex = 0;
            this.tabControlTracking.Size = new System.Drawing.Size(350, 350);
            this.tabControlTracking.TabIndex = 1;
            // 
            // tabLiveTracking
            // 
            this.tabLiveTracking.Controls.Add(this.panelLiveTracking);
            this.tabLiveTracking.Location = new System.Drawing.Point(4, 22);
            this.tabLiveTracking.Name = "tabLiveTracking";
            this.tabLiveTracking.Padding = new System.Windows.Forms.Padding(3);
            this.tabLiveTracking.Size = new System.Drawing.Size(342, 324);
            this.tabLiveTracking.TabIndex = 0;
            this.tabLiveTracking.Text = "Live Tracking";
            this.tabLiveTracking.UseVisualStyleBackColor = true;
            // 
            // panelLiveTracking
            // 
            this.panelLiveTracking.Controls.Add(this.groupRecentActivity);
            this.panelLiveTracking.Controls.Add(this.groupTodayStats);
            this.panelLiveTracking.Controls.Add(this.groupCurrentReaders);
            this.panelLiveTracking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLiveTracking.Location = new System.Drawing.Point(3, 3);
            this.panelLiveTracking.Name = "panelLiveTracking";
            this.panelLiveTracking.Size = new System.Drawing.Size(336, 318);
            this.panelLiveTracking.TabIndex = 0;
            // 
            // groupCurrentReaders
            // 
            this.groupCurrentReaders.Controls.Add(this.listCurrentReaders);
            this.groupCurrentReaders.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupCurrentReaders.Location = new System.Drawing.Point(0, 0);
            this.groupCurrentReaders.Name = "groupCurrentReaders";
            this.groupCurrentReaders.Size = new System.Drawing.Size(336, 100);
            this.groupCurrentReaders.TabIndex = 0;
            this.groupCurrentReaders.TabStop = false;
            this.groupCurrentReaders.Text = "Currently Reading";
            // 
            // listCurrentReaders
            // 
            this.listCurrentReaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnReaderEmail,
            this.columnReaderPage,
            this.columnReaderDuration,
            this.columnReaderSince});
            this.listCurrentReaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listCurrentReaders.FullRowSelect = true;
            this.listCurrentReaders.GridLines = true;
            this.listCurrentReaders.HideSelection = false;
            this.listCurrentReaders.Location = new System.Drawing.Point(3, 16);
            this.listCurrentReaders.Name = "listCurrentReaders";
            this.listCurrentReaders.Size = new System.Drawing.Size(330, 81);
            this.listCurrentReaders.TabIndex = 0;
            this.listCurrentReaders.UseCompatibleStateImageBehavior = false;
            this.listCurrentReaders.View = System.Windows.Forms.View.Details;
            // 
            // columnReaderEmail
            // 
            this.columnReaderEmail.Text = "Email";
            this.columnReaderEmail.Width = 120;
            // 
            // columnReaderPage
            // 
            this.columnReaderPage.Text = "Page";
            this.columnReaderPage.Width = 60;
            // 
            // columnReaderDuration
            // 
            this.columnReaderDuration.Text = "Duration";
            this.columnReaderDuration.Width = 70;
            // 
            // columnReaderSince
            // 
            this.columnReaderSince.Text = "Since";
            this.columnReaderSince.Width = 70;
            // 
            // groupTodayStats
            // 
            this.groupTodayStats.Controls.Add(this.lblUniqueViewers);
            this.groupTodayStats.Controls.Add(this.lblPageViews);
            this.groupTodayStats.Controls.Add(this.lblLinksClicked);
            this.groupTodayStats.Controls.Add(this.lblEmailsOpened);
            this.groupTodayStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupTodayStats.Location = new System.Drawing.Point(0, 100);
            this.groupTodayStats.Name = "groupTodayStats";
            this.groupTodayStats.Size = new System.Drawing.Size(336, 80);
            this.groupTodayStats.TabIndex = 1;
            this.groupTodayStats.TabStop = false;
            this.groupTodayStats.Text = "Today\'s Stats";
            // 
            // lblUniqueViewers
            // 
            this.lblUniqueViewers.AutoSize = true;
            this.lblUniqueViewers.Location = new System.Drawing.Point(170, 40);
            this.lblUniqueViewers.Name = "lblUniqueViewers";
            this.lblUniqueViewers.Size = new System.Drawing.Size(87, 13);
            this.lblUniqueViewers.TabIndex = 3;
            this.lblUniqueViewers.Text = "Unique Viewers: 0";
            // 
            // lblPageViews
            // 
            this.lblPageViews.AutoSize = true;
            this.lblPageViews.Location = new System.Drawing.Point(170, 20);
            this.lblPageViews.Name = "lblPageViews";
            this.lblPageViews.Size = new System.Drawing.Size(72, 13);
            this.lblPageViews.TabIndex = 2;
            this.lblPageViews.Text = "Page Views: 0";
            // 
            // lblLinksClicked
            // 
            this.lblLinksClicked.AutoSize = true;
            this.lblLinksClicked.Location = new System.Drawing.Point(10, 40);
            this.lblLinksClicked.Name = "lblLinksClicked";
            this.lblLinksClicked.Size = new System.Drawing.Size(78, 13);
            this.lblLinksClicked.TabIndex = 1;
            this.lblLinksClicked.Text = "Links Clicked: 0";
            // 
            // lblEmailsOpened
            // 
            this.lblEmailsOpened.AutoSize = true;
            this.lblEmailsOpened.Location = new System.Drawing.Point(10, 20);
            this.lblEmailsOpened.Name = "lblEmailsOpened";
            this.lblEmailsOpened.Size = new System.Drawing.Size(85, 13);
            this.lblEmailsOpened.TabIndex = 0;
            this.lblEmailsOpened.Text = "Emails Opened: 0";
            // 
            // groupRecentActivity
            // 
            this.groupRecentActivity.Controls.Add(this.listRecentActivity);
            this.groupRecentActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupRecentActivity.Location = new System.Drawing.Point(0, 180);
            this.groupRecentActivity.Name = "groupRecentActivity";
            this.groupRecentActivity.Size = new System.Drawing.Size(336, 138);
            this.groupRecentActivity.TabIndex = 2;
            this.groupRecentActivity.TabStop = false;
            this.groupRecentActivity.Text = "Recent Activity";
            // 
            // listRecentActivity
            // 
            this.listRecentActivity.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnActivityType,
            this.columnActivityUser,
            this.columnActivityPage,
            this.columnActivityTime});
            this.listRecentActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listRecentActivity.FullRowSelect = true;
            this.listRecentActivity.GridLines = true;
            this.listRecentActivity.HideSelection = false;
            this.listRecentActivity.Location = new System.Drawing.Point(3, 16);
            this.listRecentActivity.Name = "listRecentActivity";
            this.listRecentActivity.Size = new System.Drawing.Size(330, 119);
            this.listRecentActivity.TabIndex = 0;
            this.listRecentActivity.UseCompatibleStateImageBehavior = false;
            this.listRecentActivity.View = System.Windows.Forms.View.Details;
            // 
            // columnActivityType
            // 
            this.columnActivityType.Text = "Type";
            this.columnActivityType.Width = 80;
            // 
            // columnActivityUser
            // 
            this.columnActivityUser.Text = "User";
            this.columnActivityUser.Width = 100;
            // 
            // columnActivityPage
            // 
            this.columnActivityPage.Text = "Page";
            this.columnActivityPage.Width = 50;
            // 
            // columnActivityTime
            // 
            this.columnActivityTime.Text = "Time";
            this.columnActivityTime.Width = 80;
            // 
            // tabAnalytics
            // 
            this.tabAnalytics.Controls.Add(this.panelAnalytics);
            this.tabAnalytics.Location = new System.Drawing.Point(4, 22);
            this.tabAnalytics.Name = "tabAnalytics";
            this.tabAnalytics.Padding = new System.Windows.Forms.Padding(3);
            this.tabAnalytics.Size = new System.Drawing.Size(342, 324);
            this.tabAnalytics.TabIndex = 1;
            this.tabAnalytics.Text = "Analytics";
            this.tabAnalytics.UseVisualStyleBackColor = true;
            // 
            // panelAnalytics
            // 
            this.panelAnalytics.Controls.Add(this.groupSummary);
            this.panelAnalytics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAnalytics.Location = new System.Drawing.Point(3, 3);
            this.panelAnalytics.Name = "panelAnalytics";
            this.panelAnalytics.Size = new System.Drawing.Size(336, 318);
            this.panelAnalytics.TabIndex = 0;
            // 
            // groupSummary
            // 
            this.groupSummary.Controls.Add(this.lblUniqueViewersTotal);
            this.groupSummary.Controls.Add(this.lblClickRate);
            this.groupSummary.Controls.Add(this.lblOpenRate);
            this.groupSummary.Controls.Add(this.lblTotalClicks);
            this.groupSummary.Controls.Add(this.lblTotalOpens);
            this.groupSummary.Controls.Add(this.lblTotalEmailsSent);
            this.groupSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupSummary.Location = new System.Drawing.Point(0, 0);
            this.groupSummary.Name = "groupSummary";
            this.groupSummary.Size = new System.Drawing.Size(336, 150);
            this.groupSummary.TabIndex = 0;
            this.groupSummary.TabStop = false;
            this.groupSummary.Text = "Summary Statistics";
            // 
            // lblUniqueViewersTotal
            // 
            this.lblUniqueViewersTotal.AutoSize = true;
            this.lblUniqueViewersTotal.Location = new System.Drawing.Point(170, 80);
            this.lblUniqueViewersTotal.Name = "lblUniqueViewersTotal";
            this.lblUniqueViewersTotal.Size = new System.Drawing.Size(87, 13);
            this.lblUniqueViewersTotal.TabIndex = 5;
            this.lblUniqueViewersTotal.Text = "Unique Viewers: 0";
            // 
            // lblClickRate
            // 
            this.lblClickRate.AutoSize = true;
            this.lblClickRate.Location = new System.Drawing.Point(170, 60);
            this.lblClickRate.Name = "lblClickRate";
            this.lblClickRate.Size = new System.Drawing.Size(69, 13);
            this.lblClickRate.TabIndex = 4;
            this.lblClickRate.Text = "Click Rate: 0%";
            // 
            // lblOpenRate
            // 
            this.lblOpenRate.AutoSize = true;
            this.lblOpenRate.Location = new System.Drawing.Point(170, 40);
            this.lblOpenRate.Name = "lblOpenRate";
            this.lblOpenRate.Size = new System.Drawing.Size(69, 13);
            this.lblOpenRate.TabIndex = 3;
            this.lblOpenRate.Text = "Open Rate: 0%";
            // 
            // lblTotalClicks
            // 
            this.lblTotalClicks.AutoSize = true;
            this.lblTotalClicks.Location = new System.Drawing.Point(10, 60);
            this.lblTotalClicks.Name = "lblTotalClicks";
            this.lblTotalClicks.Size = new System.Drawing.Size(71, 13);
            this.lblTotalClicks.TabIndex = 2;
            this.lblTotalClicks.Text = "Total Clicks: 0";
            // 
            // lblTotalOpens
            // 
            this.lblTotalOpens.AutoSize = true;
            this.lblTotalOpens.Location = new System.Drawing.Point(10, 40);
            this.lblTotalOpens.Name = "lblTotalOpens";
            this.lblTotalOpens.Size = new System.Drawing.Size(71, 13);
            this.lblTotalOpens.TabIndex = 1;
            this.lblTotalOpens.Text = "Total Opens: 0";
            // 
            // lblTotalEmailsSent
            // 
            this.lblTotalEmailsSent.AutoSize = true;
            this.lblTotalEmailsSent.Location = new System.Drawing.Point(10, 20);
            this.lblTotalEmailsSent.Name = "lblTotalEmailsSent";
            this.lblTotalEmailsSent.Size = new System.Drawing.Size(78, 13);
            this.lblTotalEmailsSent.TabIndex = 0;
            this.lblTotalEmailsSent.Text = "Total Emails: 0";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(270, 50);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Location = new System.Drawing.Point(0, 430);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(350, 20);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LiveTrackingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlTracking);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.lblStatus);
            this.Name = "LiveTrackingForm";
            this.Size = new System.Drawing.Size(350, 450);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.tabControlTracking.ResumeLayout(false);
            this.tabLiveTracking.ResumeLayout(false);
            this.panelLiveTracking.ResumeLayout(false);
            this.groupCurrentReaders.ResumeLayout(false);
            this.groupTodayStats.ResumeLayout(false);
            this.groupTodayStats.PerformLayout();
            this.groupRecentActivity.ResumeLayout(false);
            this.tabAnalytics.ResumeLayout(false);
            this.panelAnalytics.ResumeLayout(false);
            this.groupSummary.ResumeLayout(false);
            this.groupSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.TabControl tabControlTracking;
        private System.Windows.Forms.TabPage tabLiveTracking;
        private System.Windows.Forms.TabPage tabAnalytics;
        private System.Windows.Forms.Button btnSelectDocument;
        private System.Windows.Forms.TextBox txtSelectedDocument;
        private System.Windows.Forms.Label lblDocument;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblConnectionStatus;
        private System.Windows.Forms.Panel panelLiveTracking;
        private System.Windows.Forms.GroupBox groupCurrentReaders;
        private System.Windows.Forms.ListView listCurrentReaders;
        private System.Windows.Forms.ColumnHeader columnReaderEmail;
        private System.Windows.Forms.ColumnHeader columnReaderPage;
        private System.Windows.Forms.ColumnHeader columnReaderDuration;
        private System.Windows.Forms.ColumnHeader columnReaderSince;
        private System.Windows.Forms.GroupBox groupTodayStats;
        private System.Windows.Forms.Label lblUniqueViewers;
        private System.Windows.Forms.Label lblPageViews;
        private System.Windows.Forms.Label lblLinksClicked;
        private System.Windows.Forms.Label lblEmailsOpened;
        private System.Windows.Forms.GroupBox groupRecentActivity;
        private System.Windows.Forms.ListView listRecentActivity;
        private System.Windows.Forms.ColumnHeader columnActivityType;
        private System.Windows.Forms.ColumnHeader columnActivityUser;
        private System.Windows.Forms.ColumnHeader columnActivityPage;
        private System.Windows.Forms.ColumnHeader columnActivityTime;
        private System.Windows.Forms.Panel panelAnalytics;
        private System.Windows.Forms.GroupBox groupSummary;
        private System.Windows.Forms.Label lblUniqueViewersTotal;
        private System.Windows.Forms.Label lblClickRate;
        private System.Windows.Forms.Label lblOpenRate;
        private System.Windows.Forms.Label lblTotalClicks;
        private System.Windows.Forms.Label lblTotalOpens;
        private System.Windows.Forms.Label lblTotalEmailsSent;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblStatus;
    }
}