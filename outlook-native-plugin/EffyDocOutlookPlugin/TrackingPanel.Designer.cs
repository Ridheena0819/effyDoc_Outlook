namespace EffyDocOutlookPlugin
{
    partial class TrackingPanel
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblTotalDocuments = new System.Windows.Forms.Label();
            this.lblTotalViews = new System.Windows.Forms.Label();
            this.lblActiveToday = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pnlDocuments = new System.Windows.Forms.Panel();
            this.lstDocuments = new System.Windows.Forms.ListView();
            this.colDocTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDocType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDocViews = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDocUpdated = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblDocumentsList = new System.Windows.Forms.Label();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.pnlDetailContent = new System.Windows.Forms.Panel();
            this.pnlMetrics = new System.Windows.Forms.Panel();
            this.lblTotalOpens = new System.Windows.Forms.Label();
            this.lblTotalClicks = new System.Windows.Forms.Label();
            this.lblUniqueViewers = new System.Windows.Forms.Label();
            this.lblOpenRate = new System.Windows.Forms.Label();
            this.lblClickRate = new System.Windows.Forms.Label();
            this.pnlActivity = new System.Windows.Forms.Panel();
            this.lstRecentActivity = new System.Windows.Forms.ListView();
            this.colActivityEvent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActivityUser = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActivityTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblRecentActivity = new System.Windows.Forms.Label();
            this.pnlDetailHeader = new System.Windows.Forms.Panel();
            this.btnShareDocument = new System.Windows.Forms.Button();
            this.btnViewOnline = new System.Windows.Forms.Button();
            this.lblDocumentInfo = new System.Windows.Forms.Label();
            this.lblDocumentTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlDocuments.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.pnlDetailContent.SuspendLayout();
            this.pnlMetrics.SuspendLayout();
            this.pnlActivity.SuspendLayout();
            this.pnlDetailHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.pnlHeader.Controls.Add(this.chkAutoRefresh);
            this.pnlHeader.Controls.Add(this.btnRefresh);
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(984, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 19);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(309, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📊 effyDOC Analytics Dashboard";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(930, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(42, 30);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "✕";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(845, 15);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 30);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "🔄 Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // chkAutoRefresh
            // 
            this.chkAutoRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoRefresh.AutoSize = true;
            this.chkAutoRefresh.Checked = true;
            this.chkAutoRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoRefresh.ForeColor = System.Drawing.Color.White;
            this.chkAutoRefresh.Location = new System.Drawing.Point(740, 22);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(88, 17);
            this.chkAutoRefresh.TabIndex = 3;
            this.chkAutoRefresh.Text = "Auto Refresh";
            this.chkAutoRefresh.UseVisualStyleBackColor = true;
            this.chkAutoRefresh.CheckedChanged += new System.EventHandler(this.chkAutoRefresh_CheckedChanged);
            // 
            // pnlSummary
            // 
            this.pnlSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlSummary.Controls.Add(this.lblActiveToday);
            this.pnlSummary.Controls.Add(this.lblTotalViews);
            this.pnlSummary.Controls.Add(this.lblTotalDocuments);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSummary.Location = new System.Drawing.Point(0, 60);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Size = new System.Drawing.Size(984, 60);
            this.pnlSummary.TabIndex = 1;
            // 
            // lblTotalDocuments
            // 
            this.lblTotalDocuments.AutoSize = true;
            this.lblTotalDocuments.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalDocuments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.lblTotalDocuments.Location = new System.Drawing.Point(20, 20);
            this.lblTotalDocuments.Name = "lblTotalDocuments";
            this.lblTotalDocuments.Size = new System.Drawing.Size(127, 20);
            this.lblTotalDocuments.TabIndex = 0;
            this.lblTotalDocuments.Text = "📄 - Documents";
            // 
            // lblTotalViews
            // 
            this.lblTotalViews.AutoSize = true;
            this.lblTotalViews.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalViews.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.lblTotalViews.Location = new System.Drawing.Point(200, 20);
            this.lblTotalViews.Name = "lblTotalViews";
            this.lblTotalViews.Size = new System.Drawing.Size(124, 20);
            this.lblTotalViews.TabIndex = 1;
            this.lblTotalViews.Text = "👁 - Total Views";
            // 
            // lblActiveToday
            // 
            this.lblActiveToday.AutoSize = true;
            this.lblActiveToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveToday.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(101)))), ((int)(((byte)(101)))));
            this.lblActiveToday.Location = new System.Drawing.Point(380, 20);
            this.lblActiveToday.Name = "lblActiveToday";
            this.lblActiveToday.Size = new System.Drawing.Size(134, 20);
            this.lblActiveToday.TabIndex = 2;
            this.lblActiveToday.Text = "🔥 - Active Today";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 120);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.pnlDocuments);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pnlDetails);
            this.splitContainer.Size = new System.Drawing.Size(984, 541);
            this.splitContainer.SplitterDistance = 400;
            this.splitContainer.TabIndex = 2;
            // 
            // pnlDocuments
            // 
            this.pnlDocuments.Controls.Add(this.lstDocuments);
            this.pnlDocuments.Controls.Add(this.lblDocumentsList);
            this.pnlDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDocuments.Location = new System.Drawing.Point(0, 0);
            this.pnlDocuments.Name = "pnlDocuments";
            this.pnlDocuments.Padding = new System.Windows.Forms.Padding(10);
            this.pnlDocuments.Size = new System.Drawing.Size(400, 541);
            this.pnlDocuments.TabIndex = 0;
            // 
            // lstDocuments
            // 
            this.lstDocuments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDocTitle,
            this.colDocType,
            this.colDocViews,
            this.colDocUpdated});
            this.lstDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDocuments.FullRowSelect = true;
            this.lstDocuments.GridLines = true;
            this.lstDocuments.HideSelection = false;
            this.lstDocuments.Location = new System.Drawing.Point(10, 35);
            this.lstDocuments.MultiSelect = false;
            this.lstDocuments.Name = "lstDocuments";
            this.lstDocuments.Size = new System.Drawing.Size(380, 496);
            this.lstDocuments.TabIndex = 1;
            this.lstDocuments.UseCompatibleStateImageBehavior = false;
            this.lstDocuments.View = System.Windows.Forms.View.Details;
            this.lstDocuments.SelectedIndexChanged += new System.EventHandler(this.lstDocuments_SelectedIndexChanged);
            // 
            // colDocTitle
            // 
            this.colDocTitle.Text = "Document";
            this.colDocTitle.Width = 180;
            // 
            // colDocType
            // 
            this.colDocType.Text = "Type";
            this.colDocType.Width = 80;
            // 
            // colDocViews
            // 
            this.colDocViews.Text = "Views";
            this.colDocViews.Width = 60;
            // 
            // colDocUpdated
            // 
            this.colDocUpdated.Text = "Updated";
            this.colDocUpdated.Width = 100;
            // 
            // lblDocumentsList
            // 
            this.lblDocumentsList.AutoSize = true;
            this.lblDocumentsList.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDocumentsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDocumentsList.Location = new System.Drawing.Point(10, 10);
            this.lblDocumentsList.Name = "lblDocumentsList";
            this.lblDocumentsList.Padding = new System.Windows.Forms.Padding(0, 5, 0, 10);
            this.lblDocumentsList.Size = new System.Drawing.Size(133, 31);
            this.lblDocumentsList.TabIndex = 0;
            this.lblDocumentsList.Text = "📋 Your Documents";
            // 
            // pnlDetails
            // 
            this.pnlDetails.Controls.Add(this.pnlDetailContent);
            this.pnlDetails.Controls.Add(this.pnlDetailHeader);
            this.pnlDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetails.Location = new System.Drawing.Point(0, 0);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(580, 541);
            this.pnlDetails.TabIndex = 0;
            // 
            // pnlDetailContent
            // 
            this.pnlDetailContent.Controls.Add(this.pnlActivity);
            this.pnlDetailContent.Controls.Add(this.pnlMetrics);
            this.pnlDetailContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetailContent.Location = new System.Drawing.Point(0, 100);
            this.pnlDetailContent.Name = "pnlDetailContent";
            this.pnlDetailContent.Padding = new System.Windows.Forms.Padding(10);
            this.pnlDetailContent.Size = new System.Drawing.Size(580, 441);
            this.pnlDetailContent.TabIndex = 1;
            // 
            // pnlMetrics
            // 
            this.pnlMetrics.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlMetrics.Controls.Add(this.lblClickRate);
            this.pnlMetrics.Controls.Add(this.lblOpenRate);
            this.pnlMetrics.Controls.Add(this.lblUniqueViewers);
            this.pnlMetrics.Controls.Add(this.lblTotalClicks);
            this.pnlMetrics.Controls.Add(this.lblTotalOpens);
            this.pnlMetrics.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMetrics.Location = new System.Drawing.Point(10, 10);
            this.pnlMetrics.Name = "pnlMetrics";
            this.pnlMetrics.Padding = new System.Windows.Forms.Padding(15);
            this.pnlMetrics.Size = new System.Drawing.Size(560, 120);
            this.pnlMetrics.TabIndex = 0;
            // 
            // lblTotalOpens
            // 
            this.lblTotalOpens.AutoSize = true;
            this.lblTotalOpens.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalOpens.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.lblTotalOpens.Location = new System.Drawing.Point(18, 18);
            this.lblTotalOpens.Name = "lblTotalOpens";
            this.lblTotalOpens.Size = new System.Drawing.Size(90, 17);
            this.lblTotalOpens.TabIndex = 0;
            this.lblTotalOpens.Text = "📧 - Opens";
            // 
            // lblTotalClicks
            // 
            this.lblTotalClicks.AutoSize = true;
            this.lblTotalClicks.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalClicks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.lblTotalClicks.Location = new System.Drawing.Point(150, 18);
            this.lblTotalClicks.Name = "lblTotalClicks";
            this.lblTotalClicks.Size = new System.Drawing.Size(86, 17);
            this.lblTotalClicks.TabIndex = 1;
            this.lblTotalClicks.Text = "🖱 - Clicks";
            // 
            // lblUniqueViewers
            // 
            this.lblUniqueViewers.AutoSize = true;
            this.lblUniqueViewers.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUniqueViewers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(85)))), ((int)(((byte)(247)))));
            this.lblUniqueViewers.Location = new System.Drawing.Point(290, 18);
            this.lblUniqueViewers.Name = "lblUniqueViewers";
            this.lblUniqueViewers.Size = new System.Drawing.Size(150, 17);
            this.lblUniqueViewers.TabIndex = 2;
            this.lblUniqueViewers.Text = "👥 - Unique Viewers";
            // 
            // lblOpenRate
            // 
            this.lblOpenRate.AutoSize = true;
            this.lblOpenRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOpenRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(101)))), ((int)(((byte)(101)))));
            this.lblOpenRate.Location = new System.Drawing.Point(18, 55);
            this.lblOpenRate.Name = "lblOpenRate";
            this.lblOpenRate.Size = new System.Drawing.Size(124, 17);
            this.lblOpenRate.TabIndex = 3;
            this.lblOpenRate.Text = "📈 - Open Rate";
            // 
            // lblClickRate
            // 
            this.lblClickRate.AutoSize = true;
            this.lblClickRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClickRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.lblClickRate.Location = new System.Drawing.Point(200, 55);
            this.lblClickRate.Name = "lblClickRate";
            this.lblClickRate.Size = new System.Drawing.Size(119, 17);
            this.lblClickRate.TabIndex = 4;
            this.lblClickRate.Text = "📊 - Click Rate";
            // 
            // pnlActivity
            // 
            this.pnlActivity.Controls.Add(this.lstRecentActivity);
            this.pnlActivity.Controls.Add(this.lblRecentActivity);
            this.pnlActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlActivity.Location = new System.Drawing.Point(10, 130);
            this.pnlActivity.Name = "pnlActivity";
            this.pnlActivity.Size = new System.Drawing.Size(560, 301);
            this.pnlActivity.TabIndex = 1;
            // 
            // lstRecentActivity
            // 
            this.lstRecentActivity.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colActivityEvent,
            this.colActivityUser,
            this.colActivityTime});
            this.lstRecentActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRecentActivity.FullRowSelect = true;
            this.lstRecentActivity.GridLines = true;
            this.lstRecentActivity.HideSelection = false;
            this.lstRecentActivity.Location = new System.Drawing.Point(0, 25);
            this.lstRecentActivity.Name = "lstRecentActivity";
            this.lstRecentActivity.Size = new System.Drawing.Size(560, 276);
            this.lstRecentActivity.TabIndex = 1;
            this.lstRecentActivity.UseCompatibleStateImageBehavior = false;
            this.lstRecentActivity.View = System.Windows.Forms.View.Details;
            // 
            // colActivityEvent
            // 
            this.colActivityEvent.Text = "Event";
            this.colActivityEvent.Width = 200;
            // 
            // colActivityUser
            // 
            this.colActivityUser.Text = "User";
            this.colActivityUser.Width = 250;
            // 
            // colActivityTime
            // 
            this.colActivityTime.Text = "Time";
            this.colActivityTime.Width = 80;
            // 
            // lblRecentActivity
            // 
            this.lblRecentActivity.AutoSize = true;
            this.lblRecentActivity.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRecentActivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecentActivity.Location = new System.Drawing.Point(0, 0);
            this.lblRecentActivity.Name = "lblRecentActivity";
            this.lblRecentActivity.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblRecentActivity.Size = new System.Drawing.Size(126, 26);
            this.lblRecentActivity.TabIndex = 0;
            this.lblRecentActivity.Text = "⚡ Recent Activity";
            // 
            // pnlDetailHeader
            // 
            this.pnlDetailHeader.BackColor = System.Drawing.Color.White;
            this.pnlDetailHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDetailHeader.Controls.Add(this.btnShareDocument);
            this.pnlDetailHeader.Controls.Add(this.btnViewOnline);
            this.pnlDetailHeader.Controls.Add(this.lblDocumentInfo);
            this.pnlDetailHeader.Controls.Add(this.lblDocumentTitle);
            this.pnlDetailHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDetailHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlDetailHeader.Name = "pnlDetailHeader";
            this.pnlDetailHeader.Size = new System.Drawing.Size(580, 100);
            this.pnlDetailHeader.TabIndex = 0;
            // 
            // btnShareDocument
            // 
            this.btnShareDocument.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShareDocument.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnShareDocument.Enabled = false;
            this.btnShareDocument.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShareDocument.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShareDocument.ForeColor = System.Drawing.Color.White;
            this.btnShareDocument.Location = new System.Drawing.Point(480, 55);
            this.btnShareDocument.Name = "btnShareDocument";
            this.btnShareDocument.Size = new System.Drawing.Size(85, 30);
            this.btnShareDocument.TabIndex = 3;
            this.btnShareDocument.Text = "📋 Copy Link";
            this.btnShareDocument.UseVisualStyleBackColor = false;
            this.btnShareDocument.Click += new System.EventHandler(this.btnShareDocument_Click);
            // 
            // btnViewOnline
            // 
            this.btnViewOnline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewOnline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnViewOnline.Enabled = false;
            this.btnViewOnline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewOnline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewOnline.ForeColor = System.Drawing.Color.White;
            this.btnViewOnline.Location = new System.Drawing.Point(480, 15);
            this.btnViewOnline.Name = "btnViewOnline";
            this.btnViewOnline.Size = new System.Drawing.Size(85, 30);
            this.btnViewOnline.TabIndex = 2;
            this.btnViewOnline.Text = "🌐 View Online";
            this.btnViewOnline.UseVisualStyleBackColor = false;
            this.btnViewOnline.Click += new System.EventHandler(this.btnViewOnline_Click);
            // 
            // lblDocumentInfo
            // 
            this.lblDocumentInfo.AutoSize = true;
            this.lblDocumentInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblDocumentInfo.Location = new System.Drawing.Point(15, 55);
            this.lblDocumentInfo.Name = "lblDocumentInfo";
            this.lblDocumentInfo.Size = new System.Drawing.Size(0, 13);
            this.lblDocumentInfo.TabIndex = 1;
            // 
            // lblDocumentTitle
            // 
            this.lblDocumentTitle.AutoSize = true;
            this.lblDocumentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDocumentTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblDocumentTitle.Location = new System.Drawing.Point(15, 20);
            this.lblDocumentTitle.Name = "lblDocumentTitle";
            this.lblDocumentTitle.Size = new System.Drawing.Size(283, 20);
            this.lblDocumentTitle.TabIndex = 0;
            this.lblDocumentTitle.Text = "📊 Select a document to view analytics";
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblStatus.Location = new System.Drawing.Point(12, 643);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Ready";
            this.lblStatus.Visible = false;
            // 
            // TrackingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.pnlHeader);
            this.Name = "TrackingPanel";
            this.Text = "effyDOC Analytics Dashboard";
            this.Load += new System.EventHandler(this.TrackingPanel_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrackingPanel_FormClosing);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).ResumeLayout(false);
            this.pnlDocuments.ResumeLayout(false);
            this.pnlDocuments.PerformLayout();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetailContent.ResumeLayout(false);
            this.pnlMetrics.ResumeLayout(false);
            this.pnlMetrics.PerformLayout();
            this.pnlActivity.ResumeLayout(false);
            this.pnlActivity.PerformLayout();
            this.pnlDetailHeader.ResumeLayout(false);
            this.pnlDetailHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblActiveToday;
        private System.Windows.Forms.Label lblTotalViews;
        private System.Windows.Forms.Label lblTotalDocuments;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnlDocuments;
        private System.Windows.Forms.ListView lstDocuments;
        private System.Windows.Forms.ColumnHeader colDocTitle;
        private System.Windows.Forms.ColumnHeader colDocType;
        private System.Windows.Forms.ColumnHeader colDocViews;
        private System.Windows.Forms.ColumnHeader colDocUpdated;
        private System.Windows.Forms.Label lblDocumentsList;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Panel pnlDetailContent;
        private System.Windows.Forms.Panel pnlActivity;
        private System.Windows.Forms.ListView lstRecentActivity;
        private System.Windows.Forms.ColumnHeader colActivityEvent;
        private System.Windows.Forms.ColumnHeader colActivityUser;
        private System.Windows.Forms.ColumnHeader colActivityTime;
        private System.Windows.Forms.Label lblRecentActivity;
        private System.Windows.Forms.Panel pnlMetrics;
        private System.Windows.Forms.Label lblClickRate;
        private System.Windows.Forms.Label lblOpenRate;
        private System.Windows.Forms.Label lblUniqueViewers;
        private System.Windows.Forms.Label lblTotalClicks;
        private System.Windows.Forms.Label lblTotalOpens;
        private System.Windows.Forms.Panel pnlDetailHeader;
        private System.Windows.Forms.Button btnShareDocument;
        private System.Windows.Forms.Button btnViewOnline;
        private System.Windows.Forms.Label lblDocumentInfo;
        private System.Windows.Forms.Label lblDocumentTitle;
        private System.Windows.Forms.Label lblStatus;
    }
}