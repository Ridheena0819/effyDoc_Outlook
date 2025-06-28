namespace EffyDocOutlookPlugin
{
    partial class EffyDocRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public EffyDocRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.groupEffyDoc = this.Factory.CreateRibbonGroup();
            this.btnAttachDocument = this.Factory.CreateRibbonButton();
            this.btnViewAnalytics = this.Factory.CreateRibbonButton();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.btnNewProposal = this.Factory.CreateRibbonButton();
            this.btnNewContract = this.Factory.CreateRibbonButton();
            this.separator2 = this.Factory.CreateRibbonSeparator();
            this.btnSettings = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.groupEffyDoc.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.ControlId.OfficeId = "TabMail";
            this.tab1.Groups.Add(this.groupEffyDoc);
            this.tab1.Label = "TabMail";
            // 
            // groupEffyDoc
            // 
            this.groupEffyDoc.Items.Add(this.btnAttachDocument);
            this.groupEffyDoc.Items.Add(this.btnViewAnalytics);
            this.groupEffyDoc.Items.Add(this.separator1);
            this.groupEffyDoc.Items.Add(this.btnNewProposal);
            this.groupEffyDoc.Items.Add(this.btnNewContract);
            this.groupEffyDoc.Items.Add(this.separator2);
            this.groupEffyDoc.Items.Add(this.btnSettings);
            this.groupEffyDoc.Label = "effyDOC";
            // 
            // btnAttachDocument
            // 
            this.btnAttachDocument.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnAttachDocument.Image = global::EffyDocOutlookPlugin.Properties.Resources.attach_document_32;
            this.btnAttachDocument.Label = "Attach Document";
            this.btnAttachDocument.Name = "btnAttachDocument";
            this.btnAttachDocument.ScreenTip = "Attach effyDOC Document";
            this.btnAttachDocument.SuperTip = "Attach a trackable document from your effyDOC library to this email. Recipients will be able to view and interact with the document while you track their engagement.";
            this.btnAttachDocument.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAttachDocument_Click);
            // 
            // btnViewAnalytics
            // 
            this.btnViewAnalytics.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnViewAnalytics.Image = global::EffyDocOutlookPlugin.Properties.Resources.analytics_32;
            this.btnViewAnalytics.Label = "View Analytics";
            this.btnViewAnalytics.Name = "btnViewAnalytics";
            this.btnViewAnalytics.ScreenTip = "Document Analytics";
            this.btnViewAnalytics.SuperTip = "View real-time analytics for your sent documents including open rates, time spent reading, and page-by-page engagement metrics.";
            this.btnViewAnalytics.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnViewAnalytics_Click);
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // btnNewProposal
            // 
            this.btnNewProposal.Image = global::EffyDocOutlookPlugin.Properties.Resources.proposal_16;
            this.btnNewProposal.Label = "New Proposal";
            this.btnNewProposal.Name = "btnNewProposal";
            this.btnNewProposal.ScreenTip = "Create New Proposal";
            this.btnNewProposal.SuperTip = "Create a new business proposal using AI-powered templates and smart content suggestions.";
            this.btnNewProposal.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnNewProposal_Click);
            // 
            // btnNewContract
            // 
            this.btnNewContract.Image = global::EffyDocOutlookPlugin.Properties.Resources.contract_16;
            this.btnNewContract.Label = "New Contract";
            this.btnNewContract.Name = "btnNewContract";
            this.btnNewContract.ScreenTip = "Create New Contract";
            this.btnNewContract.SuperTip = "Create a new contract or agreement with professional templates and legal clause suggestions.";
            this.btnNewContract.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnNewContract_Click);
            // 
            // separator2
            // 
            this.separator2.Name = "separator2";
            // 
            // btnSettings
            // 
            this.btnSettings.Image = global::EffyDocOutlookPlugin.Properties.Resources.settings_16;
            this.btnSettings.Label = "Settings";
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.ScreenTip = "effyDOC Settings";
            this.btnSettings.SuperTip = "Configure your effyDOC account settings, authentication, and plugin preferences.";
            this.btnSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSettings_Click);
            // 
            // EffyDocRibbon
            // 
            this.Name = "EffyDocRibbon";
            this.RibbonType = "Microsoft.Outlook.Mail.Compose";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.EffyDocRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.groupEffyDoc.ResumeLayout(false);
            this.groupEffyDoc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupEffyDoc;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAttachDocument;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnViewAnalytics;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnNewProposal;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnNewContract;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSettings;
    }
}